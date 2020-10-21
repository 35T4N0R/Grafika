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

        //private bool drag = false;
        //private Point startPt;
        //private int wid;
        //private int hei;
        //private Point lastLoc;
        //private double CanvasLeft, CanvasTop;
        MainWindow mw = (MainWindow)Application.Current.MainWindow;


        private Point lastPoint;

        private void Circle_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (mw.currentShape == MainWindow.Shapes.Cursor)
            {

                mw.MouseHitType = mw.SetHitType(Circle, Mouse.GetPosition(mw.Canvas));
                mw.SetMouseCursor();
                if (mw.MouseHitType == MainWindow.HitType.None) return;

                lastPoint = e.GetPosition(mw.Canvas);
                mw.DragInProgress = true;


                mw.clearInputsAndNames();
                mw.SetInputs(MainWindow.Shapes.Circle);
                var actionButton = ((Button)mw.inputs.FindName("actionButton"));
                actionButton.Content = "Modyfikuj";
                actionButton.Click -= mw.rysujButton_Click;
                actionButton.Click += modifyCircle;
                ((TextBox)mw.inputs.FindName("x1")).Text = Convert.ToString(Canvas.GetLeft(Circle) + Circle.Width / 2);
                ((TextBox)mw.inputs.FindName("y1")).Text = Convert.ToString(Canvas.GetTop(Circle) + Circle.Width / 2);
                ((TextBox)mw.inputs.FindName("r")).Text = Convert.ToString(Circle.Width / 2);

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
            if (mw.currentShape == MainWindow.Shapes.Cursor)
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

                    if (mw.DragInProgress)
                    {

                        Point point = Mouse.GetPosition(mw.Canvas);

                        if (point.Y > mw.Canvas.ActualHeight || point.X > mw.Canvas.ActualWidth || point.Y < 0 || point.X < 0)
                        {
                            return;
                        }

                        double offset_x = point.X - lastPoint.X;
                        double offset_y = point.Y - lastPoint.Y;

                        double recX = Canvas.GetLeft(Circle);
                        double recY = Canvas.GetTop(Circle);
                        double recWidth = Circle.Width;
                        double recHeight = Circle.Height;

                        switch (mw.MouseHitType)
                        {
                            case MainWindow.HitType.Body:
                                recX += offset_x;
                                recY += offset_y;
                                break;
                            case MainWindow.HitType.UL:
                                recX += offset_x;
                                recY += offset_y;
                                recWidth -= offset_x;
                                recHeight -= offset_y;
                                break;
                            case MainWindow.HitType.UR:
                                recY += offset_y;
                                recWidth += offset_x;
                                recHeight -= offset_y;
                                break;
                            case MainWindow.HitType.LR:
                                recWidth += offset_x;
                                recHeight += offset_y;
                                break;
                            case MainWindow.HitType.LL:
                                recX += offset_x;
                                recWidth -= offset_x;
                                recHeight += offset_y;
                                break;
                            case MainWindow.HitType.L:
                                recX += offset_x;
                                recWidth -= offset_x;
                                break;
                            case MainWindow.HitType.R:
                                recWidth += offset_x;
                                break;
                            case MainWindow.HitType.B:
                                recHeight += offset_y;
                                break;
                            case MainWindow.HitType.T:
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

                        ((TextBox)mw.inputs.FindName("x1")).Text = Convert.ToString(Canvas.GetLeft(Circle) + Circle.Width / 2);
                        ((TextBox)mw.inputs.FindName("y1")).Text = Convert.ToString(Canvas.GetTop(Circle) + Circle.Width / 2);
                        ((TextBox)mw.inputs.FindName("r")).Text = Convert.ToString(Circle.Width / 2);
                    }
                    else
                    {
                        mw.MouseHitType = mw.SetHitType(Circle, Mouse.GetPosition(mw.Canvas));
                        mw.SetMouseCursor();
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
            if (mw.currentShape == MainWindow.Shapes.Cursor)
            {
                mw.DragInProgress = false;
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
            if (!mw.checkInputs(MainWindow.Shapes.Circle))
            {
                var x1 = Convert.ToDouble(((TextBox)mw.inputs.FindName("x1")).Text);
                var y1 = Convert.ToDouble(((TextBox)mw.inputs.FindName("y1")).Text);
                var r = Convert.ToDouble(((TextBox)mw.inputs.FindName("r")).Text);

                Circle.SetValue(Canvas.TopProperty, y1 - r);
                Circle.SetValue(Canvas.LeftProperty, x1 - r);

                Circle.Height = Math.Abs(r * 2);
                Circle.Width = Math.Abs(r * 2);
            }


        }
    }
}
