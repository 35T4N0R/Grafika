using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Grafika.Shapes
{

    public class myCircle : IDrawable
    {
        public Ellipse Circle { get; private set; }

        public myCircle(Point location)
        {
            Circle = new Ellipse
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2,
            };

            Circle.MouseMove += new MouseEventHandler(Circle_MouseMove);
            Circle.MouseUp += new MouseButtonEventHandler(Circle_MouseUp);
            Circle.MouseLeftButtonDown += new MouseButtonEventHandler(Circle_MouseDown);

        }

        public myCircle(Point center, double r)
        {
            Circle = new Ellipse
            {
                Stroke = Brushes.Black,
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
        FirstPage fp = (((MainWindow)Application.Current.MainWindow).Content as Frame).Content as FirstPage;

        private Point lastPoint;

        private void Circle_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (fp.currentShape == FirstPage.Shapes.Cursor)
            {

                fp.MouseHitType = fp.SetHitType(Circle, Mouse.GetPosition(fp.Canvas));
                fp.SetMouseCursor();
                if (fp.MouseHitType == FirstPage.HitType.None) return;

                lastPoint = e.GetPosition(fp.Canvas);
                fp.DragInProgress = true;


                fp.clearInputsAndNames();
                fp.SetInputs(FirstPage.Shapes.Circle);
                var actionButton = ((Button)fp.inputs.FindName("actionButton"));
                actionButton.Content = "Modyfikuj";
                actionButton.Click -= fp.rysujButton_Click;
                actionButton.Click += modifyCircle;
                ((TextBox)fp.inputs.FindName("x1")).Text = Convert.ToString(Canvas.GetLeft(Circle) + Circle.Width / 2);
                ((TextBox)fp.inputs.FindName("y1")).Text = Convert.ToString(Canvas.GetTop(Circle) + Circle.Width / 2);
                ((TextBox)fp.inputs.FindName("r")).Text = Convert.ToString(Circle.Width / 2);

                //drag = true;
                //startPt = e.GetPosition(mw.Canvas);
                //wid = (int)Circle.Width;
                //hei = (int)Circle.Height;
                //lastLoc = new Point(Canvas.GetLeft(Circle), Canvas.GetTop(Circle));
                Mouse.Capture((IInputElement)sender);
            }
        }

        private void Circle_MouseMove(object sender, MouseEventArgs e)
        {
            if (fp.currentShape == FirstPage.Shapes.Cursor)
            {
                try
                {
                    //if (drag)
                    //{
                    //    var newX = (startPt.X + (e.GetPosition(mw.Canvas).X - startPt.X));
                    //    var newY = (startPt.Y + (e.GetPosition(mw.Canvas).Y - startPt.Y));
                    //    Point offset = new Point((startPt.X - lastLoc.X), (startPt.Y - lastLoc.Y));
                    //    CanvasTop = newY - offset.Y;
                    //    CanvasLeft = newX - offset.X;

                    //    var cursorPoint = e.MouseDevice.GetPosition(mw.Canvas);
                    //    if (cursorPoint.Y > mw.Canvas.ActualHeight || cursorPoint.X > mw.Canvas.ActualWidth || cursorPoint.Y < 0 || cursorPoint.X < 0)
                    //    {
                    //        return;
                    //    }
                    //    Circle.SetValue(Canvas.TopProperty, CanvasTop);
                    //    Circle.SetValue(Canvas.LeftProperty, CanvasLeft);

                    //    ((TextBox)mw.inputs.FindName("x1")).Text = Convert.ToString(Canvas.GetLeft(Circle));
                    //    ((TextBox)mw.inputs.FindName("y1")).Text = Convert.ToString(Canvas.GetTop(Circle));
                    //    ((TextBox)mw.inputs.FindName("r")).Text = Convert.ToString(Circle.Width / 2);

                    //}

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
                        double recWidth = Circle.Width;
                        double recHeight = Circle.Height;

                        switch (fp.MouseHitType)
                        {
                            case FirstPage.HitType.Body:
                                recX += offset_x;
                                recY += offset_y;
                                break;
                            case FirstPage.HitType.UL:
                                recX += offset_x;
                                recY += offset_y;
                                recWidth -= offset_x;
                                recHeight -= offset_y;
                                break;
                            case FirstPage.HitType.UR:
                                recY += offset_y;
                                recWidth += offset_x;
                                recHeight -= offset_y;
                                break;
                            case FirstPage.HitType.LR:
                                recWidth += offset_x;
                                recHeight += offset_y;
                                break;
                            case FirstPage.HitType.LL:
                                recX += offset_x;
                                recWidth -= offset_x;
                                recHeight += offset_y;
                                break;
                            case FirstPage.HitType.L:
                                recX += offset_x;
                                recWidth -= offset_x;
                                break;
                            case FirstPage.HitType.R:
                                recWidth += offset_x;
                                break;
                            case FirstPage.HitType.B:
                                recHeight += offset_y;
                                break;
                            case FirstPage.HitType.T:
                                recY += offset_y;
                                recHeight -= offset_y;
                                break;
                        }

                        if ((recWidth > 0) && (recHeight > 0))
                        {
                            Canvas.SetLeft(Circle, recX);
                            Canvas.SetTop(Circle, recY);

                            Circle.Width = recWidth;
                            Circle.Height = recHeight;

                            lastPoint = point;
                        }

                        ((TextBox)fp.inputs.FindName("x1")).Text = Convert.ToString(Canvas.GetLeft(Circle) + Circle.Width / 2);
                        ((TextBox)fp.inputs.FindName("y1")).Text = Convert.ToString(Canvas.GetTop(Circle) + Circle.Width / 2);
                        ((TextBox)fp.inputs.FindName("r")).Text = Convert.ToString(Circle.Width / 2);
                    }
                    else
                    {
                        fp.MouseHitType = fp.SetHitType(Circle, Mouse.GetPosition(fp.Canvas));
                        fp.SetMouseCursor();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

        }

        private void Circle_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (fp.currentShape == FirstPage.Shapes.Cursor)
            {
                fp.DragInProgress = false;
                Mouse.Capture(null);
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

        private void modifyCircle(object sender, RoutedEventArgs e)
        {
            if (!fp.checkInputs(FirstPage.Shapes.Circle))
            {
                var x1 = Convert.ToDouble(((TextBox)fp.inputs.FindName("x1")).Text);
                var y1 = Convert.ToDouble(((TextBox)fp.inputs.FindName("y1")).Text);
                var r = Convert.ToDouble(((TextBox)fp.inputs.FindName("r")).Text);

                Circle.SetValue(Canvas.TopProperty, y1 - r);
                Circle.SetValue(Canvas.LeftProperty, x1 - r);

                Circle.Height = Math.Abs(r * 2);
                Circle.Width = Math.Abs(r * 2);
            }


        }
    }
}
