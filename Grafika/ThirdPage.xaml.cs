using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

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
        bool rgbTyping = false;
        bool cmykTyping = false;

        public ThirdPage()
        {
            InitializeComponent();

            CalculateCMYK();
            FillRectangles();
        }

        //formulas
        //Black = minimum(1-Red,1-Green,1-Blue)
        //Cyan = (1-Red-Black)/(1-Black)
        //Magenta = (1-Green-Black)/(1-Black)
        //Yellow = (1-Blue-Black)/(1-Black)

        //Red = 1-minimum(1, Cyan*(1-Black)+Black)
        //Green = 1-minimum(1, Magenta*(1-Black)+Black)
        //Blue = 1-minimum(1, Yellow*(1-Black)+Black)


        #region helper functions
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
            if(RGBRec == null/* || CMYKRec == null*/)
            {
                return;
            }
            brush.Color = Color.FromRgb((byte)(red * 255), (byte)(green * 255), (byte)(blue * 255));
            RGBRec.Fill = new SolidColorBrush(Color.FromRgb((byte)(red * 255), (byte)(green * 255), (byte)(blue * 255)));
            //CMYKRec.Fill = new SolidColorBrush(Color.FromRgb((byte)(red * 255), (byte)(green * 255), (byte)(blue * 255)));
        }

        private void CalculateCMYK()
        {
            red = RSlider != null ? RSlider.Value / 255 : 0;
            green = GSlider != null ? GSlider.Value / 255 : 0;
            blue = BSlider != null ? BSlider.Value / 255 : 0;

            blackKey = CheckIfProperValue(Math.Min(Math.Min(1 - red, 1 - green), 1 - blue));
            cyan = CheckIfProperValue((1 - red - blackKey) / (1 - blackKey));
            magenta = CheckIfProperValue((1 - green - blackKey) / (1 - blackKey));
            yellow = CheckIfProperValue((1 - blue - blackKey) / (1 - blackKey));

            KSlider.Value = Math.Round(blackKey * 100, MidpointRounding.AwayFromZero);
            CSlider.Value = Math.Round(cyan * 100, MidpointRounding.AwayFromZero);
            MSlider.Value = Math.Round(magenta * 100, MidpointRounding.AwayFromZero);
            YSlider.Value = Math.Round(yellow * 100, MidpointRounding.AwayFromZero);
        }

        private void CalculateRGB()
        {
            cyan = CSlider != null ? CSlider.Value / 100 : 0;
            magenta = MSlider != null ? MSlider.Value / 100 : 0;
            yellow = YSlider != null ? YSlider.Value / 100 : 0;
            blackKey = KSlider != null ? KSlider.Value / 100 : 0;

            red = CheckIfProperValue((1 - cyan) * (1 - blackKey));
            green = CheckIfProperValue((1 - magenta) * (1 - blackKey));
            blue = CheckIfProperValue((1 - yellow) * (1 - blackKey));

            RSlider.Value = Math.Round(red * 255, MidpointRounding.AwayFromZero);
            GSlider.Value = Math.Round(green * 255, MidpointRounding.AwayFromZero);
            BSlider.Value = Math.Round(blue * 255, MidpointRounding.AwayFromZero);
        }
        private bool CheckIfSlidersLoaded()
        {

            if (RSlider == null || GSlider == null || BSlider == null || CSlider == null || MSlider == null || YSlider == null || KSlider == null)
            {
                return false;
            }

            return true;
        }
        #endregion
        #region slider value changed
        private void RSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (rgbDragged)
            {
                CalculateCMYK();

                FillRectangles();
            }

        }

        private void GSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (rgbDragged)
            {
                CalculateCMYK();

                FillRectangles();
            }
        }

        private void BSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (rgbDragged)
            {
                CalculateCMYK();

                FillRectangles();
            }
        }
        
        private void CSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (cmykDragged)
            {
                CalculateRGB();

                FillRectangles();
            }
        }

        private void MSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (cmykDragged)
            {
                CalculateRGB();

                FillRectangles();
            }
        }

        private void YSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (cmykDragged)
            {
                CalculateRGB();

                FillRectangles();
            }
        }

        private void KSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (cmykDragged)
            {
                CalculateRGB();

                FillRectangles();
            }
        }
        #endregion
        #region slider drag started/completed
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
        #endregion
        #region text box text changed

        private void RTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(CheckIfSlidersLoaded() && !string.IsNullOrEmpty(RTextBox.Text) && rgbTyping)
            {
                red = Convert.ToDouble(RTextBox.Text) / 255;

                CalculateCMYK();

                FillRectangles();
            }
        }

        private void GTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CheckIfSlidersLoaded() && !string.IsNullOrEmpty(GTextBox.Text) && rgbTyping)
            {
                green = Convert.ToDouble(GTextBox.Text) / 255;

                CalculateCMYK();

                FillRectangles();
            }
        }

        private void BTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CheckIfSlidersLoaded() && !string.IsNullOrEmpty(BTextBox.Text) && rgbTyping)
            {
                blue = Convert.ToDouble(BTextBox.Text) / 255;

                CalculateCMYK();

                FillRectangles();
            }
        }

        private void CTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CheckIfSlidersLoaded() && !string.IsNullOrEmpty(CTextBox.Text) && cmykTyping)
            {
                cyan = Convert.ToDouble(CTextBox.Text) / 255;

                CalculateRGB();

                FillRectangles();
            }
        }

        private void MTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CheckIfSlidersLoaded() && !string.IsNullOrEmpty(MTextBox.Text) && cmykTyping)
            {
                magenta = Convert.ToDouble(MTextBox.Text) / 255;

                CalculateRGB();

                FillRectangles();
            }
        }

        private void YTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CheckIfSlidersLoaded() && !string.IsNullOrEmpty(YTextBox.Text) && cmykTyping)
            {
                yellow = Convert.ToDouble(YTextBox.Text) / 255;

                CalculateRGB();

                FillRectangles();
            }
        }

        private void KTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CheckIfSlidersLoaded() && !string.IsNullOrEmpty(KTextBox.Text) && cmykTyping)
            {
                blackKey = Convert.ToDouble(KTextBox.Text) / 255;

                CalculateRGB();

                FillRectangles();
            }
        }
        #endregion
        #region text box got/lost focus
        private void RTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            rgbTyping = true;
        }

        private void GTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            rgbTyping = true;
        }

        private void BTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            rgbTyping = true;
        }

        private void CTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            cmykTyping = true;
        }

        private void MTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            cmykTyping = true;
        }

        private void YTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            cmykTyping = true;
        }

        private void KTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            cmykTyping = true;
        }

        private void RTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            rgbTyping = false;
        }

        private void GTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            rgbTyping = false;
        }

        private void BTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            rgbTyping = false;
        }

        private void CTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            cmykTyping = false;
        }

        private void MTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            cmykTyping = false;
        }

        private void YTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            cmykTyping = false;
        }

        private void KTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            cmykTyping = false;
        }
        #endregion


        Point dragStart;
        Point dragTotal;
        double Rotation;
        Vector3D AxisVector;
        bool dragging;
        private void viewport3D1_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            dragging = true;
            dragStart = e.GetPosition(viewport3D1);
            dragStart.Offset(-this.dragTotal.X, this.dragTotal.Y);
        }

        private void viewport3D1_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            dragging = false;

            var dragEnd = e.GetPosition(this.viewport3D1);
            this.dragTotal.X = dragEnd.X - this.dragStart.X;
            this.dragTotal.Y = dragEnd.Y - this.dragStart.Y;
        }

        private void viewport3D1_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (dragging)
            {
                var pos = e.GetPosition(this.viewport3D1);

                var x = pos.X - this.dragStart.X;
                var y = pos.Y - this.dragStart.Y;

                rot.Angle = Math.Sqrt(x * x + y * y);

                rot.Axis = new Vector3D(-y, 0 , -x);
            }
        }
    }
}
