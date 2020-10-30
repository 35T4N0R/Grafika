using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Grafika
{
    /// <summary>
    /// Logika interakcji dla klasy ThirdPage.xaml
    /// </summary>
    public partial class ThirdPage : Page
    {

        double red = 0.0;
        double green = 0.0;
        double blue = 0.0;

        double cyan = 0.0;
        double magenta = 0.0;
        double yellow = 0.0;
        double blackKey = 0.0;

        bool rgbDragged = false;
        bool cmykDragged = false;

        public ThirdPage()
        {
            InitializeComponent();
        }
//        Black = minimum(1-Red,1-Green,1-Blue)
//Cyan = (1-Red-Black)/(1-Black)
//Magenta = (1-Green-Black)/(1-Black)
//Yellow = (1-Blue-Black)/(1-Black)

//Red = 1-minimum(1, Cyan*(1-Black)+Black)
//Green = 1-minimum(1, Magenta*(1-Black)+Black)
//Blue = 1-minimum(1, Yellow*(1-Black)+Black)




        private double CheckIfProperValue(double value)
        {
            if(value < 0 || double.IsNaN(value))
            {
                return 0;
            }

            return value;
        }

        private void FillRectangles()
        {

            RGBRec.Fill = new SolidColorBrush(Color.FromRgb((byte)(red * 255), (byte)(green * 255), (byte)(blue * 255)));
            CMYKRec.Fill = new SolidColorBrush(Color.FromRgb((byte)(red * 255), (byte)(green * 255), (byte)(blue * 255)));
        }


        private void RSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (rgbDragged)
            {
                red = RSlider.Value / 255;
                green = GSlider.Value / 255;
                blue = BSlider.Value / 255;

                blackKey = CheckIfProperValue(1 - Math.Max(Math.Max(red, green), blue));
                cyan = CheckIfProperValue((1 - red - blackKey) / (1 - blackKey));
                magenta = CheckIfProperValue((1 - green - blackKey) / (1 - blackKey));
                yellow = CheckIfProperValue((1 - blue - blackKey) / (1 - blackKey));

                KSlider.Value = (int)(blackKey * 100);
                CSlider.Value = (int)(cyan * 100);
                MSlider.Value = (int)(magenta * 100);
                YSlider.Value = (int)(yellow * 100);

                FillRectangles();
            }

        }

        private void GSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (rgbDragged)
            {
                red = RSlider.Value / 255;
                green = GSlider.Value / 255;
                blue = BSlider.Value / 255;

                blackKey = CheckIfProperValue(1 - Math.Max(Math.Max(red, green), blue));
                cyan = CheckIfProperValue((1 - red - blackKey) / (1 - blackKey));
                magenta = CheckIfProperValue((1 - green - blackKey) / (1 - blackKey));
                yellow = CheckIfProperValue((1 - blue - blackKey) / (1 - blackKey));

                KSlider.Value = (int)(blackKey * 100);
                CSlider.Value = (int)(cyan * 100);
                MSlider.Value = (int)(magenta * 100);
                YSlider.Value = (int)(yellow * 100);

                FillRectangles();
            }
        }

        private void BSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (rgbDragged)
            {
                red = RSlider.Value / 255;
                green = GSlider.Value / 255;
                blue = BSlider.Value / 255;

                blackKey = CheckIfProperValue(1 - Math.Max(Math.Max(red, green), blue));
                cyan = CheckIfProperValue((1 - red - blackKey) / (1 - blackKey));
                magenta = CheckIfProperValue((1 - green - blackKey) / (1 - blackKey));
                yellow = CheckIfProperValue((1 - blue - blackKey) / (1 - blackKey));

                KSlider.Value = (int)(blackKey * 100);
                CSlider.Value = (int)(cyan * 100);
                MSlider.Value = (int)(magenta * 100);
                YSlider.Value = (int)(yellow * 100);

                FillRectangles();
            }
        }
        
        private void CSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (cmykDragged)
            {
                cyan = CSlider.Value / 100;
                magenta = MSlider.Value / 100;
                yellow = YSlider.Value / 100;
                blackKey = KSlider.Value / 100;

                red = (1 - cyan) * (1 - blackKey);
                green = (1 - magenta) * (1 - blackKey);
                blue = (1 - yellow) * (1 - blackKey);

                RSlider.Value = (int)(red * 255);
                GSlider.Value = (int)(green * 255);
                BSlider.Value = (int)(blue * 255);

                FillRectangles();
            }
        }

        private void MSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (cmykDragged)
            {
                cyan = CSlider.Value / 100;
                magenta = MSlider.Value / 100;
                yellow = YSlider.Value / 100;
                blackKey = KSlider.Value / 100;

                red = (1 - cyan) * (1 - blackKey);
                green = (1 - magenta) * (1 - blackKey);
                blue = (1 - yellow) * (1 - blackKey);

                RSlider.Value = (int)(red * 255);
                GSlider.Value = (int)(green * 255);
                BSlider.Value = (int)(blue * 255);

                FillRectangles();
            }
        }

        private void YSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (cmykDragged)
            {
                cyan = CSlider.Value / 100;
                magenta = MSlider.Value / 100;
                yellow = YSlider.Value / 100;
                blackKey = KSlider.Value / 100;

                red = (1 - cyan) * (1 - blackKey);
                green = (1 - magenta) * (1 - blackKey);
                blue = (1 - yellow) * (1 - blackKey);

                RSlider.Value = (int)(red * 255);
                GSlider.Value = (int)(green * 255);
                BSlider.Value = (int)(blue * 255);

                FillRectangles();
            }
        }

        private void KSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (cmykDragged)
            {
                cyan = CSlider.Value / 100;
                magenta = MSlider.Value / 100;
                yellow = YSlider.Value / 100;
                blackKey = KSlider.Value / 100;

                red = (1 - cyan) * (1 - blackKey);
                green = (1 - magenta) * (1 - blackKey);
                blue = (1 - yellow) * (1 - blackKey);

                RSlider.Value = (int)(red * 255);
                GSlider.Value = (int)(green * 255);
                BSlider.Value = (int)(blue * 255);

                FillRectangles();
            }
        }

        private void RSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            rgbDragged = true;
        }

        private void RSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            rgbDragged = false;
        }

        private void CSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            cmykDragged = true;
        }

        private void CSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            cmykDragged = false;
        }

        private void MSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            cmykDragged = true;
        }

        private void MSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            cmykDragged = false;
        }

        private void YSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            cmykDragged = true;
        }

        private void YSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            cmykDragged = false;
        }

        private void KSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            cmykDragged = true;
        }

        private void KSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            cmykDragged = false;
        }

        private void GSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            rgbDragged = true;
        }

        private void GSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            rgbDragged = false;
        }

        private void BSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            rgbDragged = true;
        }

        private void BSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            rgbDragged = false;
        }


        //private void CalculateValues(int r, int g, int b, int c, int m, int y, int k)
        //{
        //    blackKey = new[] { 1 - red, 1 - green, 1 - blue }.Min();
        //    cyan = (1 - red - blackKey) / (1 - blackKey);
        //    magenta = (1 - green - blackKey) / (1 - blackKey);
        //    yellow = (1 - blue - blackKey) / (1 - blackKey);

        //    red = 1 - new[] { 1, cyan * (1 - blackKey) + blackKey }.Min();
        //    green = 1 - new[] { 1, magenta * (1 - blackKey) + blackKey }.Min();
        //    blue = 1 - new[] { 1, yellow * (1 - blackKey) + blackKey }.Min();
        //}
    }
}
