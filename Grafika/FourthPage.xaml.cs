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

        private void LoadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                ImageSource imageSource = new BitmapImage(new Uri(ofd.FileName));
                Image.Source = imageSource;
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
                value = Convert.ToInt32(ValueBox.Text) >= 0 ? Convert.ToInt32(ValueBox.Text): 0;

                for (int i = 0; i < bitmap.Width; i++)
                {
                    for (int j = 0; j < bitmap.Height; j++)
                    {
                        var pixel = bitmap.GetPixel(i, j);
                        var r = pixel.R + value >= 255 ? 255 : pixel.R + value;
                        var g = pixel.G + value >= 255 ? 255 : pixel.G + value;
                        var b = pixel.B + value >= 255 ? 255 : pixel.B + value;
                        bitmap.SetPixel(i, j, System.Drawing.Color.FromArgb(r, g, b));
                    }
                }

                Image.Source = ConvertBitmap(bitmap);
            }
        }
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
        #endregion

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
                        var r = pixel.R - value < 0 ? 0 : pixel.R - value;
                        var g = pixel.G - value < 0 ? 0 : pixel.G - value;
                        var b = pixel.B - value < 0 ? 0 : pixel.B - value;
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
                        var r = pixel.R * value >= 255 ? 255 : pixel.R * value;
                        var g = pixel.G * value >= 255 ? 255 : pixel.G * value;
                        var b = pixel.B * value >= 255 ? 255 : pixel.B * value;
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
            var bitmap = BitmapFromSource((BitmapSource)Image.Source);
            int value = 0;

            if (!String.IsNullOrEmpty(ValueBox.Text))
            {
                value = Convert.ToInt32(ValueBox.Text) >= 0 ? Convert.ToInt32(ValueBox.Text) % 256 : 0;

                for (int i = 0; i < bitmap.Width; i++)
                {
                    for (int j = 0; j < bitmap.Height; j++)
                    {
                        var pixel = bitmap.GetPixel(i, j);
                        bitmap.SetPixel(i, j, System.Drawing.Color.FromArgb(value, pixel.R, pixel.G, pixel.B));

                    }
                }

                Image.Source = ConvertBitmap(bitmap);
            }
        }

        private void MaskWindowButton_Click(object sender, RoutedEventArgs e)
        {
            var maskWindow = new MaskWindow();
            maskWindow.ShowDialog();
        }
    }
}
