using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Grafika.Shapes
{
    public class myLine : IDrawable
    {
        public Line Line { get; private set; }

        public myLine(Point location)
        {
            Line = new Line
            {
                Stroke = Brushes.Blue,
                StrokeThickness = 2,
                X1 = location.X,
                X2 = location.X,
                Y1 = location.Y,
                Y2 = location.Y
            };

            Line.MouseMove += new MouseEventHandler(Line_MouseMove);
            Line.MouseUp += new MouseButtonEventHandler(Line_MouseUp);
            Line.MouseLeftButtonDown += new MouseButtonEventHandler(Line_MouseDown);
        }

        public myLine(Point start, Point end)
        {
            Line = new Line
            {
                Stroke = Brushes.Red,
                StrokeThickness = 3,
                X1 = start.X,
                X2 = end.X,
                Y1 = start.Y,
                Y2 = end.Y
            };

            Line.MouseMove += new MouseEventHandler(Line_MouseMove);
            Line.MouseUp += new MouseButtonEventHandler(Line_MouseUp);
            Line.MouseLeftButtonDown += new MouseButtonEventHandler(Line_MouseDown);
        }

        private bool drag = false;
        private Point startPt;
        MainWindow mw = (MainWindow)Application.Current.MainWindow;
        FirstPage fp = (((MainWindow)Application.Current.MainWindow).Content as Frame).Content as FirstPage;


        private void Line_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (fp.currentShape == FirstPage.Shapes.Cursor)
            {
                fp.MouseHitType = fp.SetHitType(Line, Mouse.GetPosition(fp.Canvas));
                fp.SetMouseCursor();
                if (fp.MouseHitType == FirstPage.HitType.None) return;

                fp.DragInProgress = true;

                fp.clearInputsAndNames();
                fp.SetInputs(FirstPage.Shapes.Line);
                var actionButton = ((Button)fp.inputs.FindName("actionButton"));
                actionButton.Content = "Modyfikuj";
                actionButton.Click -= fp.rysujButton_Click;
                actionButton.Click += modifyLine;
                ((TextBox)fp.inputs.FindName("x1")).Text = Convert.ToString(Line.X1);
                ((TextBox)fp.inputs.FindName("y1")).Text = Convert.ToString(Line.Y1);
                ((TextBox)fp.inputs.FindName("x2")).Text = Convert.ToString(Line.X2);
                ((TextBox)fp.inputs.FindName("y2")).Text = Convert.ToString(Line.Y2);

                drag = true;
                startPt = e.MouseDevice.GetPosition(fp.Canvas);
                //wid = (int)Line.Width;
                //hei = (int)Line.Height;
                //lastLoc = new Point(Line.X1, Line.Y1);
                Mouse.Capture((IInputElement)sender);
            }
        }

        private void Line_MouseMove(object sender, MouseEventArgs e)
        {
            if (fp.currentShape == FirstPage.Shapes.Cursor)
            {
                try
                {
                    if (drag)
                    {
                        Point newPos = e.MouseDevice.GetPosition(fp.Canvas);

                        if (newPos.Y >= fp.Canvas.ActualHeight || newPos.Y < 0 || newPos.X >= fp.Canvas.ActualWidth || newPos.X < 0)
                        {
                            return;
                        }

                        switch (fp.MouseHitType)
                        {
                            case FirstPage.HitType.Body:
                                Line.X1 += newPos.X - startPt.X;
                                Line.X2 += newPos.X - startPt.X;
                                Line.Y1 += newPos.Y - startPt.Y;
                                Line.Y2 += newPos.Y - startPt.Y;
                                startPt = newPos;
                                break;
                            case FirstPage.HitType.T:
                                Line.X1 += newPos.X - startPt.X;
                                Line.Y1 += newPos.Y - startPt.Y;
                                startPt = newPos;
                                break;
                            case FirstPage.HitType.B:
                                Line.X2 += newPos.X - startPt.X;
                                Line.Y2 += newPos.Y - startPt.Y;
                                startPt = newPos;
                                break;
                            default:
                                break;
                        }

                        

                        ((TextBox)fp.inputs.FindName("x1")).Text = Convert.ToString(Line.X1);
                        ((TextBox)fp.inputs.FindName("y1")).Text = Convert.ToString(Line.Y1);
                        ((TextBox)fp.inputs.FindName("x2")).Text = Convert.ToString(Line.X2);
                        ((TextBox)fp.inputs.FindName("y2")).Text = Convert.ToString(Line.Y2);

                    }
                    else
                    {
                        fp.MouseHitType = fp.SetHitType(Line, Mouse.GetPosition(fp.Canvas));
                        fp.SetMouseCursor();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

        }

        private void Line_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (fp.currentShape == FirstPage.Shapes.Cursor)
            {
                drag = false;
                Mouse.Capture(null);
            }
        }

        public void Draw(Point location, Point start)
        {
            Line.X2 = location.X;
            Line.Y2 = location.Y;
        }

        private void modifyLine(object sender, RoutedEventArgs e)
        {
            if (!fp.checkInputs(FirstPage.Shapes.Line))
            {
                var x1 = Convert.ToDouble(((TextBox)fp.inputs.FindName("x1")).Text);
                var y1 = Convert.ToDouble(((TextBox)fp.inputs.FindName("y1")).Text);
                var x2 = Convert.ToDouble(((TextBox)fp.inputs.FindName("x2")).Text);
                var y2 = Convert.ToDouble(((TextBox)fp.inputs.FindName("y2")).Text);

                Line.X1 = x1;
                Line.Y1 = y1;
                Line.X2 = x2;
                Line.Y2 = y2;
            }


        }

    }
}
