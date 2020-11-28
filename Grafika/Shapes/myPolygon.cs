using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Grafika.Shapes
{
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

                int new_x1 = (int)(pos.X + OffsetX);
                int new_y1 = (int)(pos.Y + OffsetY);

                int dx = (int)(new_x1 - Polygon.Points[0].X);
                int dy = (int)(new_y1 - Polygon.Points[0].Y);

                if (dx == 0 && dy == 0) return;

                for (int i = 0; i < Polygon.Points.Count; i++)
                {
                    Polygon.Points[i] = new Point(Polygon.Points[i].X + dx, Polygon.Points[i].Y + dy);

                    var centerX = Canvas.GetLeft(PolygonPoints[i]);
                    var centerY = Canvas.GetTop(PolygonPoints[i]);

                    PolygonPoints[i].SetValue(Canvas.TopProperty, centerY + dy);
                    PolygonPoints[i].SetValue(Canvas.LeftProperty, centerX + dx);

                }

            }

        }

        private void Polygon_MouseUp(object sender, MouseButtonEventArgs e)
        {
            sp.DragInProgress = false;
            Mouse.Capture(null);
        }

        public void Draw(Point location, Point start)
        {
            sp.Canvas.Children.Add(Polygon);
        }
    }
}
