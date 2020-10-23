using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Grafika.Shapes
{

    public class myRectangle : IDrawable
    {
        public Rectangle Rectangle { get; private set; }
        MainWindow mw = (MainWindow)Application.Current.MainWindow;
        FirstPage fp = (((MainWindow)Application.Current.MainWindow).Content as Frame).Content as FirstPage;

        public myRectangle(Point location)
        {
            Rectangle = new Rectangle
            {
                Stroke = Brushes.Black,
                Fill = Brushes.YellowGreen,
                StrokeThickness = 2,
            };

            Rectangle.MouseMove += new MouseEventHandler(Rectangle_MouseMove);
            Rectangle.MouseUp += new MouseButtonEventHandler(Rectangle_MouseUp);
            Rectangle.MouseLeftButtonDown += new MouseButtonEventHandler(Rectangle_MouseDown);

        }

        public myRectangle(Point topLeft, Point bottomRight)
        {
            Rectangle = new Rectangle
            {
                Stroke = Brushes.Black,
                Fill = Brushes.YellowGreen,
                StrokeThickness = 2,
            };

            if (Rectangle != null)
            {
                double minX = Math.Min(topLeft.X, bottomRight.X);
                double minY = Math.Min(topLeft.Y, bottomRight.Y);
                double maxX = Math.Max(topLeft.X, bottomRight.X);
                double maxY = Math.Max(topLeft.Y, bottomRight.Y);

                Canvas.SetTop(Rectangle, minY);
                Canvas.SetLeft(Rectangle, minX);

                double height = maxY - minY;
                double width = maxX - minX;

                Rectangle.Height = Math.Abs(height);
                Rectangle.Width = Math.Abs(width);
            }

            Rectangle.MouseMove += new MouseEventHandler(Rectangle_MouseMove);
            Rectangle.MouseUp += new MouseButtonEventHandler(Rectangle_MouseUp);
            Rectangle.MouseLeftButtonDown += new MouseButtonEventHandler(Rectangle_MouseDown);

        }

        //private bool drag = false;
        //private Point startPt;
        //private int wid;
        //private int hei;
        //private Point lastLoc;
        //private double CanvasLeft, CanvasTop;

       
        private Point lastPoint;



        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (fp.currentShape == FirstPage.Shapes.Cursor)
            {

                fp.MouseHitType = fp.SetHitType(Rectangle, Mouse.GetPosition(fp.Canvas));
                fp.SetMouseCursor();
                if (fp.MouseHitType == FirstPage.HitType.None) return;

                lastPoint = e.GetPosition(fp.Canvas);
                fp.DragInProgress = true;

                fp.clearInputsAndNames();
                fp.SetInputs(FirstPage.Shapes.Rectangle);
                var actionButton = ((Button)fp.inputs.FindName("actionButton"));
                actionButton.Content = "Modyfikuj";
                actionButton.Click -= fp.rysujButton_Click;
                actionButton.Click += modifyRectangle;
                ((TextBox)fp.inputs.FindName("x1")).Text = Convert.ToString(Canvas.GetLeft(Rectangle));
                ((TextBox)fp.inputs.FindName("y1")).Text = Convert.ToString(Canvas.GetTop(Rectangle));
                ((TextBox)fp.inputs.FindName("x2")).Text = Convert.ToString(Canvas.GetLeft(Rectangle) + Rectangle.Width);
                ((TextBox)fp.inputs.FindName("y2")).Text = Convert.ToString(Canvas.GetTop(Rectangle) + Rectangle.Height);

                //drag = true;
                //startPt = e.GetPosition(mw.Canvas);
                //wid = (int)Rectangle.Width;
                //hei = (int)Rectangle.Height;
                /*lastLoc = new Point(Canvas.GetLeft(Rectangle), Canvas.GetTop(Rectangle));*/
                Mouse.Capture((IInputElement)sender);
            }
        }

        private void Rectangle_MouseMove(object sender, MouseEventArgs e)
        {
            if(fp.currentShape == FirstPage.Shapes.Cursor)
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

                    //    Rectangle.SetValue(Canvas.TopProperty, CanvasTop);
                    //    Rectangle.SetValue(Canvas.LeftProperty, CanvasLeft);





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

                        double recX = Canvas.GetLeft(Rectangle);
                        double recY = Canvas.GetTop(Rectangle);
                        double recWidth = Rectangle.Width;
                        double recHeight = Rectangle.Height;

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
                            Canvas.SetLeft(Rectangle, recX);
                            Canvas.SetTop(Rectangle, recY);

                            Rectangle.Width = recWidth;
                            Rectangle.Height = recHeight;

                            lastPoint = point;
                        }

                        ((TextBox)fp.inputs.FindName("x1")).Text = Convert.ToString(Canvas.GetLeft(Rectangle));
                        ((TextBox)fp.inputs.FindName("y1")).Text = Convert.ToString(Canvas.GetTop(Rectangle));
                        ((TextBox)fp.inputs.FindName("x2")).Text = Convert.ToString(Canvas.GetLeft(Rectangle) + Rectangle.Width);
                        ((TextBox)fp.inputs.FindName("y2")).Text = Convert.ToString(Canvas.GetTop(Rectangle) + Rectangle.Height);
                    }
                    else
                    {
                        fp.MouseHitType = fp.SetHitType(Rectangle, Mouse.GetPosition(fp.Canvas));
                        fp.SetMouseCursor();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

        }

        private void Rectangle_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(fp.currentShape == FirstPage.Shapes.Cursor)
            {
                //drag = false;
                fp.DragInProgress = false;
                Mouse.Capture(null);
            }

        }


        public void Draw(Point location, Point start)
        {
            if (Rectangle != null)
            {
                double minX = Math.Min(location.X, start.X);
                double minY = Math.Min(location.Y, start.Y);
                double maxX = Math.Max(location.X, start.X);
                double maxY = Math.Max(location.Y, start.Y);

                Canvas.SetTop(Rectangle, minY);
                Canvas.SetLeft(Rectangle, minX);

                double height = maxY - minY;
                double width = maxX - minX;

                Rectangle.Height = Math.Abs(height);
                Rectangle.Width = Math.Abs(width);
            }
        }

        private void modifyRectangle(object sender, RoutedEventArgs e)
        {
            if (!fp.checkInputs(FirstPage.Shapes.Rectangle))
            {
                var x1 = Convert.ToDouble(((TextBox)fp.inputs.FindName("x1")).Text);
                var y1 = Convert.ToDouble(((TextBox)fp.inputs.FindName("y1")).Text);
                var x2 = Convert.ToDouble(((TextBox)fp.inputs.FindName("x2")).Text);
                var y2 = Convert.ToDouble(((TextBox)fp.inputs.FindName("y2")).Text);



                if(x1 > x2)
                {
                    var tmp = x1;
                    x1 = x2;
                    x2 = tmp;
                }

                if(y1 > y2)
                {
                    var tmp2 = y1;
                    y1 = y2;
                    y2 = tmp2;
                }


                Rectangle.SetValue(Canvas.TopProperty, y1);
                Rectangle.SetValue(Canvas.LeftProperty, x1);
                Rectangle.Height = Math.Abs(y2 - y1);
                Rectangle.Width = Math.Abs(x2 - x1);
            }



        }
    }

}
