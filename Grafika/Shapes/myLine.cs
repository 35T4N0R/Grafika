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



        private void Line_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (mw.currentShape == MainWindow.Shapes.Cursor)
            {
                mw.MouseHitType = mw.SetHitType(Line, Mouse.GetPosition(mw.Canvas));
                mw.SetMouseCursor();
                if (mw.MouseHitType == MainWindow.HitType.None) return;

                mw.DragInProgress = true;

                mw.clearInputsAndNames();
                mw.SetInputs(MainWindow.Shapes.Line);
                var actionButton = ((Button)mw.inputs.FindName("actionButton"));
                actionButton.Content = "Modyfikuj";
                actionButton.Click -= mw.rysujButton_Click;
                actionButton.Click += modifyLine;
                ((TextBox)mw.inputs.FindName("x1")).Text = Convert.ToString(Line.X1);
                ((TextBox)mw.inputs.FindName("y1")).Text = Convert.ToString(Line.Y1);
                ((TextBox)mw.inputs.FindName("x2")).Text = Convert.ToString(Line.X2);
                ((TextBox)mw.inputs.FindName("y2")).Text = Convert.ToString(Line.Y2);

                drag = true;
                startPt = e.MouseDevice.GetPosition(mw.Canvas);
                //wid = (int)Line.Width;
                //hei = (int)Line.Height;
                //lastLoc = new Point(Line.X1, Line.Y1);
                Mouse.Capture((IInputElement)sender);
            }
        }

        private void Line_MouseMove(object sender, MouseEventArgs e)
        {
            if (mw.currentShape == MainWindow.Shapes.Cursor)
            {
                try
                {
                    if (drag)
                    {
                        Point newPos = e.MouseDevice.GetPosition(mw.Canvas);

                        if (newPos.Y >= mw.Canvas.ActualHeight || newPos.Y < 0 || newPos.X >= mw.Canvas.ActualWidth || newPos.X < 0)
                        {
                            return;
                        }

                        switch (mw.MouseHitType)
                        {
                            case MainWindow.HitType.Body:
                                Line.X1 += newPos.X - startPt.X;
                                Line.X2 += newPos.X - startPt.X;
                                Line.Y1 += newPos.Y - startPt.Y;
                                Line.Y2 += newPos.Y - startPt.Y;
                                startPt = newPos;
                                break;
                            case MainWindow.HitType.T:
                                Line.X1 += newPos.X - startPt.X;
                                Line.Y1 += newPos.Y - startPt.Y;
                                startPt = newPos;
                                break;
                            case MainWindow.HitType.B:
                                Line.X2 += newPos.X - startPt.X;
                                Line.Y2 += newPos.Y - startPt.Y;
                                startPt = newPos;
                                break;
                            default:
                                break;
                        }

                        

                        ((TextBox)mw.inputs.FindName("x1")).Text = Convert.ToString(Line.X1);
                        ((TextBox)mw.inputs.FindName("y1")).Text = Convert.ToString(Line.Y1);
                        ((TextBox)mw.inputs.FindName("x2")).Text = Convert.ToString(Line.X2);
                        ((TextBox)mw.inputs.FindName("y2")).Text = Convert.ToString(Line.Y2);

                    }
                    else
                    {
                        mw.MouseHitType = mw.SetHitType(Line, Mouse.GetPosition(mw.Canvas));
                        mw.SetMouseCursor();
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
            if (mw.currentShape == MainWindow.Shapes.Cursor)
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
            if (!mw.checkInputs(MainWindow.Shapes.Line))
            {
                var x1 = Convert.ToDouble(((TextBox)mw.inputs.FindName("x1")).Text);
                var y1 = Convert.ToDouble(((TextBox)mw.inputs.FindName("y1")).Text);
                var x2 = Convert.ToDouble(((TextBox)mw.inputs.FindName("x2")).Text);
                var y2 = Convert.ToDouble(((TextBox)mw.inputs.FindName("y2")).Text);

                Line.X1 = x1;
                Line.Y1 = y1;
                Line.X2 = x2;
                Line.Y2 = y2;
            }


        }

    }
}
