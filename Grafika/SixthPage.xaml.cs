using Grafika.SerializeModels;
using Grafika.Shapes;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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

        public void DrawPolygon(Ellipse pp)
        {
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

        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < Canvas.Children.Count; i++)
            {
                Canvas.Children.RemoveAt(i--);
            }
            drawing = false;
            NewPolygon.Fill = Brushes.LightGreen;
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

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            List<JsonPolygon> objects = new List<JsonPolygon>();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.CheckFileExists = true;
            openFileDialog.ShowDialog();

            if (openFileDialog.FileName.Trim() != string.Empty)
            {
                using (StreamReader r = new StreamReader(openFileDialog.FileName))
                {
                    string jsonString = r.ReadToEnd();
                    objects = JsonConvert.DeserializeObject<List<JsonPolygon>>(jsonString);
                }
            }

            drawing = false;

            foreach (var obj in objects)
            {

                foreach(var p in obj.PointsList)
                {
                    _dr = new PolygonPoint(new Point(p.X, p.Y), R);
                    _pen.Down(_dr);

                    DrawPolygon((_dr as PolygonPoint).Circle);
                }

                drawing = false;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var shapes = Canvas.Children;

            List<JsonPolygon> objects = new List<JsonPolygon>();

            foreach (object obj in shapes)
            {
                if (obj is Polygon)
                {
                    Polygon polygon = (Polygon)obj;
                    JsonPolygon newObj = new JsonPolygon()
                    {
                        PointsList = polygon.Points
                    };

                    objects.Add(newObj);
                }
            }

            string jsonString = JsonConvert.SerializeObject(objects.ToArray());

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JSON (*.json)|*.json|All Files (*.*)|*.*";
            saveFileDialog.DefaultExt = ".json";
            saveFileDialog.ShowDialog();

            if (!string.IsNullOrEmpty(saveFileDialog.FileName))
            {
                File.WriteAllText(saveFileDialog.FileName, jsonString);
            }
        }
    }

    public class PolygonPoint : IDrawable
    {
        public Ellipse Circle { get; private set; }

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
    }

    

}
