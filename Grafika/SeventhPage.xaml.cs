using System;
using System.Collections.Generic;
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
        MainWindow mw = (MainWindow)System.Windows.Application.Current.MainWindow;
        private ImageSource originalImage;
        public const int kernelSize = 3;

        public int[,] B1 = new int[,] { { 0, 0, 0 }, 
                                        { 0, 1, 0 }, 
                                        { 1, 1, 1 } }; //obraz

        public int[,] B2 = new int[,] { { 1, 1, 1 },
                                        { 0, 0, 0 },
                                        { 0, 0, 0 } }; //inwersja

        public int[,] A1 = new int[,] { { 0, 0, 0 },
                                        { 1, 1, 0 },
                                        { 0, 1, 0 } }; //obraz

        public int[,] A2 = new int[,] { { 0, 1, 1 },
                                        { 0, 0, 1 },
                                        { 0, 0, 0 } }; //inwersja

        public int[,] D1 = new int[,] { { 1, 1, 0 },
                                        { 1, 0, 0 },
                                        { 1, 0, 0 } };

        public int[,] D2 = new int[,] { { 0, 0, 0 },
                                        { 0, 1, 0 },
                                        { 0, 0, 1 } };

        public int[,] C1 = new int[,] { { 0, 1, 1 },
                                        { 0, 0, 1 },
                                        { 0, 0, 1 } };

        public int[,] C2 = new int[,] { { 0, 0, 0 },
                                        { 0, 1, 0 },
                                        { 1, 0, 0 } };

        public SeventhPage()
        {
            InitializeComponent();
        }

        public int[,] rotateMatrix(int N, int[,] mat)
        {

            for (int x = 0; x < N / 2; x++)
            {

                for (int y = x; y < N - x - 1; y++)
                {

                    int temp = mat[x, y];

                    mat[x, y] = mat[y, N - 1 - x];

                    mat[y, N - 1 - x] = mat[N - 1 - x, N - 1 - y];

                    mat[N - 1 - x, N - 1 - y] = mat[N - 1 - y, x];

                    mat[N - 1 - y, x] = temp;
                }
            }

            return mat;
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

        public int checkIndex(int index)
        {
            return index < 0 ? 0 : index;
        }

        public Bitmap Dilatation(Bitmap img)
        {

            var mask = new int[,] { { 1, 1, 1 },
                                    { 1, 1, 1 },
                                    { 1, 1, 1 } };

            //var mask = new int[,] { { 0,0,1,0,0 }, 
            //                        { 0,1,1,1,0 }, 
            //                        { 1,1,1,1,1 },
            //                        { 0,1,1,1,0},
            //                        { 0,0,1,0,0} };
            

            int width = img.Width;
            int height = img.Height;

            var srcData = img.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            int stride = srcData.Stride;

            var pixelBuffer = new byte[srcData.Stride * srcData.Height];
            var resultBuffer = new byte[srcData.Stride * srcData.Height];

            Marshal.Copy(srcData.Scan0, pixelBuffer, 0, pixelBuffer.Length);

            img.UnlockBits(srcData);


            var offset = (kernelSize - 1) / 2;
            var calcOffset = 0;
            var byteOffset = 0;


            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
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
                                if (calcOffset < 0 || calcOffset >= srcData.Stride * srcData.Height) continue;

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

        public Bitmap Erosion(Bitmap img, int[,] mask)
        {

            

            var srcData = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            var pixelBuffer = new byte[srcData.Stride * srcData.Height];
            var resultBuffer = new byte[srcData.Stride * srcData.Height];

            Marshal.Copy(srcData.Scan0, pixelBuffer, 0, pixelBuffer.Length);

            img.UnlockBits(srcData);

            var offset = (kernelSize - 1) / 2;
            var calcOffset = 0;
            var byteOffset = 0;


            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
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
                                if (calcOffset < 0 || calcOffset >= srcData.Stride * srcData.Height) continue;

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
            var mask = new int[,] { { 1, 1, 1 },
                                    { 1, 1, 1 },
                                    { 1, 1, 1 } };

            var resultErosion = Erosion(img, mask);

            var result = Dilatation(resultErosion);

            return result;
        }

        public Bitmap Close(Bitmap img)
        {
            var mask = new int[,] { { 1, 1, 1 },
                                    { 1, 1, 1 },
                                    { 1, 1, 1 } };

            var resultDilatation = Dilatation(img);

            var result = Erosion(resultDilatation, mask);

            return result;
        }

        public bool CheckIfGray(Bitmap bitmap)
        {

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

        public (int[], int[], int[]) CalcHistogram(Bitmap bitmap)
        {

            if (CheckIfGray(bitmap))
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

        public static BitmapSource Invert(BitmapSource source)
        {
            int stride = (source.PixelWidth * source.Format.BitsPerPixel + 7) / 8;

            int length = stride * source.PixelHeight;
            byte[] data = new byte[length];

            source.CopyPixels(data, stride, 0);

            for (int i = 0; i < length; i += 4)
            {
                data[i] = (byte)(255 - data[i]); //R
                data[i + 1] = (byte)(255 - data[i + 1]); //G
                data[i + 2] = (byte)(255 - data[i + 2]); //B
                                                         //data[i + 3] = (byte)(255 - data[i + 3]); //A
            }

            return BitmapSource.Create(source.PixelWidth, source.PixelHeight, source.DpiX, source.DpiY, source.Format, null, data, stride);
        }

        public BitmapSource thresh(Bitmap bitmap)
        {

            var histogram = CalcHistogram(bitmap).Item1;

            var maxPixels = bitmap.Width * bitmap.Height;

            List<int> wyniki = new List<int>();

            wyniki.Add(127);

            int treshold = 0;
            int k = 1;

            while (true)
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
                        if (pixel.R < wynik)
                        {
                            TB.Add(pixel.R);
                        }
                        else if (pixel.R >= wynik)
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

            return ConvertBitmap(bitmap);
        }

        public Bitmap HitOrMiss(Bitmap img, int[,] mask1, int[,] mask2)
        {
            var invertImage = BitmapFromSource(Invert(ConvertBitmap(img)));

            var orgi = img;

            var orgiErosion = Erosion(orgi, mask1);

            var invertImageErosion = Erosion(invertImage, mask2);


            var bitmap = new Bitmap(img.Width, img.Height);

            for (int i = 0; i < orgiErosion.Width; i++)
            {
                for (int j = 0; j < orgiErosion.Height; j++)
                {
                    var pixelO = orgiErosion.GetPixel(i, j).R;
                    var pixelI = invertImageErosion.GetPixel(i, j).R;

                    var value = pixelO == pixelI ? pixelO : 0;

                    bitmap.SetPixel(i, j, System.Drawing.Color.FromArgb(value, value, value));
                }
            }

            return bitmap;
        }

        public void HitOrMissThinning()
        {
            var mask1 = B1;
            var mask2 = B2;
            var mask3 = A1;
            var mask4 = A2;
            Bitmap bitmap2 = null;
            Bitmap bitmap4 = null;

            var img = BitmapFromSource(thresh(BitmapFromSource((BitmapSource)Image.Source)));

            for (int z = 0; z < 4; z++)
            {

                var bitmap = HitOrMiss(img, mask1, mask2);

                bitmap2 = new Bitmap(img.Width, img.Height);

                for (int i = 0; i < img.Width; i++)
                {
                    for (int j = 0; j < img.Height; j++)
                    {
                        var pixelO = img.GetPixel(i, j).R;
                        var pixelI = bitmap.GetPixel(i, j).R;

                        var value = (pixelO - pixelI) < 0 ? 0 : (pixelO - pixelI);

                        bitmap2.SetPixel(i, j, System.Drawing.Color.FromArgb(value, value, value));
                    }
                }

                img = bitmap2;

                var bitmap3 = HitOrMiss(img, mask3, mask4);

                bitmap4 = new Bitmap(img.Width, img.Height);

                for (int i = 0; i < img.Width; i++)
                {
                    for (int j = 0; j < img.Height; j++)
                    {
                        var pixelO = img.GetPixel(i, j).R;
                        var pixelI = bitmap3.GetPixel(i, j).R;

                        var value = (pixelO - pixelI) < 0 ? 0 : (pixelO - pixelI);

                        bitmap4.SetPixel(i, j, System.Drawing.Color.FromArgb(value, value, value));
                    }
                }

                img = bitmap4;

                mask1 = rotateMatrix(3, mask1);
                mask2 = rotateMatrix(3, mask2);
                mask3 = rotateMatrix(3, mask3);
                mask4 = rotateMatrix(3, mask4);
            }

            Image.Source = ConvertBitmap(bitmap4);

        }

        public void HitOrMissFatting()
        {
            var mask1 = D1;
            var mask2 = D2;
            var mask3 = C1;
            var mask4 = C2;
            Bitmap bitmap2 = null;

            var img = BitmapFromSource(thresh(BitmapFromSource((BitmapSource)Image.Source)));

            for (int z = 0; z < 4; z++)
            {

                var bitmap = HitOrMiss(img, mask1, mask2);

                bitmap2 = new Bitmap(img.Width, img.Height);

                for (int i = 0; i < img.Width; i++)
                {
                    for (int j = 0; j < img.Height; j++)
                    {
                        var pixelO = img.GetPixel(i, j).R;
                        var pixelI = bitmap.GetPixel(i, j).R;

                        var value = (pixelO + pixelI) > 255 ? 255 : (pixelO + pixelI);

                        bitmap2.SetPixel(i, j, System.Drawing.Color.FromArgb(value, value, value));
                    }
                }

                img = bitmap2;

                var bitmap3 = HitOrMiss(img, mask3, mask4);

                bitmap2 = new Bitmap(img.Width, img.Height);

                for (int i = 0; i < img.Width; i++)
                {
                    for (int j = 0; j < img.Height; j++)
                    {
                        var pixelO = img.GetPixel(i, j).R;
                        var pixelI = bitmap3.GetPixel(i, j).R;

                        var value = (pixelO + pixelI) > 255 ? 255 : (pixelO + pixelI);

                        bitmap2.SetPixel(i, j, System.Drawing.Color.FromArgb(value, value, value));
                    }
                }

                img = bitmap2;

                mask1 = rotateMatrix(3, mask1);
                mask2 = rotateMatrix(3, mask2);
                mask3 = rotateMatrix(3, mask3);
                mask4 = rotateMatrix(3, mask4);
            }

            Image.Source = ConvertBitmap(bitmap2);
            
        }



        private void DilatationButton_Click(object sender, RoutedEventArgs e)
        {
            var img = BitmapFromSource((BitmapSource)Image.Source);

            Image.Source = ConvertBitmap(Dilatation(img));
        }

        private void ErosionButton_Click(object sender, RoutedEventArgs e)
        {
            var img = BitmapFromSource((BitmapSource)Image.Source);

            var mask = new int[,] { { 1, 1, 1 },
                                    { 1, 1, 1 },
                                    { 1, 1, 1 } };
            //var mask = new int[,] { { 0,0,1,0,0 },
            //                        { 0,1,1,1,0 },
            //                        { 1,1,1,1,1 },
            //                        { 0,1,1,1,0 },
            //                        { 0,0,1,0,0 } };

            Image.Source = ConvertBitmap(Erosion(img, mask));
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

        private void HitOrMiss1Button_Click(object sender, RoutedEventArgs e)
        {
            HitOrMissThinning();
        }

        private void HitOrMiss2Button_Click(object sender, RoutedEventArgs e)
        {
            HitOrMissFatting();
        }

        public (double,Bitmap) CalculatePercentage(Bitmap img, char color)
        {
            var totalPixels = img.Width * img.Height;
            var pixelCounter = 0;

            var resultImg = new Bitmap(img.Width, img.Height);

            for (int i = 0; i < img.Width; i++)
            {
                for (int j = 0; j < img.Height; j++)
                {
                    var pixel = img.GetPixel(i, j);
                    switch (color)
                    {
                        case 'r':
                            if(pixel.R > pixel.G && pixel.R > pixel.B)
                            {
                                pixelCounter++;
                                resultImg.SetPixel(i, j, System.Drawing.Color.FromArgb(0, 0, 0));
                            }
                            else
                            {
                                resultImg.SetPixel(i, j, System.Drawing.Color.FromArgb(pixel.R, pixel.G, pixel.B));
                            }
                            break;
                        case 'g':
                            if (pixel.G > pixel.R && pixel.G > pixel.B)
                            {
                                pixelCounter++;
                                resultImg.SetPixel(i, j, System.Drawing.Color.FromArgb(0, 0, 0));
                            }
                            else
                            {
                                resultImg.SetPixel(i, j, System.Drawing.Color.FromArgb(pixel.R, pixel.G, pixel.B));
                            }
                            break;
                        case 'b':
                            if (pixel.B > pixel.R && pixel.B > pixel.G)
                            {
                                pixelCounter++;
                                resultImg.SetPixel(i, j, System.Drawing.Color.FromArgb(0, 0, 0));
                            }
                            else
                            {
                                resultImg.SetPixel(i, j, System.Drawing.Color.FromArgb(pixel.R, pixel.G, pixel.B));
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            var percentage = ((double)pixelCounter / (double)totalPixels) * 100;

            return (percentage,resultImg);
        }

        private void ImgAnalizeButton_Click(object sender, RoutedEventArgs e)
        {
            var img = BitmapFromSource((BitmapSource)originalImage);
            var (percentage, bitmap) = CalculatePercentage(img, Convert.ToChar(PercentInput.Text));

            Image.Source = ConvertBitmap(bitmap);
            MessageBoxResult result = System.Windows.MessageBox.Show($"Procent: {percentage}%",
                                          "Result",
                                          MessageBoxButton.OK,
                                          MessageBoxImage.None);

        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            mw.MainFrame.Content = new SixthPage();
        }
    }
}
