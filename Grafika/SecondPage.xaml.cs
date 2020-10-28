using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
        public SecondPage()
        {
            InitializeComponent();
        }

        private void LoadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string ext = Path.GetExtension(ofd.FileName);

                if (ext.Equals(".ppm")) {
                    BitmapSource bitmap = CreateBitmapSourceFromGdiBitmap(new PNMReader().ReadImage(ofd.FileName));
                    Image.Source = bitmap;
                }
                else if(ext.Equals(".jpeg") || ext.Equals(".jpg") || ext.Equals(".png"))
                {
                    ImageSource imageSource = new BitmapImage(new Uri(ofd.FileName));
                    Image.Source = imageSource;
                }
                else
                {
                    MessageBoxResult result = System.Windows.MessageBox.Show("Obsługiwanymi formatami plików graficznych są: .ppm, .jpeg, .jpg, .png",
                                             "Error",
                                             MessageBoxButton.OK,
                                             MessageBoxImage.Error);
                    return;

                }
            }
        }

        private void SaveImage_Click(object sender, RoutedEventArgs e)
        {
            int parsedValue;
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "JPEG (*.jpeg)|*.jpeg|PNG (*.png)|*.png|All Files (*.*)|*.*";
            sfd.DefaultExt = ".jpeg";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.QualityLevel = 100;

                if (Path.GetExtension(sfd.FileName).Equals(".jpeg"))
                {
                    if(String.IsNullOrEmpty(((System.Windows.Controls.TextBox)inputs.FindName("compresionLevelInput")).Text))
                    {
                        MessageBoxResult result = System.Windows.MessageBox.Show("Jeżeli chcesz zapisać jako .jpeg nie możesz zostawić stopnia kompresji pustego",
                                          "Error",
                                          MessageBoxButton.OK,
                                          MessageBoxImage.Error);
                        return;
                    }
                    else if (!int.TryParse(compresionLevelInput.Text, out parsedValue) || !(Convert.ToInt32(compresionLevelInput.Text) > 0 && Convert.ToInt32(compresionLevelInput.Text) <= 100))
                    {
                        MessageBoxResult result = System.Windows.MessageBox.Show("Stopień kompresji jest wartością całkowitą od 1 do 100",
                                          "Error",
                                          MessageBoxButton.OK,
                                          MessageBoxImage.Error);
                        return;
                    }
                    else
                    {
                        var compression = Convert.ToInt32(((System.Windows.Controls.TextBox)inputs.FindName("compresionLevelInput")).Text);
                        encoder.QualityLevel = compression;
                    }
                }

                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)Image.Source));
                using (FileStream stream = new FileStream(sfd.FileName, FileMode.Create))
                    encoder.Save(stream);
            }
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
            int width = GetNextTextValue(reader);
            int height = GetNextTextValue(reader);
            int scale = GetNextTextValue(reader);

            Bitmap bitmap = new Bitmap(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int r = GetNextTextValue(reader) * 255 / scale;
                    int g = GetNextTextValue(reader) * 255 / scale;
                    int b = GetNextTextValue(reader) * 255 / scale;

                    bitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(255, r, g, b));
                }
            }

            return bitmap;
        }

        private Bitmap ReadBinaryPixelImage(BinaryReader reader)
        {
            int width = GetNextTextValue(reader);
            int height = GetNextTextValue(reader);
            int scale = GetNextTextValue(reader);

            Bitmap bitmap = new Bitmap(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int r = reader.ReadByte() * 255 / scale;
                    int g = reader.ReadByte() * 255 / scale;
                    int b = reader.ReadByte() * 255 / scale;

                    bitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(r, g, b));
                }
            }

            return bitmap;
        }


        private int GetNextTextValue(BinaryReader reader)
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
    }
}
