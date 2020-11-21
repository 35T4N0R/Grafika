using Grafika.Shapes;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Grafika
{
    /// <summary>
    /// Logika interakcji dla klasy FithfPage.xaml
    /// </summary>
    public partial class FithfPage : Page
    {
        public Cursor cursor = Cursors.Arrow;
        public List<Ellipse> points = new List<Ellipse>();
        List<Point> bezierPoints = new List<Point>();
        private IDrawable _dr;
        private readonly myPen _pen;
        public bool DragInProgress = false;
        Point test = new Point();
        public const double R = 10;

        public FithfPage()
        {
            InitializeComponent();
            _pen = new myPen(Canvas);
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.DirectlyOver == Canvas)
            {
                Cursor = Cursors.Arrow;
            }
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.DirectlyOver == Canvas)
            {
                Cursor = Cursors.Arrow;

                var actionButton = ((Button)inputs.FindName("AddPointButton"));
                if (actionButton.Content.Equals("Modyfikuj"))
                {
                    for (int i = 0; i < inputs.Children.Count; i++)
                    {
                        if (inputs.Children[i] is Button) inputs.Children.RemoveAt(i);
                    }
                    var button = new Button();
                    button.Name = "AddPointButton";
                    button.Width = 75;
                    button.Margin = new Thickness(0, 15, 0, 0);
                    button.Click += AddPointButton_Click;
                    button.Content = "Dodaj Punkt";

                    inputs.Children.Add(button);

                    if (inputs.FindName("AddPointButton") != null)
                        inputs.UnregisterName("AddPointButton");

                    inputs.RegisterName(button.Name, button);

                    ((TextBox)inputs.FindName("XBox")).Text = "";
                    ((TextBox)inputs.FindName("YBox")).Text = "";
                }

            }
        }

        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

            var pos = e.MouseDevice.GetPosition(Canvas);

            _dr = new CurvePoint(pos, R);
            points.Add((_dr as CurvePoint).Circle);
            _pen.Down(_dr);

            if(points.Count >= 3)
            {
                test = pos;
                CalculateXY();
                DrawLines();

                
            }
        }

        public void CalculateXY()
        {
            int n = points.Count - 1;
            bezierPoints = new List<Point>();

            for (double t = 0.0; t <= 1.0; t += 0.01)
            {
                var newP = new Point();

                var sumX = 0.0;
                var sumY = 0.0;
                for (int i = 0; i <= n; i++)
                {
                    sumX += CalculateNewton(n, i) * Math.Pow(1 - t, n - i) * Math.Pow(t, i) * (Canvas.GetLeft(points[i]) + R);
                }

                newP.X = sumX;

                for (int i = 0; i <= n; i++)
                {
                    
                    sumY += CalculateNewton(n, i) * Math.Pow(t, i) * Math.Pow(1 - t, n - i) * (Canvas.GetTop(points[i]) + R);
                }

                newP.Y = sumY;

                bezierPoints.Add(newP);
            }

            //var point = new Point();
            //point.X = Canvas.GetLeft(points[points.Count - 1]) + (points[points.Count - 1].ActualWidth / 2);
            //point.Y = Canvas.GetTop(points[points.Count - 1]) + (points[points.Count - 1].ActualHeight / 2);

            //bezierPoints.Add(point);

        }

        public void DrawLines()
        {
            ClearLines();
            Line newLine;
            int i;

            newLine = new Line();
            newLine.StrokeThickness = 1;
            newLine.Stroke = Brushes.Red;
            newLine.X1 = Canvas.GetLeft(points[0]) + R;
            newLine.Y1 = Canvas.GetTop(points[0]) + R;
            newLine.X2 = bezierPoints[0].X;
            newLine.Y2 = bezierPoints[0].Y;

            Canvas.Children.Add(newLine);

            for (i = 1; i < bezierPoints.Count; i++)
            {
                newLine = new Line();
                newLine.StrokeThickness = 1;
                newLine.Stroke = Brushes.Red;
                newLine.X1 = bezierPoints[i - 1].X;
                newLine.Y1 = bezierPoints[i - 1].Y;
                newLine.X2 = bezierPoints[i].X;
                newLine.Y2 = bezierPoints[i].Y;

                Canvas.Children.Add(newLine);
            }

            newLine = new Line();
            newLine.StrokeThickness = 1;
            newLine.Stroke = Brushes.Red;
            newLine.X1 = bezierPoints[bezierPoints.Count - 1].X;
            newLine.Y1 = bezierPoints[bezierPoints.Count - 1].Y;
            newLine.X2 = Canvas.GetLeft(points[points.Count - 1]) + R;
            newLine.Y2 = Canvas.GetTop(points[points.Count - 1]) + R;

            Canvas.Children.Add(newLine);
        }

        public void ClearLines()
        {

            for (int i = 0; i < Canvas.Children.Count; i++)
            {
                if(Canvas.Children[i] is Line)
                {
                    Canvas.Children.RemoveAt(i--);
                }
            }

        }

        public double CalculateNewton(int n, int i)
        {
            int result = 1;

            var factorialN = 1.0;
            var factorialI = 1.0;
            var factorialNI = 1.0;

            for (int j = 1; j <= n; j++)
            {
                factorialN *= j;
            }

            for (int j = 1; j <= i; j++)
            {
                factorialI *= j;
            }

            for (int j = 1; j <= (n - i); j++)
            {
                factorialNI *= j;
            }

            result = (int)((double)factorialN / (double)(factorialI * factorialNI));

            return result;
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            points = new List<Ellipse>();
            bezierPoints = new List<Point>();
            DragInProgress = false;

            Canvas.Children.Clear();
        }

        public void AddPointButton_Click(object sender, RoutedEventArgs e)
        {
            var xT = XBox.Text;
            var yT = YBox.Text;

            if(!String.IsNullOrEmpty(xT) && !String.IsNullOrEmpty(yT))
            {
                var xResult = Double.TryParse(xT, out double x);
                var yResult = Double.TryParse(yT, out double y);
                
                if(xResult && yResult)
                {
                    _dr = new CurvePoint(new Point(x, y), R);
                    points.Add((_dr as CurvePoint).Circle);
                    _pen.Down(_dr);

                    if (points.Count >= 3)
                    {
                        CalculateXY();
                        DrawLines();


                    }
                }
            }
        }
    }

    public class CurvePoint : IDrawable
    {
        public Ellipse Circle { get; private set; }

        //public CurvePoint(Point location)
        //{
        //    Circle = new Ellipse
        //    {
        //        Stroke = Brushes.Black,
        //        StrokeThickness = 2,
        //    };

        //    Circle.MouseMove += new MouseEventHandler(Circle_MouseMove);
        //    Circle.MouseUp += new MouseButtonEventHandler(Circle_MouseUp);
        //    Circle.MouseLeftButtonDown += new MouseButtonEventHandler(Circle_MouseDown);

        //}

        public CurvePoint(Point center, double r)
        {
            Circle = new Ellipse
            {
                Stroke = Brushes.Black,
                Fill = Brushes.White,
                StrokeThickness = 2,
                Height = 2 * r,
                Width = 2 * r,
            };

            if (Circle != null)
            {
                Canvas.SetTop(Circle, center.Y - r);
                Canvas.SetLeft(Circle, center.X - r);
            }

            Circle.MouseMove += new MouseEventHandler(Circle_MouseMove);
            Circle.MouseUp += new MouseButtonEventHandler(Circle_MouseUp);
            Circle.MouseLeftButtonDown += new MouseButtonEventHandler(Circle_MouseDown);

        }

        MainWindow mw = (MainWindow)Application.Current.MainWindow;
        FithfPage fp = (((MainWindow)Application.Current.MainWindow).Content as Frame).Content as FithfPage;
        

        private Point lastPoint;

        private void Circle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SetCursor(Circle, Mouse.GetPosition(fp.Canvas));

            lastPoint = e.GetPosition(fp.Canvas);
            fp.DragInProgress = true;

            Mouse.Capture((IInputElement)sender);

            //    fp.clearInputsAndNames();
            //    fp.SetInputs(FirstPage.Shapes.Circle);

            var actionButton = ((Button)fp.inputs.FindName("AddPointButton"));

            for (int i = 0; i < fp.inputs.Children.Count; i++)
            {
                if (fp.inputs.Children[i] is Button) fp.inputs.Children.RemoveAt(i);
            }

            var button = new Button();
            button.Name = "AddPointButton";
            button.Width = 75;
            button.Margin = new Thickness(0, 15, 0, 0);
            button.Click += modifyCircle;
            button.Content = "Modyfikuj";

            fp.inputs.Children.Add(button);

            if (fp.inputs.FindName("AddPointButton") != null)
                fp.inputs.UnregisterName("AddPointButton");

            fp.inputs.RegisterName(button.Name, button);

            ((TextBox)fp.inputs.FindName("XBox")).Text = Convert.ToString(Canvas.GetLeft(Circle) + Circle.Width / 2);
            ((TextBox)fp.inputs.FindName("YBox")).Text = Convert.ToString(Canvas.GetTop(Circle) + Circle.Width / 2);

            //    ((TextBox)fp.inputs.FindName("r")).Text = Convert.ToString(Circle.Width / 2);

            //    
            //}
        }

        private void Circle_MouseMove(object sender, MouseEventArgs e)
        {
            SetCursor(Circle, Mouse.GetPosition(fp.Canvas));

            if (fp.DragInProgress)
            {
                Point point = Mouse.GetPosition(fp.Canvas);

                if (point.Y > fp.Canvas.ActualHeight || point.X > fp.Canvas.ActualWidth || point.Y < 0 || point.X < 0)
                {
                    return;
                }

                double offset_x = point.X - lastPoint.X;
                double offset_y = point.Y - lastPoint.Y;

                double recX = Canvas.GetLeft(Circle);
                double recY = Canvas.GetTop(Circle);

                recX += offset_x;
                recY += offset_y;

                Canvas.SetLeft(Circle, recX);
                Canvas.SetTop(Circle, recY);

                lastPoint = point;

                if (fp.points.Count >= 3)
                {
                    fp.CalculateXY();
                    fp.DrawLines();
                }

                fp.XBox.Text = Convert.ToString(Canvas.GetLeft(Circle) + Circle.Width / 2);
                fp.YBox.Text = Convert.ToString(Canvas.GetTop(Circle) + Circle.Width / 2);
            }

            //try
            //{

            //    if (fp.DragInProgress)
            //    {

            //        Point point = Mouse.GetPosition(fp.Canvas);

            //        if (point.Y > fp.Canvas.ActualHeight || point.X > fp.Canvas.ActualWidth || point.Y < 0 || point.X < 0)
            //        {
            //            return;
            //        }

            //        double offset_x = point.X - lastPoint.X;
            //        double offset_y = point.Y - lastPoint.Y;

            //        double recX = Canvas.GetLeft(Circle);
            //        double recY = Canvas.GetTop(Circle);
            //        double recWidth = Circle.Width;
            //        double recHeight = Circle.Height;

            //        switch (fp.MouseHitType)
            //        {
            //            case FirstPage.HitType.Body:
            //                recX += offset_x;
            //                recY += offset_y;
            //                break;
            //            case FirstPage.HitType.UL:
            //                recX += offset_x;
            //                recY += offset_y;
            //                recWidth -= offset_x;
            //                recHeight -= offset_y;
            //                break;
            //            case FirstPage.HitType.UR:
            //                recY += offset_y;
            //                recWidth += offset_x;
            //                recHeight -= offset_y;
            //                break;
            //            case FirstPage.HitType.LR:
            //                recWidth += offset_x;
            //                recHeight += offset_y;
            //                break;
            //            case FirstPage.HitType.LL:
            //                recX += offset_x;
            //                recWidth -= offset_x;
            //                recHeight += offset_y;
            //                break;
            //            case FirstPage.HitType.L:
            //                recX += offset_x;
            //                recWidth -= offset_x;
            //                break;
            //            case FirstPage.HitType.R:
            //                recWidth += offset_x;
            //                break;
            //            case FirstPage.HitType.B:
            //                recHeight += offset_y;
            //                break;
            //            case FirstPage.HitType.T:
            //                recY += offset_y;
            //                recHeight -= offset_y;
            //                break;
            //        }

            //        if ((recWidth > 0) && (recHeight > 0))
            //        {
            //            Canvas.SetLeft(Circle, recX);
            //            Canvas.SetTop(Circle, recY);

            //            Circle.Width = recWidth;
            //            Circle.Height = recHeight;

            //            lastPoint = point;
            //        }

            //        ((TextBox)fp.inputs.FindName("x1")).Text = Convert.ToString(Canvas.GetLeft(Circle) + Circle.Width / 2);
            //        ((TextBox)fp.inputs.FindName("y1")).Text = Convert.ToString(Canvas.GetTop(Circle) + Circle.Width / 2);
            //        ((TextBox)fp.inputs.FindName("r")).Text = Convert.ToString(Circle.Width / 2);
            //    }
            //    else
            //    {
            //        fp.MouseHitType = fp.SetHitType(Circle, Mouse.GetPosition(fp.Canvas));
            //        fp.SetMouseCursor();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}

        }

        private void Circle_MouseUp(object sender, MouseButtonEventArgs e)
        {
            fp.DragInProgress = false;
            Mouse.Capture(null);
        }

        public void Draw(Point location, Point start)
        {
            if (Circle != null)
            {
                double minX = Math.Min(location.X, start.X);
                double minY = Math.Min(location.Y, start.Y);
                double maxX = Math.Max(location.X, start.X);
                double maxY = Math.Max(location.Y, start.Y);

                Canvas.SetTop(Circle, minY);
                Canvas.SetLeft(Circle, minX);


                double height = maxY - minY;
                double width = maxX - minX;

                Circle.Height = Math.Abs(height);
                Circle.Width = Math.Abs(width);
            }
        }

        public void modifyCircle(object sender, RoutedEventArgs e)
        {
            var x = Convert.ToDouble(((TextBox)fp.inputs.FindName("XBox")).Text);
            var y = Convert.ToDouble(((TextBox)fp.inputs.FindName("YBox")).Text);

            Circle.SetValue(Canvas.TopProperty, y - FithfPage.R);
            Circle.SetValue(Canvas.LeftProperty, x - FithfPage.R);

            Circle.Height = Math.Abs(FithfPage.R * 2);
            Circle.Width = Math.Abs(FithfPage.R * 2);

            if (fp.points.Count >= 3)
            {
                fp.CalculateXY();
                fp.DrawLines();
            }
        }

        public void SetCursor(Shape shape, Point point)
        {
            double left = Canvas.GetLeft(shape);
            double top = Canvas.GetTop(shape);
            double right = left + shape.Width;
            double bottom = top + shape.Height;

            if (point.X < left || point.X > right || point.Y < top | point.Y > bottom)
            {
                fp.Cursor = Cursors.Arrow;
                return;
            }

            fp.Cursor = Cursors.ScrollAll;

            return;
        }
    }
}
