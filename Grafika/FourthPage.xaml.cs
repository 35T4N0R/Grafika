using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Grafika
{
    /// <summary>
    /// Logika interakcji dla klasy FourthPage.xaml
    /// </summary>
    public partial class FourthPage : Page
    {
        public FourthPage()
        {
            InitializeComponent();
        }

        private ImageSource originalImage;
        private int[,] customMask;

        #region helpers
        public static BitmapSource ConvertBitmap(Bitmap source)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                          source.GetHbitmap(),
                          IntPtr.Zero,
                          Int32Rect.Empty,
                          BitmapSizeOptions.FromEmptyOptions());
        }

        public static Bitmap BitmapFromSource(BitmapSource bitmapsource)
        {
            Bitmap bitmap;
            using (var outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                bitmap = new Bitmap(outStream);
            }
            return bitmap;
        }

        public (int[], int[], int[]) CalcHistogram()
        {
            var bitmap = BitmapFromSource((BitmapSource)Image.Source);

            if (CheckIfGray())
            {
                var values = new int[256];

                for (int i = 0; i < bitmap.Width; i++)
                {
                    for (int j = 0; j < bitmap.Height; j++)
                    {
                        var pixel = bitmap.GetPixel(i, j);

                        values[pixel.R] += 1;
                    }
                }

                return (values, null, null);
            }
            else
            {
                var values_r = new int[256];
                var values_g = new int[256];
                var values_b = new int[256];

                for (int i = 0; i < bitmap.Width; i++)
                {
                    for (int j = 0; j < bitmap.Height; j++)
                    {
                        var pixel = bitmap.GetPixel(i, j);

                        values_r[pixel.R] += 1;
                        values_g[pixel.G] += 1;
                        values_b[pixel.B] += 1;
                    }
                }

                return (values_r, values_g, values_b);
            }


        }

        public List<double> CalcDistribution(int[] values, int pixelAmount)
        {
            var distribution = new List<double>();
            double dist = 0.0;

            for (int i = 0; i < values.Length; i++)
            {
                dist += ((double)values[i] / (double)pixelAmount);
                distribution.Add(dist);
            }

            return distribution;
        }

        public List<int> CalcLUTTable(List<double> distribution)
        {
            var LUT_table = new List<int>();
            var m = 256;

            for (int i = 0; i < distribution.Count; i++)
            {
                LUT_table.Add((int)((distribution[i] - distribution[0]) / (1 - distribution[0]) * (m - 1)));
            }
            return LUT_table;
        }

        public bool CheckIfGray()
        {
            var bitmap = BitmapFromSource((BitmapSource)Image.Source);

            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    var pixel = bitmap.GetPixel(i, j);

                    if (pixel.R == pixel.G && pixel.G == pixel.B) continue;
                    else return false;
                }
            }

            return true;
        }

        public List<int> StretchFunction(List<int> values)
        {
            var a = Convert.ToInt32(ABriBox.Text);
            var b = Convert.ToInt32(BBriBox.Text);

            var stretched_values = new List<int>();

            for (int i = 0; i < values.Count; i++)
            {
                if (values[i] < a)
                {
                    stretched_values.Add(0);
                }
                else if (values[i] > b)
                {
                    stretched_values.Add(255);
                }
                else
                {
                    stretched_values.Add((int)(((double)(values[i] - a) / (double)(b - a)) * 255));
                }
            }

            return stretched_values;
        }

        public BitmapSource ConvOperation(int[,] mask)
        {
            var bitmap = BitmapFromSource((BitmapSource)originalImage);

            var copy = BitmapFromSource((BitmapSource)originalImage);//bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), bitmap.PixelFormat);

            var maskRange = (int)(mask.GetLength(0) / 2);

            var sumInMask = 0;
            for (int i = 0; i < mask.GetLength(0); i++)
            {
                for (int j = 0; j < mask.GetLength(1); j++)
                {
                    sumInMask += mask[i, j];
                }
            }

            if (sumInMask == 0) sumInMask = 1;

            if (CheckIfGray())
            {
                int sum = 0;

                for (int i = 0; i < bitmap.Width; i++)
                {
                    for (int j = 0; j < bitmap.Height; j++)
                    {

                        sum = 0;

                        for (int x = -1 * maskRange; x < maskRange + 1; x++)
                        {
                            for (int y = -1 * maskRange; y < maskRange + 1; y++)
                            {
                                if (i + x >= 0 && i + x < bitmap.Width && j + y >= 0 && j + y < bitmap.Height)
                                {
                                    var pixel = bitmap.GetPixel(i + x, j + y);
                                    sum += pixel.R * (int)(mask[x + maskRange, y + maskRange]);
                                }
                                else
                                {
                                    sum += 0;
                                }
                            }
                        }


                        var value = (int)(sum / sumInMask);

                        if (value > 255) value = 255;
                        else if (value < 0) value = 0;


                        copy.SetPixel(i, j, System.Drawing.Color.FromArgb(value, value, value));

                    }
                }

                return ConvertBitmap(copy);
            }
            else
            {
                int sum_r = 0;
                int sum_g = 0;
                int sum_b = 0;

                for (int i = 0; i < bitmap.Width; i++)
                {
                    for (int j = 0; j < bitmap.Height; j++)
                    {

                        sum_r = 0;
                        sum_g = 0;
                        sum_b = 0;

                        for (int x = -1 * maskRange; x < maskRange + 1; x++)
                        {
                            for (int y = -1 * maskRange; y < maskRange + 1; y++)
                            {
                                if (i + x >= 0 && i + x < bitmap.Width && j + y >= 0 && j + y < bitmap.Height)
                                {
                                    var pixel = bitmap.GetPixel(i + x, j + y);
                                    sum_r += pixel.R * (int)(mask[x + maskRange, y + maskRange]);
                                    sum_g += pixel.G * (int)(mask[x + maskRange, y + maskRange]);
                                    sum_b += pixel.B * (int)(mask[x + maskRange, y + maskRange]);
                                }
                                else
                                {
                                    sum_r += 0;
                                    sum_g += 0;
                                    sum_b += 0;
                                }
                            }
                        }


                        var r_value = (int)(sum_r / sumInMask);
                        var g_value = (int)(sum_g / sumInMask);
                        var b_value = (int)(sum_b / sumInMask);

                        if (r_value > 255) r_value = 255;
                        else if (r_value < 0) r_value = 0;

                        if (g_value > 255) g_value = 255;
                        else if (g_value < 0) g_value = 0;

                        if (b_value > 255) b_value = 255;
                        else if (b_value < 0) b_value = 0;

                        copy.SetPixel(i, j, System.Drawing.Color.FromArgb(r_value, g_value, b_value));

                    }
                }

                return ConvertBitmap(copy);

            }

        }
        #endregion

        private void LoadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                ImageSource imageSource = new BitmapImage(new Uri(ofd.FileName));
                Image.Source = imageSource;
                originalImage = imageSource;
            }
        }

        private void SaveImage_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "JPEG (*.jpeg)|*.jpeg|PNG (*.png)|*.png|All Files (*.*)|*.*";
            sfd.DefaultExt = ".jpeg";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.QualityLevel = 100;
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)Image.Source));
                using (FileStream stream = new FileStream(sfd.FileName, FileMode.Create))
                    encoder.Save(stream);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var bitmap = BitmapFromSource((BitmapSource)Image.Source);
            int value = 0;

            if (!String.IsNullOrEmpty(ValueBox.Text))
            {
                value = Convert.ToInt32(ValueBox.Text) >= 0 ? Convert.ToInt32(ValueBox.Text) : 0;

                for (int i = 0; i < bitmap.Width; i++)
                {
                    for (int j = 0; j < bitmap.Height; j++)
                    {
                        var pixel = bitmap.GetPixel(i, j);
                        var r = (pixel.R + value) % 256;
                        var g = (pixel.G + value) % 256;
                        var b = (pixel.B + value) % 256;
                        bitmap.SetPixel(i, j, System.Drawing.Color.FromArgb(r, g, b));
                    }
                }

                Image.Source = ConvertBitmap(bitmap);
            }
        }
        

        private void SubButton_Click(object sender, RoutedEventArgs e)
        {
            var bitmap = BitmapFromSource((BitmapSource)Image.Source);
            int value = 0;

            if (!String.IsNullOrEmpty(ValueBox.Text))
            {
                value = Convert.ToInt32(ValueBox.Text) >= 0 ? Convert.ToInt32(ValueBox.Text) : 0;

                for (int i = 0; i < bitmap.Width; i++)
                {
                    for (int j = 0; j < bitmap.Height; j++)
                    {
                        var pixel = bitmap.GetPixel(i, j);
                        var r = pixel.R - value < 0 ? 255 - Math.Abs(pixel.R - value) : pixel.R - value;
                        var g = pixel.G - value < 0 ? 255 - Math.Abs(pixel.G - value) : pixel.G - value;
                        var b = pixel.B - value < 0 ? 255 - Math.Abs(pixel.B - value) : pixel.B - value;
                        bitmap.SetPixel(i, j, System.Drawing.Color.FromArgb(r, g, b));
                    }
                }

                Image.Source = ConvertBitmap(bitmap);
            }
        }

        private void MultiButton_Click(object sender, RoutedEventArgs e)
        {
            var bitmap = BitmapFromSource((BitmapSource)Image.Source);
            int value = 0;

            if (!String.IsNullOrEmpty(ValueBox.Text))
            {
                value = Convert.ToInt32(ValueBox.Text) >= 0 ? Convert.ToInt32(ValueBox.Text) : 0;

                for (int i = 0; i < bitmap.Width; i++)
                {
                    for (int j = 0; j < bitmap.Height; j++)
                    {
                        var pixel = bitmap.GetPixel(i, j);
                        var r = (pixel.R * value) % 256;
                        var g = (pixel.G * value) % 256;
                        var b = (pixel.B * value) % 256;
                        bitmap.SetPixel(i, j, System.Drawing.Color.FromArgb(r, g, b));
                    }
                }

                Image.Source = ConvertBitmap(bitmap);
            }
        }

        private void DivButton_Click(object sender, RoutedEventArgs e)
        {
            var bitmap = BitmapFromSource((BitmapSource)Image.Source);
            int value = 0;

            if (!String.IsNullOrEmpty(ValueBox.Text))
            {
                value = Convert.ToInt32(ValueBox.Text) > 0 ? Convert.ToInt32(ValueBox.Text) : 1;

                for (int i = 0; i < bitmap.Width; i++)
                {
                    for (int j = 0; j < bitmap.Height; j++)
                    {
                        var pixel = bitmap.GetPixel(i, j);
                        var r = pixel.R / value < 0 ? 0 : pixel.R / value;
                        var g = pixel.G / value < 0 ? 0 : pixel.G / value;
                        var b = pixel.B / value < 0 ? 0 : pixel.B / value;
                        bitmap.SetPixel(i, j, System.Drawing.Color.FromArgb(r, g, b));
                    }
                }

                Image.Source = ConvertBitmap(bitmap);
            }
        }

        private void Gray1Button_Click(object sender, RoutedEventArgs e)
        {
            var bitmap = BitmapFromSource((BitmapSource)Image.Source);

            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    var pixel = bitmap.GetPixel(i, j);
                    bitmap.SetPixel(i, j, System.Drawing.Color.FromArgb(pixel.R, pixel.R, pixel.R));
                }
            }

            Image.Source = ConvertBitmap(bitmap);
        }

        private void Gray2Button_Click(object sender, RoutedEventArgs e)
        {
            var bitmap = BitmapFromSource((BitmapSource)Image.Source);

            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    var pixel = bitmap.GetPixel(i, j);

                    var value = (pixel.R + pixel.G + pixel.B) / 3;

                    bitmap.SetPixel(i, j, System.Drawing.Color.FromArgb(value, value, value));
                }
            }

            Image.Source = ConvertBitmap(bitmap);
        }

        private void BriButton_Click(object sender, RoutedEventArgs e)
        {
            var bitmap = BitmapFromSource((BitmapSource)originalImage);
            int value = 0;

            if (!String.IsNullOrEmpty(ValueBox.Text))
            {
                value = Convert.ToInt32(ValueBox.Text);

                for (int i = 0; i < bitmap.Width; i++)
                {
                    for (int j = 0; j < bitmap.Height; j++)
                    {
                        var pixel = bitmap.GetPixel(i, j);

                        var r = pixel.R + value >= 255 ? 255 : pixel.R + value <= 0 ? 0 : pixel.R + value;
                        var g = pixel.G + value >= 255 ? 255 : pixel.G + value <= 0 ? 0 : pixel.G + value;
                        var b = pixel.B + value >= 255 ? 255 : pixel.B + value <= 0 ? 0 : pixel.B + value;

                        bitmap.SetPixel(i, j, System.Drawing.Color.FromArgb(r, g, b));

                    }
                }

                Image.Source = ConvertBitmap(bitmap);
            }
        }

        private void MaskWindowButton_Click(object sender, RoutedEventArgs e)
        {

            if (!String.IsNullOrEmpty(MaskBox.Text))
            {
                var boxNumber = Convert.ToInt32(MaskBox.Text);

                if (boxNumber % 2 == 0)
                {
                    MessageBoxResult result = System.Windows.MessageBox.Show("Wielkość maski powinna być liczbą nieparzystą",
                                          "Error",
                                          MessageBoxButton.OK,
                                          MessageBoxImage.Error);
                    return;
                }

                var maskWindow = new MaskWindow(boxNumber);

                maskWindow.Width = 140 + 100 + boxNumber * 25 + (boxNumber - 1) * 20;
                maskWindow.Height = 100 * 2 + boxNumber * 20 + (boxNumber - 1) * 20;

                customMask = maskWindow.ShowDialog() == true ? maskWindow.mask : null;

                if(customMask == null)  MaskButton.IsEnabled = false;
                else  MaskButton.IsEnabled = true;
            }
        }

        private void MeanButton_Click(object sender, RoutedEventArgs e)
        {
            int[,] mask = new int[,]{{1, 1, 1 },
                                     {1, 1, 1 },
                                     {1, 1, 1 } };

            Image.Source = ConvOperation(mask);
        }

        private void SobelPozButton_Click(object sender, RoutedEventArgs e)
        {
            int[,] mask = new int[,]{{1, 2, 1 },
                                     {0, 0, 0 },
                                     {-1, -2, -1 } };

            Image.Source = ConvOperation(mask);
        }

        private void SobelPioButton_Click(object sender, RoutedEventArgs e)
        {
            int[,] mask = new int[,]{{-1, 0, 1 },
                                     {-2, 0, 2 },
                                     {-1, 0, 1 } };

            Image.Source = ConvOperation(mask);
        }

        private void SharpButton_Click(object sender, RoutedEventArgs e)
        {
            int[,] mask = new int[,]{{-1, -1, -1 },
                                     {-1, 9, -1},
                                     {-1, -1, -1 } };

            Image.Source = ConvOperation(mask);
        }

        private void GaussButton_Click(object sender, RoutedEventArgs e)
        {
            int[,] mask = new int[,]{{1, 2, 1 },
                                     {2, 4, 2 },
                                     {1, 2, 1 } };

            Image.Source = ConvOperation(mask);
        }

        private void MedianButton_Click(object sender, RoutedEventArgs e)
        {
            var bitmap = BitmapFromSource((BitmapSource)originalImage);

            var copy = BitmapFromSource((BitmapSource)originalImage);//bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), bitmap.PixelFormat);


            var size = 3;

            var maskRange = (int)(size / 2);

            var cellsInMask = (int)Math.Pow(size, 2);


            if (CheckIfGray())
            {
                for (int i = 0; i < bitmap.Width; i++)
                {
                    for (int j = 0; j < bitmap.Height; j++)
                    {

                        var values = new int[cellsInMask];
                        var iter = 0;

                        for (int x = -1 * maskRange; x < maskRange + 1; x++)
                        {
                            for (int y = -1 * maskRange; y < maskRange + 1; y++)
                            {
                                if(i + x >= 0 && i + x < bitmap.Width && j + y >= 0 && j + y < bitmap.Height)
                                {
                                    var pixel = bitmap.GetPixel(i + x, j + y);
                                    values[iter++] = pixel.R;
                                }
                            }
                        }

                        Array.Sort(values);

                        var median = values.Length % 2 != 0 ? values[values.Length / 2] : (int)((values[values.Length / 2] + values[(values.Length / 2) - 1]) / 2);

                        copy.SetPixel(i, j, System.Drawing.Color.FromArgb(median, median, median));
                    }
                }
            }
            else
            {
                for (int i = 0; i < bitmap.Width; i++)
                {
                    for (int j = 0; j < bitmap.Height; j++)
                    {

                        var values_r = new int[cellsInMask];
                        var values_g = new int[cellsInMask];
                        var values_b = new int[cellsInMask];
                        var iter = 0;

                        for (int x = -1 * maskRange; x < maskRange + 1; x++)
                        {
                            for (int y = -1 * maskRange; y < maskRange + 1; y++)
                            {
                                if (i + x >= 0 && i + x < bitmap.Width && j + y >= 0 && j + y < bitmap.Height)
                                {
                                    var pixel = bitmap.GetPixel(i + x, j + y);
                                    values_r[iter] = pixel.R;
                                    values_g[iter] = pixel.G;
                                    values_b[iter++] = pixel.B;
                                }
                            }
                        }

                        Array.Sort(values_r);
                        Array.Sort(values_g);
                        Array.Sort(values_b);

                        var median_r = values_r.Length % 2 != 0 ? values_r[values_r.Length / 2] : (int)((values_r[values_r.Length / 2] + values_r[(values_r.Length / 2) - 1]) / 2);
                        var median_g = values_g.Length % 2 != 0 ? values_g[values_g.Length / 2] : (int)((values_g[values_g.Length / 2] + values_g[(values_g.Length / 2) - 1]) / 2);
                        var median_b = values_b.Length % 2 != 0 ? values_b[values_b.Length / 2] : (int)((values_b[values_b.Length / 2] + values_b[(values_b.Length / 2) - 1]) / 2);

                        copy.SetPixel(i, j, System.Drawing.Color.FromArgb(median_r, median_g, median_b));
                    }
                }
            }

            Image.Source = ConvertBitmap(copy);
        }

        private void MaskButton_Click(object sender, RoutedEventArgs e)
        {
            if(customMask != null)
            {
                Image.Source = ConvOperation(customMask);
            }
            else
            {
                MessageBoxResult result = System.Windows.MessageBox.Show("Brak utworzonej maski",
                                          "Error",
                                          MessageBoxButton.OK,
                                          MessageBoxImage.Error);
                return;
            }
        }

        private void EqHistButton_Click(object sender, RoutedEventArgs e)
        {
            var val = CalcHistogram();
            var pixelAmount = 0;

            foreach (var v in val.Item1)
            {
                pixelAmount += v;
            }

            if (CheckIfGray())
            {
                var values = val.Item1;

                var distribution = CalcDistribution(values, pixelAmount);
                var LUT_table = CalcLUTTable(distribution);

                var bitmap = BitmapFromSource((BitmapSource)Image.Source);

                for (int i = 0; i < bitmap.Width; i++)
                {
                    for (int j = 0; j < bitmap.Height; j++)
                    {
                        var pixel = bitmap.GetPixel(i, j);
                        bitmap.SetPixel(i, j, System.Drawing.Color.FromArgb(LUT_table[pixel.R], LUT_table[pixel.R], LUT_table[pixel.R]));
                    }
                }

                Image.Source = ConvertBitmap(bitmap);

                var val4hist = CalcHistogram();

                var values4hist = val4hist.Item1;

                var histWindow = new HistogramWindow();

                WindowsFormsHost host = new WindowsFormsHost();
                HistSomething.UserControl1 graph = new HistSomething.UserControl1(values4hist);
                host.Child = graph;
                histWindow.grid.Children.Add(host);
                histWindow.Width = graph.Width + 2 * 20;
                histWindow.Height = graph.Height + 3 * 20;
                histWindow.Show();

            }
            else
            {
                var values_r = val.Item1;
                var values_g = val.Item2;
                var values_b = val.Item3;

                var distribution_r = CalcDistribution(values_r, pixelAmount);
                var distribution_g = CalcDistribution(values_g, pixelAmount);
                var distribution_b = CalcDistribution(values_b, pixelAmount);
                var LUT_table_r = CalcLUTTable(distribution_r);
                var LUT_table_g = CalcLUTTable(distribution_g);
                var LUT_table_b = CalcLUTTable(distribution_b);

                var bitmap = BitmapFromSource((BitmapSource)Image.Source);

                for (int i = 0; i < bitmap.Width; i++)
                {
                    for (int j = 0; j < bitmap.Height; j++)
                    {
                        var pixel = bitmap.GetPixel(i, j);
                        bitmap.SetPixel(i, j, System.Drawing.Color.FromArgb(LUT_table_r[pixel.R], LUT_table_g[pixel.G], LUT_table_b[pixel.B]));
                    }
                }

                Image.Source = ConvertBitmap(bitmap);

                var val4hist = CalcHistogram();

                var values_r4hist = val4hist.Item1;
                var values_g4hist = val4hist.Item2;
                var values_b4hist = val4hist.Item3;

                var histWindow = new HistogramWindow();

                WindowsFormsHost host = new WindowsFormsHost();
                HistSomething.UserControl3 graph = new HistSomething.UserControl3(values_r4hist, values_g4hist, values_b4hist);
                host.Child = graph;
                histWindow.grid.Children.Add(host);
                histWindow.Width = graph.Width + 2 * 30;
                histWindow.Height = graph.Height + 3 * 30;
                histWindow.Show();
            }

            
        }

        private void StrechHistButton_Click(object sender, RoutedEventArgs e)
        {
            var bitmap = BitmapFromSource((BitmapSource)Image.Source);

            if (CheckIfGray())
            {
                var values = new List<int>();

                for (int i = 0; i < bitmap.Width; i++)
                {
                    for (int j = 0; j < bitmap.Height; j++)
                    {
                        values.Add(bitmap.GetPixel(i, j).R);
                    }
                }

                var stretched = StretchFunction(values);

                var iter = 0;
                for (int i = 0; i < bitmap.Width; i++)
                {
                    for (int j = 0; j < bitmap.Height; j++)
                    {
                        bitmap.SetPixel(i, j, System.Drawing.Color.FromArgb(stretched[iter], stretched[iter], stretched[iter]));
                        iter++;
                    }
                }

                Image.Source = ConvertBitmap(bitmap);

                var val = CalcHistogram();

                var values4hist = val.Item1;

                values4hist[0] = 0;
                values4hist[255] = 0;

                var histWindow = new HistogramWindow();

                WindowsFormsHost host = new WindowsFormsHost();
                HistSomething.UserControl1 graph = new HistSomething.UserControl1(values4hist);
                host.Child = graph;
                histWindow.grid.Children.Add(host);
                histWindow.Width = graph.Width + 2 * 20;
                histWindow.Height = graph.Height + 3 * 20;
                histWindow.Show();


            }
            else
            {
                var values_r = new List<int>();
                var values_g = new List<int>();
                var values_b = new List<int>();

                for (int i = 0; i < bitmap.Width; i++)
                {
                    for (int j = 0; j < bitmap.Height; j++)
                    {
                        values_r.Add(bitmap.GetPixel(i, j).R);
                        values_g.Add(bitmap.GetPixel(i, j).G);
                        values_b.Add(bitmap.GetPixel(i, j).B);
                    }
                }

                var stretched_r = StretchFunction(values_r);
                var stretched_g = StretchFunction(values_g);
                var stretched_b = StretchFunction(values_b);

                var iter = 0;
                for (int i = 0; i < bitmap.Width; i++)
                {
                    for (int j = 0; j < bitmap.Height; j++)
                    {
                        bitmap.SetPixel(i, j, System.Drawing.Color.FromArgb(stretched_r[iter], stretched_g[iter], stretched_b[iter]));
                        iter++;
                    }
                }

                Image.Source = ConvertBitmap(bitmap);

                var val = CalcHistogram();

                var values_r4hist = val.Item1;
                var values_g4hist = val.Item2;
                var values_b4hist = val.Item3;

                values_r4hist[0] = 0;
                values_r4hist[255] = 0;
                values_g4hist[0] = 0;
                values_g4hist[255] = 0;
                values_b4hist[0] = 0;
                values_b4hist[255] = 0;

                var histWindow = new HistogramWindow();

                WindowsFormsHost host = new WindowsFormsHost();
                HistSomething.UserControl3 graph = new HistSomething.UserControl3(values_r4hist, values_g4hist, values_b4hist);
                host.Child = graph;
                histWindow.grid.Children.Add(host);
                histWindow.Width = graph.Width + 2 * 30;
                histWindow.Height = graph.Height + 3 * 30;
                histWindow.Show();
                
            }

            

        }

        private void HistButton_Click(object sender, RoutedEventArgs e)
        {
            var val = CalcHistogram();

            if (CheckIfGray())
            {
                var values = val.Item1;
                var histWindow = new HistogramWindow();

                WindowsFormsHost host = new WindowsFormsHost();
                HistSomething.UserControl1 graph = new HistSomething.UserControl1(values);
                host.Child = graph;
                histWindow.grid.Children.Add(host);
                histWindow.Width = graph.Width + 2 * 20;
                histWindow.Height = graph.Height + 3 * 20;
                histWindow.Show();
            }
            else
            {
                var values_r = val.Item1;
                var values_g = val.Item2;
                var values_b = val.Item3;

                var histWindow = new HistogramWindow();

                WindowsFormsHost host = new WindowsFormsHost();
                HistSomething.UserControl3 graph = new HistSomething.UserControl3(values_r, values_g, values_b);
                host.Child = graph;
                histWindow.grid.Children.Add(host);
                histWindow.Width = graph.Width + 2 * 30;
                histWindow.Height = graph.Height + 3 * 30;
                histWindow.Show();

            }
        }

        private void ManTresholdingButton_Click(object sender, RoutedEventArgs e)
        {
            var treshold = Convert.ToInt32(TresholdBox.Text);

            var bitmap = BitmapFromSource((BitmapSource)Image.Source);

            if (!CheckIfGray())
            {
                bitmap = ToGrayFunction(bitmap);   
            }

            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    var pixel = bitmap.GetPixel(i, j);
                    if(pixel.R < treshold)
                    {
                        bitmap.SetPixel(i, j, System.Drawing.Color.FromArgb(0, 0, 0));
                    }else if(pixel.R >= treshold)
                    {
                        bitmap.SetPixel(i, j, System.Drawing.Color.FromArgb(255, 255, 255));
                    }
                }
            }

            Image.Source = ConvertBitmap(bitmap);
        }

        public Bitmap ToGrayFunction(Bitmap bitmap)
        {
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    var pixel = bitmap.GetPixel(i, j);
                    bitmap.SetPixel(i, j, System.Drawing.Color.FromArgb(pixel.R, pixel.R, pixel.R));
                }
            }
            
            return bitmap;
        }

        private void PercentageButton_Click(object sender, RoutedEventArgs e)
        {
            var perc = Convert.ToDouble(PercentageBox.Text)/100.0;

            var bitmap = BitmapFromSource((BitmapSource)Image.Source);

            var maxPixels = bitmap.Width * bitmap.Height;

            var histogram = CalcHistogram().Item1;

            int suma = 0;
            int treshold = 0;
            for (int i = 0; i < 256; i++)
            {
                suma += histogram[i];

                if(suma >= maxPixels * perc)
                {
                    treshold = i;
                    break;
                }
            }


            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    var pixel = bitmap.GetPixel(i, j);
                    if (pixel.R < treshold)
                    {
                        bitmap.SetPixel(i, j, System.Drawing.Color.FromArgb(0, 0, 0));
                    }
                    else if (pixel.R >= treshold)
                    {
                        bitmap.SetPixel(i, j, System.Drawing.Color.FromArgb(255, 255, 255));
                    }
                }
            }

            Image.Source = ConvertBitmap(bitmap);
        }

        private void EntropyButton_Click(object sender, RoutedEventArgs e)
        {
            var bitmap = BitmapFromSource((BitmapSource)Image.Source);

            var histogram = CalcHistogram().Item1;

            var maxPixels = bitmap.Width * bitmap.Height;

            List<int> wyniki = new List<int>();

            wyniki.Add(127);

            int treshold = 0;
            int k = 1;

            while(true)
            {
                var t = wyniki[k - 1];

                int jeden = 0;
                int dwa = 0;

                for (int i = 0; i < t; i++)
                {
                    jeden += (i * histogram[i]);
                    dwa += histogram[i];
                }

                int lewo = (int)((double)jeden / (double)(2 * dwa));

                int trzy = 0;
                int cztery = 0;

                for (int i = t + 1; i < 256; i++)
                {
                    trzy += i * histogram[i];
                    cztery += histogram[i];
                }

                int prawo = (int)((double)trzy / (double)(2 * cztery));

                var wynik = lewo + prawo;

                wyniki.Add(wynik);
                
                
                k++;
                var TB = new List<int>();
                var TW = new List<int>();

                for (int i = 0; i < bitmap.Width; i++)
                {
                    for (int j = 0; j < bitmap.Height; j++)
                    {
                        var pixel = bitmap.GetPixel(i, j);
                        if(pixel.R < wynik)
                        {
                            TB.Add(pixel.R);
                        }else if(pixel.R >= wynik)
                        {
                            TW.Add(pixel.R);
                        }
                    }
                }

                int sumaTB = 0;
                int sumeTW = 0;

                for (int i = 0; i < TB.Count; i++)
                {
                    sumaTB += TB[i];
                }

                for (int i = 0; i < TW.Count; i++)
                {
                    sumeTW += TW[i];
                }

                var meanTB = (int)((double)sumaTB / (double)TB.Count);
                var meanTW = (int)((double)sumeTW / (double)TW.Count);



                if (wynik == t || meanTB == meanTW)
                {
                    treshold = wynik;
                    break;
                }
            }

            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    var pixel = bitmap.GetPixel(i, j);
                    if (pixel.R < treshold)
                    {
                        bitmap.SetPixel(i, j, System.Drawing.Color.FromArgb(0, 0, 0));
                    }
                    else if (pixel.R >= treshold)
                    {
                        bitmap.SetPixel(i, j, System.Drawing.Color.FromArgb(255, 255, 255));
                    }
                }
            }

            Image.Source = ConvertBitmap(bitmap);

        }
    }
}
