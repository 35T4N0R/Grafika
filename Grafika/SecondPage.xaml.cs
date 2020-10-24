using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Grafika
{
    /// <summary>
    /// Logika interakcji dla klasy SecondPage.xaml
    /// </summary>
    public partial class SecondPage : Page
    {
        string currentImageExtension = string.Empty;
        public SecondPage()
        {
            InitializeComponent();
        }

        private void LoadImage_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();

            //if (ofd.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(ofd.FileName))
            //{
            //    Image.Source = CreateBitmapSourceFromGdiBitmap(new PNMReader().ReadImage(ofd.FileName));
            //}
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string ext = Path.GetExtension(ofd.FileName);

                if (ext.Equals(".ppm")) { 
                    BitmapSource bitmap = CreateBitmapSourceFromGdiBitmap(new PNMReader().ReadImage(ofd.FileName));
                    Image.Source = bitmap;
                }
                else
                {
                    ImageSource imageSource = new BitmapImage(new Uri(ofd.FileName));
                    Image.Source = imageSource;
                }
                currentImageExtension = ext;
            }
        }

        private void SaveImage_Click(object sender, RoutedEventArgs e)
        {

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "PNG (*.png)|*.png|JPEG (*.jpeg)|*.jpeg|All Files (*.*)|*.*";
            sfd.DefaultExt = ".png";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                if (currentImageExtension.Equals(".ppm"))
                {
                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create((BitmapSource)Image.Source));
                    using (FileStream stream = new FileStream(sfd.FileName, FileMode.Create))
                        encoder.Save(stream);
                }else
                {
                    var compression = Convert.ToInt32(((System.Windows.Controls.TextBox)inputs.FindName("compresionLevelInput")).Text);

                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();

                    encoder.QualityLevel = compression;
                    encoder.Frames.Add(BitmapFrame.Create((BitmapSource)Image.Source));
                    using(FileStream stream = new FileStream(sfd.FileName, FileMode.Create))
                        encoder.Save(stream);
                }
            }
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            var codecs = ImageCodecInfo.GetImageDecoders();
            foreach (var codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        // Return an ImageCodecInfo object for this mime type.
        private ImageCodecInfo GetEncoderInfo(string mime_type)
        {
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            for (int i = 0; i <= encoders.Length; i++)
            {
                if (encoders[i].MimeType == mime_type) return encoders[i];
            }
            return null;
        }


        public static BitmapSource CreateBitmapSourceFromGdiBitmap(Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            var bitmapData = bitmap.LockBits(
                rect,
                ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            try
            {
                var size = (rect.Width * rect.Height) * 4;

                return BitmapSource.Create(
                    bitmap.Width,
                    bitmap.Height,
                    bitmap.HorizontalResolution,
                    bitmap.VerticalResolution,
                    PixelFormats.Bgra32,
                    null,
                    bitmapData.Scan0,
                    size,
                    bitmapData.Stride);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
        }



    }

    class PNMReader
    {
        public Bitmap ReadImage(string path)
        {

            using (BinaryReader reader = new BinaryReader(new FileStream(path, FileMode.Open)))
            {
                if (reader.ReadChar() == 'P')
                {
                    char c = reader.ReadChar();
                    if (c == '3')
                    {
                        return ReadTextPixelImage(reader);
                    }
                    else if (c == '6')
                    {
                        return ReadBinaryPixelImage(reader);
                    }
                }
            }

            return null;

        }

        private Bitmap ReadTextPixelImage(BinaryReader reader)
        {
            char c;

            int width = GetNextHeaderValue(reader);
            int height = GetNextHeaderValue(reader);
            int scale = GetNextHeaderValue(reader);

            Bitmap bitmap = new Bitmap(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int red = GetNextTextValue(reader) * 255 / scale;
                    int green = GetNextTextValue(reader) * 255 / scale;
                    int blue = GetNextTextValue(reader) * 255 / scale;

                    bitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(red, green, blue));
                }
            }

            return bitmap;
        }

        private Bitmap ReadBinaryPixelImage(BinaryReader reader)
        {
            int width = GetNextHeaderValue(reader);
            int height = GetNextHeaderValue(reader);
            int scale = GetNextHeaderValue(reader);

            Bitmap bitmap = new Bitmap(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int red = reader.ReadByte() * 255 / scale;
                    int green = reader.ReadByte() * 255 / scale;
                    int blue = reader.ReadByte() * 255 / scale;

                    bitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(red, green, blue));
                }
            }

            return bitmap;
        }
        private int GetNextHeaderValue(BinaryReader reader)
        {
            bool hasValue = false;
            string value = string.Empty;
            char c;
            bool comment = false;

            do
            {
                c = reader.ReadChar();
                
                if (c == '#')
                {
                    comment = true;
                }

                if (comment)
                {
                    if (c == '\n')
                    {
                        comment = false;
                    }

                    continue;
                }

                if (!hasValue)
                {
                    if ((c == '\n' || c == ' ' || c == '\t') && value.Length != 0)
                    {
                        hasValue = true;
                    }
                    else if (c >= '0' && c <= '9')
                    {
                        value += c;
                    }
                }

            } while (!hasValue);

            return int.Parse(value);
        }

        private int GetNextTextValue(BinaryReader reader)
        {
            //string value = string.Empty;
            //char c = reader.ReadChar();

            //do
            //{
            //    value += c;

            //    c = reader.ReadChar();

            //} while (!(c == '\n' || c == ' ' || c == '\t') || value.Length == 0);

            //return int.Parse(value);
            bool hasValue = false;
            string value = string.Empty;
            char c;
            bool comment = false;

            do
            {
                c = reader.ReadChar();

                if (c == '#')
                {
                    comment = true;
                }

                if (comment)
                {
                    if (c == '\n')
                    {
                        comment = false;
                    }

                    continue;
                }

                if (!hasValue)
                {
                    if ((c == '\n' || c == ' ' || c == '\t') && value.Length != 0)
                    {
                        hasValue = true;
                    }
                    else if (c >= '0' && c <= '9')
                    {
                        value += c;
                    }
                }

            } while (!hasValue);

            return int.Parse(value);
        }
    }

    public static class BinaryReaderExtension
    {

        public static String ReadLine(this BinaryReader reader)
        {
            var result = new StringBuilder();
            bool foundEndOfLine = false;
            char ch;
            while (!foundEndOfLine)
            {
                try
                {
                    ch = reader.ReadChar();
                }
                catch (EndOfStreamException ex)
                {
                    if (result.Length == 0) return null;
                    else break;
                }

                switch (ch)
                {
                    case '\r':
                        if (reader.PeekChar() == '\n') reader.ReadChar();
                        foundEndOfLine = true;
                        break;
                    case '\n':
                        foundEndOfLine = true;
                        break;
                    default:
                        result.Append(ch);
                        break;
                }
            }
            return result.ToString();
        }
    }
}
