using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Grafika
{
    /// <summary>
    /// Logika interakcji dla klasy SeventhPage.xaml
    /// </summary>
    public partial class SeventhPage : Page
    {
        private ImageSource originalImage;
        public const int kernelSize = 3;

        public SeventhPage()
        {
            InitializeComponent();
        }

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
            //SaveFileDialog sfd = new SaveFileDialog();
            //sfd.Filter = "JPEG (*.jpeg)|*.jpeg|PNG (*.png)|*.png|All Files (*.*)|*.*";
            //sfd.DefaultExt = ".jpeg";

            //if (sfd.ShowDialog() == DialogResult.OK)
            //{
            //    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            //    encoder.QualityLevel = 100;
            //    encoder.Frames.Add(BitmapFrame.Create((BitmapSource)Image.Source));
            //    using (FileStream stream = new FileStream(sfd.FileName, FileMode.Create))
            //        encoder.Save(stream);
            //}
        }

        public int checkIndex(int index)
        {
            return index < 0 ? 0 : index;
        }

        public Bitmap Dilatation(Bitmap img)
        {

            var mask = new int[,] { { 0, 1, 0 }, 
                                    { 1, 1, 1 }, 
                                    { 0, 1, 0 } };

            int width = img.Width;
            int height = img.Height;

            var srcData = img.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            int stride = srcData.Stride;

            var pixelBuffer = new byte[srcData.Stride * srcData.Height];
            var resultBuffer = new byte[srcData.Stride * srcData.Height];

            Marshal.Copy(srcData.Scan0, pixelBuffer, 0, pixelBuffer.Length);

            img.UnlockBits(srcData);

            ////to grauscale
            //float rgb = 0;
            //for (int i = 0; i < pixelBuffer.Length; i += 4)
            //{
            //    rgb = pixelBuffer[i] * .071f;
            //    rgb += pixelBuffer[i + 1] * .71f;
            //    rgb += pixelBuffer[i + 2] * .21f;
            //    pixelBuffer[i] = (byte)rgb;
            //    pixelBuffer[i + 1] = pixelBuffer[i];
            //    pixelBuffer[i + 2] = pixelBuffer[i];
            //    pixelBuffer[i + 3] = 255;
            //}


            var offset = (kernelSize - 1) / 2;
            var calcOffset = 0;
            var byteOffset = 0;


            for (int y = offset; y < img.Height - offset; y++)
            {
                for (int x = offset; x < img.Width - offset; x++)
                {
                    byte value_r = 0;
                    byte value_g = 0;
                    byte value_b = 0;

                    byteOffset = (y * stride) + (x * 4);


                    for (int j = -offset; j <= offset; j++)
                    {
                        for (int i = -offset; i <= offset; i++)
                        {

                            if(mask[j + offset, i + offset] == 1)
                            {
                                calcOffset = byteOffset + (i * 4) + (j * srcData.Stride);

                                value_b = Math.Max(value_b, pixelBuffer[calcOffset]);
                                value_g = Math.Max(value_g, pixelBuffer[calcOffset + 1]);
                                value_r = Math.Max(value_r, pixelBuffer[calcOffset + 2]);
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }

                    resultBuffer[byteOffset] = value_b;
                    resultBuffer[byteOffset + 1] = value_g;
                    resultBuffer[byteOffset + 2] = value_r;
                    resultBuffer[byteOffset + 3] = 255;

                }
            }

            var resultBitmap = new Bitmap(img.Width, img.Height);

            BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0, resultBitmap.Width, resultBitmap.Height), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);

            resultBitmap.UnlockBits(resultData);

            return resultBitmap;

        }

        public Bitmap Erosion(Bitmap img)
        {

            var mask = new int[,] { { 0, 1, 0 },
                                    { 1, 1, 1 },
                                    { 0, 1, 0 } };

            var srcData = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            var pixelBuffer = new byte[srcData.Stride * srcData.Height];
            var resultBuffer = new byte[srcData.Stride * srcData.Height];

            Marshal.Copy(srcData.Scan0, pixelBuffer, 0, pixelBuffer.Length);

            img.UnlockBits(srcData);

            var offset = (kernelSize - 1) / 2;
            var calcOffset = 0;
            var byteOffset = 0;


            for (int y = offset; y < img.Height - offset; y++)
            {
                for (int x = offset; x < img.Width - offset; x++)
                {
                    byteOffset = (y * srcData.Stride) + (x * 4);

                    byte value_r = 255;
                    byte value_g = 255;
                    byte value_b = 255;

                    for (int j = -offset; j <= offset; j++)
                    {
                        for (int i = -offset; i <= offset; i++)
                        {

                            if (mask[j + offset, i + offset] == 1)
                            {
                                calcOffset = byteOffset + (i * 4) + (j * srcData.Stride);

                                value_b = Math.Min(value_b, pixelBuffer[calcOffset]);
                                value_g = Math.Min(value_g, pixelBuffer[calcOffset + 1]);
                                value_r = Math.Min(value_r, pixelBuffer[calcOffset + 2]);
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }

                    resultBuffer[byteOffset] = value_b;
                    resultBuffer[byteOffset + 1] = value_g;
                    resultBuffer[byteOffset + 2] = value_r;
                    resultBuffer[byteOffset + 3] = 255;

                }
            }

            var resultBitmap = new Bitmap(img.Width, img.Height);

            BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0, resultBitmap.Width, resultBitmap.Height), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);

            resultBitmap.UnlockBits(resultData);

            return resultBitmap;
        }

        public Bitmap Open(Bitmap img)
        {
            var resultErosion = Erosion(img);

            var result = Dilatation(resultErosion);

            return result;
        }

        public Bitmap Close(Bitmap img)
        {
            var resultDilatation = Dilatation(img);

            var result = Erosion(resultDilatation);

            return result;
        }

        public void HitOrMissThinning()
        {

        }



        private void DilatationButton_Click(object sender, RoutedEventArgs e)
        {
            var img = BitmapFromSource((BitmapSource)Image.Source);

            Image.Source = ConvertBitmap(Dilatation(img));
        }

        private void ErosionButton_Click(object sender, RoutedEventArgs e)
        {
            var img = BitmapFromSource((BitmapSource)Image.Source);

            Image.Source = ConvertBitmap(Erosion(img));
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            Image.Source = originalImage;
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            var img = BitmapFromSource((BitmapSource)Image.Source);

            Image.Source = ConvertBitmap(Open(img));
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            var img = BitmapFromSource((BitmapSource)Image.Source);

            Image.Source = ConvertBitmap(Close(img));
        }
    }
}
