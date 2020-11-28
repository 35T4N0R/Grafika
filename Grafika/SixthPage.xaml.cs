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
    /// Logika interakcji dla klasy SixthPage.xaml
    /// </summary>
    public partial class SixthPage : Page
    {
        public Cursor cursor = Cursors.Arrow;
        public List<Ellipse> points = new List<Ellipse>();
        private IDrawable _dr;
        private readonly myPen _pen;
        public bool DragInProgress = false;
        public const double R = 4;
        public bool drawing = false;
        public myPolygon currentPolygon;
        public SixthPage()
        {
            InitializeComponent();
            _pen = new myPen(Canvas);
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.DirectlyOver == Canvas)
            {
                Cursor = Cursors.Arrow;
            }
        }

        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

            var pos = e.MouseDevice.GetPosition(Canvas);

            _dr = new PolygonPoint(pos, R);
            points.Add((_dr as PolygonPoint).Circle);
            _pen.Down(_dr);

            DrawPolygon((_dr as PolygonPoint).Circle);
        }

        public void RemovePolygons()
        {
            for (int i = 0; i < Canvas.Children.Count; i++)
            {
                if (Canvas.Children[i] is Polygon)
                {
                    Canvas.Children.RemoveAt(i--);
                }
            }

        }

        public IEnumerable<Point> ConvertCirclesToPoints()
        {
            var pointList = new List<Point>();

            foreach (var circle in points)
            {
                var newPoint = new Point(Canvas.GetLeft(circle) + R, Canvas.GetTop(circle) + R);

                pointList.Add(newPoint);
            }

            return pointList;
        }

        public void DrawPolygon(Ellipse pp)
        {

            //RemovePolygons();
            if (drawing)
            {

                currentPolygon.AddPoint(pp);
            }
            else
            {
                var polygon = new myPolygon();
                _pen.Down(polygon);
                currentPolygon = polygon;
                currentPolygon.AddPoint(pp);
                drawing = true;
                NewPolygon.Fill = Brushes.Red;
            }
            //if(points.Count == 3)
            //{
            //    //polygon = new Polygon();
            //    polygon.Points = new PointCollection(ConvertCirclesToPoints());
            //}
            //else if(points.Count > 3)
            //{
            //    //polygon = (Polygon)Canvas.FindName("polygon");
            //    polygon.Points.Add(new Point(Canvas.GetLeft(points[points.Count - 1]) + R, Canvas.GetTop(points[points.Count - 1]) + R)); /*= new PointCollection(ConvertCirclesToPoints());*/ /*{ new Point(10, 10), new Point(100, 100), new Point(200, 200) };*/
            //}

            //_pen.Draw()

        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < Canvas.Children.Count; i++)
            {
                Canvas.Children.RemoveAt(i--);
            }
            drawing = false;
            NewPolygon.Fill = Brushes.LightGreen;
            //currentPolygon.PolygonPoints = new List<Ellipse>();
            //currentPolygon.Polygon.Points = new PointCollection();
        }

        private void MovePolygonButton_Click(object sender, RoutedEventArgs e)
        {
            var hT = XBox.Text;
            var vT = YBox.Text;

            if (!String.IsNullOrEmpty(hT) && !String.IsNullOrEmpty(vT))
            {
                var hResult = Double.TryParse(hT, out double h);
                var vResult = Double.TryParse(vT, out double v);

                if (hResult && vResult)
                {
                    for (int i = 0; i < currentPolygon.Polygon.Points.Count; i++)
                    {
                        var point = currentPolygon.Polygon.Points[i];
                        point.X += h;
                        point.Y += v;

                        var centerX = Canvas.GetLeft(currentPolygon.PolygonPoints[i]);
                        var centerY = Canvas.GetTop(currentPolygon.PolygonPoints[i]);

                        currentPolygon.PolygonPoints[i].SetValue(Canvas.TopProperty, centerY + v);
                        currentPolygon.PolygonPoints[i].SetValue(Canvas.LeftProperty, centerX + h);

                        currentPolygon.Polygon.Points[i] = point;

                    }
                }
            }
        }

        private void RotatePolygonButton_Click(object sender, RoutedEventArgs e)
        {
            var x0T = XBox.Text;
            var y0T = YBox.Text;
            var alfaT = alfaBox.Text;

            if (!String.IsNullOrEmpty(x0T) && !String.IsNullOrEmpty(y0T) && !String.IsNullOrEmpty(alfaT))
            {
                var x0Result = Double.TryParse(x0T, out double x0);
                var y0Result = Double.TryParse(y0T, out double y0);
                var alfaResult = Double.TryParse(alfaT, out double alfa);

                if (x0Result && y0Result && alfaResult)
                {
                    for (int i = 0; i < currentPolygon.Polygon.Points.Count; i++)
                    {
                        var point = currentPolygon.Polygon.Points[i];

                        var radians = Math.PI * alfa / 180.0;

                        var xOutcome = x0 + ((point.X - x0) * Math.Cos(radians)) - ((point.Y - y0) * Math.Sin(radians));
                        var yOutcome = y0 + ((point.X - x0) * Math.Sin(radians)) + ((point.Y - y0) * Math.Cos(radians));

                        point.X = xOutcome;
                        point.Y = yOutcome;

                        currentPolygon.PolygonPoints[i].SetValue(Canvas.TopProperty, yOutcome - R);
                        currentPolygon.PolygonPoints[i].SetValue(Canvas.LeftProperty, xOutcome - R);

                        currentPolygon.Polygon.Points[i] = point;

                    }
                }
            }
        }

        private void ScalePolygonButton_Click(object sender, RoutedEventArgs e)
        {
            var xsT = XBox.Text;
            var ysT = YBox.Text;
            var kT = kBox.Text;

            if (!String.IsNullOrEmpty(xsT) && !String.IsNullOrEmpty(ysT) && !String.IsNullOrEmpty(kT))
            {
                var xsResult = Double.TryParse(xsT, out double xs);
                var ysResult = Double.TryParse(ysT, out double ys);
                var kResult = Double.TryParse(kT, out double k);

                if (xsResult && ysResult && kResult)
                {
                    for (int i = 0; i < currentPolygon.Polygon.Points.Count; i++)
                    {
                        var point = currentPolygon.Polygon.Points[i];

                        var xOutcome = (point.X * k) + ((1 - k) * xs);
                        var yOutcome = (point.Y * k) + ((1 - k) * ys);

                        point.X = xOutcome;
                        point.Y = yOutcome;

                        currentPolygon.PolygonPoints[i].SetValue(Canvas.TopProperty, yOutcome - R);
                        currentPolygon.PolygonPoints[i].SetValue(Canvas.LeftProperty, xOutcome - R);

                        currentPolygon.Polygon.Points[i] = point;

                    }
                }
            }
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            

            /* Check if it is a double click */
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                drawing = false;
                NewPolygon.Fill = Brushes.LightGreen;
            }
            else
            {
                var pos = e.MouseDevice.GetPosition(Canvas);

                XBox.Text = pos.X.ToString();
                YBox.Text = pos.Y.ToString();
            }
            e.Handled = true;
        }

        int OffsetX;
        int OffsetY;

        private void pol_MouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.DirectlyOver == pol)
            {
                Cursor = Cursors.ScrollAll;
            }
            
            if (DragInProgress)
            {
                var pos = e.MouseDevice.GetPosition(Canvas);


                // See how far the first point will move.
                int new_x1 = (int)(pos.X + OffsetX);
                int new_y1 = (int)(pos.Y + OffsetY);

                int dx = (int)(new_x1 - pol.Points[0].X);
                int dy = (int)(new_y1 - pol.Points[0].Y);

                if (dx == 0 && dy == 0) return;

                // Move the polygon.
                for (int i = 0; i < pol.Points.Count; i++)
                {
                    pol.Points[i] = new Point( pol.Points[i].X + dx, pol.Points[i].Y + dy);

                    var centerX = Canvas.GetLeft(points[i]);
                    var centerY = Canvas.GetTop(points[i]);

                    points[i].SetValue(Canvas.TopProperty, centerY + dy);
                    points[i].SetValue(Canvas.LeftProperty, centerX + dx);

                }

                // Redraw.
                //Canvas.Invalidate();
            }
        }

        private void pol_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var pos = e.MouseDevice.GetPosition(Canvas);

            OffsetX = (int)(pol.Points[0].X - pos.X);
            OffsetY = (int)(pol.Points[0].Y - pos.Y);

            DragInProgress = true;
        }

        private void pol_MouseUp(object sender, MouseButtonEventArgs e)
        {
            DragInProgress = false;
            Mouse.Capture(null);
        }
    }

    public class PolygonPoint : IDrawable
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

        public PolygonPoint(Point center, double r)
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

            //Circle.MouseMove += new MouseEventHandler(Circle_MouseMove);
            //Circle.MouseUp += new MouseButtonEventHandler(Circle_MouseUp);
            //Circle.MouseLeftButtonDown += new MouseButtonEventHandler(Circle_MouseDown);

        }

        MainWindow mw = (MainWindow)Application.Current.MainWindow;
        SixthPage sp = (((MainWindow)Application.Current.MainWindow).Content as Frame).Content as SixthPage;


        private Point lastPoint;

        private void Circle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SetCursor(Circle, Mouse.GetPosition(sp.Canvas));

            lastPoint = e.GetPosition(sp.Canvas);
            sp.DragInProgress = true;

            Mouse.Capture((IInputElement)sender);

            //    fp.clearInputsAndNames();
            //    fp.SetInputs(FirstPage.Shapes.Circle);

            //var actionButton = ((Button)sp.inputs.FindName("AddPointButton"));

            //for (int i = 0; i < sp.inputs.Children.Count; i++)
            //{
            //    if (sp.inputs.Children[i] is Button) sp.inputs.Children.RemoveAt(i);
            //}

            //var button = new Button();
            //button.Name = "AddPointButton";
            //button.Width = 75;
            //button.Margin = new Thickness(0, 15, 0, 0);
            //button.Click += modifyCircle;
            //button.Content = "Modyfikuj";

            //sp.inputs.Children.Add(button);

            //if (sp.inputs.FindName("AddPointButton") != null)
            //    sp.inputs.UnregisterName("AddPointButton");

            //sp.inputs.RegisterName(button.Name, button);

            //((TextBox)sp.inputs.FindName("XBox")).Text = Convert.ToString(Canvas.GetLeft(Circle) + Circle.Width / 2);
            //((TextBox)sp.inputs.FindName("YBox")).Text = Convert.ToString(Canvas.GetTop(Circle) + Circle.Width / 2);

            //    ((TextBox)fp.inputs.FindName("r")).Text = Convert.ToString(Circle.Width / 2);

            //    
            //}
        }

        private void Circle_MouseMove(object sender, MouseEventArgs e)
        {
            SetCursor(Circle, Mouse.GetPosition(sp.Canvas));

            if (sp.DragInProgress)
            {
                Point point = Mouse.GetPosition(sp.Canvas);

                if (point.Y > sp.Canvas.ActualHeight || point.X > sp.Canvas.ActualWidth || point.Y < 0 || point.X < 0)
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

                if (sp.points.Count >= 3)
                {
                    //sp.CalculateXY();
                    //sp.DrawLines();
                }

                //sp.XBox.Text = Convert.ToString(Canvas.GetLeft(Circle) + Circle.Width / 2);
                //sp.YBox.Text = Convert.ToString(Canvas.GetTop(Circle) + Circle.Width / 2);
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
            sp.DragInProgress = false;
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
            //var x = Convert.ToDouble(((TextBox)sp.inputs.FindName("XBox")).Text);
            //var y = Convert.ToDouble(((TextBox)sp.inputs.FindName("YBox")).Text);

            //Circle.SetValue(Canvas.TopProperty, y - FithfPage.R);
            //Circle.SetValue(Canvas.LeftProperty, x - FithfPage.R);

            //Circle.Height = Math.Abs(FithfPage.R * 2);
            //Circle.Width = Math.Abs(FithfPage.R * 2);

            //if (sp.points.Count >= 3)
            //{
            //    sp.CalculateXY();
            //    sp.DrawLines();
            //}
        }

        public void SetCursor(Shape shape, Point point)
        {
            double left = Canvas.GetLeft(shape);
            double top = Canvas.GetTop(shape);
            double right = left + shape.Width;
            double bottom = top + shape.Height;

            if (point.X < left || point.X > right || point.Y < top | point.Y > bottom)
            {
                sp.Cursor = Cursors.Arrow;
                return;
            }

            sp.Cursor = Cursors.ScrollAll;

            return;
        }
    }

    public class myPolygon : IDrawable
    {
        public Polygon Polygon { get; set; }
        public List<Ellipse> PolygonPoints { get; set; }

        int OffsetX;
        int OffsetY;


        public myPolygon()
        {
            Polygon = new Polygon()
            {
                Fill = Brushes.LightPink,
                StrokeThickness = 1
            };
            PolygonPoints = new List<Ellipse>();

            Polygon.MouseMove += new MouseEventHandler(Polygon_MouseMove);
            Polygon.MouseUp += new MouseButtonEventHandler(Polygon_MouseUp);
            Polygon.MouseLeftButtonDown += new MouseButtonEventHandler(Polygon_MouseDown);

        }

        MainWindow mw = (MainWindow)Application.Current.MainWindow;
        SixthPage sp = (((MainWindow)Application.Current.MainWindow).Content as Frame).Content as SixthPage;

        public void AddPoint(Ellipse pp)
        {
            PolygonPoints.Add(pp);

            Polygon.Points.Add(new Point(Canvas.GetLeft(pp) + SixthPage.R, Canvas.GetTop(pp) + SixthPage.R));
        }

        private void Polygon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            sp.currentPolygon = this;

            var pos = e.MouseDevice.GetPosition(sp.Canvas);

            OffsetX = (int)(Polygon.Points[0].X - pos.X);
            OffsetY = (int)(Polygon.Points[0].Y - pos.Y);

            sp.DragInProgress = true;
        }

        private void Polygon_MouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.DirectlyOver == Polygon)
            {
                sp.Cursor = Cursors.ScrollAll;
            }

            if (sp.DragInProgress)
            {
                var pos = e.MouseDevice.GetPosition(sp.Canvas);


                // See how far the first point will move.
                int new_x1 = (int)(pos.X + OffsetX);
                int new_y1 = (int)(pos.Y + OffsetY);

                int dx = (int)(new_x1 - Polygon.Points[0].X);
                int dy = (int)(new_y1 - Polygon.Points[0].Y);

                if (dx == 0 && dy == 0) return;

                // Move the polygon.
                for (int i = 0; i < Polygon.Points.Count; i++)
                {
                    Polygon.Points[i] = new Point(Polygon.Points[i].X + dx, Polygon.Points[i].Y + dy);

                    var centerX = Canvas.GetLeft(PolygonPoints[i]);
                    var centerY = Canvas.GetTop(PolygonPoints[i]);

                    PolygonPoints[i].SetValue(Canvas.TopProperty, centerY + dy);
                    PolygonPoints[i].SetValue(Canvas.LeftProperty, centerX + dx);

                }

                // Redraw.
                //Canvas.Invalidate();
            }

        }

        private void Polygon_MouseUp(object sender, MouseButtonEventArgs e)
        {
            sp.DragInProgress = false;
            Mouse.Capture(null);
        }

        public void Draw(Point location, Point start)
        {
            //if (Polygon != null)
            //{
            //    double minX = Math.Min(location.X, start.X);
            //    double minY = Math.Min(location.Y, start.Y);
            //    double maxX = Math.Max(location.X, start.X);
            //    double maxY = Math.Max(location.Y, start.Y);

            //    Canvas.SetTop(Polygon, minY);
            //    Canvas.SetLeft(Polygon, minX);


            //    double height = maxY - minY;
            //    double width = maxX - minX;

            //    Polygon.Height = Math.Abs(height);
            //    Polygon.Width = Math.Abs(width);
            //}

            sp.Canvas.Children.Add(Polygon);
        }

        public void modifyCircle(object sender, RoutedEventArgs e)
        {
            //var x = Convert.ToDouble(((TextBox)sp.inputs.FindName("XBox")).Text);
            //var y = Convert.ToDouble(((TextBox)sp.inputs.FindName("YBox")).Text);

            //Circle.SetValue(Canvas.TopProperty, y - FithfPage.R);
            //Circle.SetValue(Canvas.LeftProperty, x - FithfPage.R);

            //Circle.Height = Math.Abs(FithfPage.R * 2);
            //Circle.Width = Math.Abs(FithfPage.R * 2);

            //if (sp.points.Count >= 3)
            //{
            //    sp.CalculateXY();
            //    sp.DrawLines();
            //}
        }

        public void SetCursor(Shape shape, Point point)
        {
            double left = Canvas.GetLeft(shape);
            double top = Canvas.GetTop(shape);
            double right = left + shape.Width;
            double bottom = top + shape.Height;

            if (point.X < left || point.X > right || point.Y < top | point.Y > bottom)
            {
                sp.Cursor = Cursors.Arrow;
                return;
            }

            sp.Cursor = Cursors.ScrollAll;

            return;
        }
    }

}
