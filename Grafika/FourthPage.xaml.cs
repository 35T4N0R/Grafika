using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
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

        public bool CheckIfGray()
        {
            var bitmap = BitmapFromSource((BitmapSource)originalImage);

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
            int[,] mask = new int[,]{{1, 4, 1 },
                                     {4, 32, 4 },
                                     {1, 4, 1 } };

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
    }
}
