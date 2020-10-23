using Grafika.Shapes;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace Grafika
{
    /// <summary>
    /// Logika interakcji dla klasy FirstPage.xaml
    /// </summary>
    public partial class FirstPage : Page
    {
        MainWindow mw = (MainWindow)Application.Current.MainWindow;
        public enum Shapes
        {
            Pencil, Line, Circle, Rectangle, Cursor
        }
        public enum HitType
        {
            None, Body, UL, UR, LR, LL, L, R, B, T
        };

        public Shapes currentShape = Shapes.Pencil;
        private IDrawable _dr;
        private readonly myPen _pen;
        Point startPoint;
        public FirstPage()
        {
            InitializeComponent();
            _pen = new myPen(Canvas);
        }
        private void LineButton_Click(object sender, RoutedEventArgs e)
        {
            currentShape = Shapes.Line;
            if (textOrMouse.IsChecked == false)
            {
                clearInputsAndNames();
                SetInputs(Shapes.Line);
            }
        }

        private void ElipseButton_Click(object sender, RoutedEventArgs e)
        {
            currentShape = Shapes.Circle;
            if (textOrMouse.IsChecked == false)
            {
                clearInputsAndNames();
                SetInputs(Shapes.Circle);
            }
        }

        private void RectangleButton_Click(object sender, RoutedEventArgs e)
        {
            currentShape = Shapes.Rectangle;
            if (textOrMouse.IsChecked == false)
            {
                clearInputsAndNames();
                SetInputs(Shapes.Rectangle);
            }
        }
        private void PencilButton_Click(object sender, RoutedEventArgs e)
        {
            currentShape = Shapes.Pencil;
            if (textOrMouse.IsChecked == false)
            {
                clearInputsAndNames();
            }
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.MouseDevice.GetPosition(Canvas);

            if (textOrMouse.IsChecked == true)
            {

                switch (currentShape)
                {
                    case Shapes.Line:
                        _dr = new myLine(Mouse.GetPosition(Canvas));
                        break;
                    case Shapes.Circle:
                        _dr = new myCircle(Mouse.GetPosition(Canvas));
                        break;
                    case Shapes.Rectangle:
                        _dr = new myRectangle(Mouse.GetPosition(Canvas));
                        break;
                    case Shapes.Cursor:
                        break;
                    default:
                        break;
                }

                _pen.Down(_dr);
            }

        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (textOrMouse.IsChecked == true)
            {
                _dr = null;
            }

            DragInProgress = false;

        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.DirectlyOver == Canvas)
            {
                Cursor = Cursors.Arrow;
            }

            if (textOrMouse.IsChecked == true)
            {
                if (_dr != null)
                    _pen.Draw(_dr, e.MouseDevice.GetPosition(Canvas), startPoint);

            }

            if (currentShape == Shapes.Pencil && e.LeftButton == MouseButtonState.Pressed)
                DrawPencil(e.MouseDevice.GetPosition(Canvas));

        }

        private void DrawPencil(Point pos)
        {
            Line line = new Line();

            line.Stroke = SystemColors.WindowFrameBrush;
            line.X1 = startPoint.X;
            line.Y1 = startPoint.Y;
            line.X2 = pos.X;
            line.Y2 = pos.Y;

            startPoint = pos;

            Canvas.Children.Add(line);


        }

        private void ClearCanvas_Click(object sender, RoutedEventArgs e)
        {
            Canvas.Children.Clear();
        }

        private void CursorButton_Click(object sender, RoutedEventArgs e)
        {
            currentShape = Shapes.Cursor;
            if (textOrMouse.IsChecked == false)
            {
                inputs.Children.Clear();
            }
        }

        private void textOrMouse_Checked(object sender, RoutedEventArgs e)
        {

            if (inputs != null)
            {
                clearInputsAndNames();
            }
            if (textOrMouse.IsChecked == false)
            {
                inputs.Children.Clear();
                switch (currentShape)
                {
                    case Shapes.Circle:
                        SetInputs(Shapes.Circle);
                        break;
                    case Shapes.Line:
                        SetInputs(Shapes.Line);
                        break;
                    case Shapes.Rectangle:
                        SetInputs(Shapes.Rectangle);
                        break;
                    default:
                        break;
                }
            }
        }

        public void rysujButton_Click(object sender, RoutedEventArgs e)
        {
            switch (currentShape)
            {
                case Shapes.Line:
                    if (!checkInputs(Shapes.Line))
                    {
                        var newLine = new myLine(new Point(Convert.ToDouble(((TextBox)this.inputs.FindName("x1")).Text), Convert.ToDouble(((TextBox)this.inputs.FindName("y1")).Text)),
                            new Point(Convert.ToDouble(((TextBox)this.inputs.FindName("x2")).Text), Convert.ToDouble(((TextBox)this.inputs.FindName("y2")).Text)));

                        _pen.Down(newLine);
                    }
                    break;
                case Shapes.Rectangle:
                    if (!checkInputs(Shapes.Rectangle))
                    {
                        var newRectangle = new myRectangle(new Point(Convert.ToDouble(((TextBox)this.inputs.FindName("x1")).Text), Convert.ToDouble(((TextBox)this.inputs.FindName("y1")).Text)),
                        new Point(Convert.ToDouble(((TextBox)this.inputs.FindName("x2")).Text), Convert.ToDouble(((TextBox)this.inputs.FindName("y2")).Text)));

                        _pen.Down(newRectangle);
                    }
                    break;
                case Shapes.Circle:
                    if (!checkInputs(Shapes.Circle))
                    {
                        var newCricle = new myCircle(new Point(Convert.ToDouble(((TextBox)this.inputs.FindName("x1")).Text), Convert.ToDouble(((TextBox)this.inputs.FindName("y1")).Text)),
                        Convert.ToDouble(((TextBox)this.inputs.FindName("r")).Text));

                        _pen.Down(newCricle);
                    }
                    break;
                default:
                    break;
            }

        }

        public void SetInputs(Shapes currShape)
        {
            var x1Label = new Label();
            x1Label.Content = "X1";
            x1Label.Margin = new Thickness(50, 0, 0, 0);
            var x1Input = new TextBox();
            x1Input.Width = 200;
            x1Input.Name = "x1";

            var y1Label = new Label();
            y1Label.Content = "Y1";
            y1Label.Margin = new Thickness(50, 0, 0, 0);
            var y1Input = new TextBox();
            y1Input.Width = 200;
            y1Input.Name = "y1";

            inputs.Children.Add(x1Label);
            inputs.Children.Add(x1Input);
            inputs.Children.Add(y1Label);
            inputs.Children.Add(y1Input);

            inputs.RegisterName(x1Input.Name, x1Input);
            inputs.RegisterName(y1Input.Name, y1Input);

            switch (currShape)
            {
                case Shapes.Line:
                case Shapes.Rectangle:

                    var x2Label = new Label();
                    x2Label.Content = "X2";
                    x2Label.Margin = new Thickness(50, 0, 0, 0);
                    var x2Input = new TextBox();
                    x2Input.Width = 200;
                    x2Input.Name = "x2";

                    var y2Label = new Label();
                    y2Label.Content = "Y2";
                    y2Label.Margin = new Thickness(50, 0, 0, 0);
                    var y2Input = new TextBox();
                    y2Input.Width = 200;
                    y2Input.Name = "y2";

                    inputs.Children.Add(x2Label);
                    inputs.Children.Add(x2Input);
                    inputs.Children.Add(y2Label);
                    inputs.Children.Add(y2Input);


                    inputs.RegisterName(x2Input.Name, x2Input);
                    inputs.RegisterName(y2Input.Name, y2Input);
                    break;
                case Shapes.Circle:
                    var rLabel = new Label();
                    rLabel.Content = "R";
                    rLabel.Margin = new Thickness(50, 0, 0, 0);
                    var rInput = new TextBox();
                    rInput.Width = 200;
                    rInput.Name = "r";

                    inputs.Children.Add(rLabel);
                    inputs.Children.Add(rInput);

                    inputs.RegisterName(rInput.Name, rInput);

                    break;
                default:
                    break;
            }

            var actionButton = new Button();
            actionButton.Name = "actionButton";
            actionButton.Content = "Rysuj";
            actionButton.Width = 100;
            actionButton.Margin = new Thickness(0, 15, 0, 0);
            actionButton.Click += rysujButton_Click;

            inputs.Children.Add(actionButton);
            inputs.RegisterName(actionButton.Name, actionButton);
        }

        public void clearInputsAndNames()
        {
            inputs.Children.Clear();
            if (inputs.FindName("x1") != null)
                inputs.UnregisterName("x1");
            if (inputs.FindName("y1") != null)
                inputs.UnregisterName("y1");
            if (inputs.FindName("x2") != null)
                inputs.UnregisterName("x2");
            if (inputs.FindName("y2") != null)
                inputs.UnregisterName("y2");
            if (inputs.FindName("r") != null)
                inputs.UnregisterName("r");
            if (inputs.FindName("actionButton") != null)
                inputs.UnregisterName("actionButton");
        }


        public FirstPage.HitType MouseHitType = FirstPage.HitType.None;
        public bool DragInProgress = false;

        public HitType SetHitType(Shape shape, Point point)
        {
            const double GAP = 10;

            if (shape is Line)
            {
                double x1 = ((Line)shape).X1;
                double y1 = ((Line)shape).Y1;
                double x2 = ((Line)shape).X2;
                double y2 = ((Line)shape).Y2;

                if (point.X < x1 + GAP && point.X >= x1 - GAP
                    && point.Y < y1 + GAP && point.Y >= y1 - GAP)
                    return HitType.T;


                if (point.X < x2 + GAP && point.X >= x2 - GAP
                    && point.Y < y2 + GAP && point.Y >= y2 - GAP)
                    return HitType.B;
            }
            else if (shape is Rectangle)
            {
                double left = Canvas.GetLeft(shape);
                double top = Canvas.GetTop(shape);
                double right = left + shape.Width;
                double bottom = top + shape.Height;

                if (point.X < left) return HitType.None;
                if (point.X > right) return HitType.None;
                if (point.Y < top) return HitType.None;
                if (point.Y > bottom) return HitType.None;

                if (point.X - left < GAP)
                {
                    if (point.Y - top < GAP) return HitType.UL;
                    if (bottom - point.Y < GAP) return HitType.LL;
                    return HitType.L;
                }
                else if (right - point.X < GAP)
                {
                    if (point.Y - top < GAP) return HitType.UR;
                    if (bottom - point.Y < GAP) return HitType.LR;
                    return HitType.R;
                }
                if (point.Y - top < GAP) return HitType.T;
                if (bottom - point.Y < GAP) return HitType.B;
            }
            else if (shape is Ellipse)
            {
                double left = Canvas.GetLeft(shape);
                double top = Canvas.GetTop(shape);
                double right = left + shape.Width;
                double bottom = top + shape.Height;

                if (point.X < left) return HitType.None;
                if (point.X > right) return HitType.None;
                if (point.Y < top) return HitType.None;
                if (point.Y > bottom) return HitType.None;

                if (point.X - left < GAP)
                {
                    return HitType.L;
                }
                else if (right - point.X < GAP)
                {
                    return HitType.R;
                }
                if (point.Y - top < GAP) return HitType.T;
                if (bottom - point.Y < GAP) return HitType.B;
            }
            return HitType.Body;
        }

        public void SetMouseCursor()
        {
            Cursor cursor = Cursors.Arrow;
            switch (MouseHitType)
            {
                case HitType.None:
                    cursor = Cursors.Arrow;
                    break;
                case HitType.Body:
                    cursor = Cursors.ScrollAll;
                    break;
                case HitType.UL:
                case HitType.LR:
                    cursor = Cursors.SizeNWSE;
                    break;
                case HitType.LL:
                case HitType.UR:
                    cursor = Cursors.SizeNESW;
                    break;
                case HitType.T:
                case HitType.B:
                    cursor = Cursors.SizeNS;
                    break;
                case HitType.L:
                case HitType.R:
                    cursor = Cursors.SizeWE;
                    break;
            }

            if (Cursor != cursor) Cursor = cursor;
        }

        private void LoadCanvas_Click(object sender, RoutedEventArgs e)
        {
            List<JsonObject> objects = new List<JsonObject>();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.CheckFileExists = true;
            openFileDialog.ShowDialog();

            if (openFileDialog.FileName.Trim() != string.Empty)
            {
                using (StreamReader r = new StreamReader(openFileDialog.FileName))
                {
                    string jsonString = r.ReadToEnd();
                    objects = JsonConvert.DeserializeObject<List<JsonObject>>(jsonString);
                }
            }
            foreach (var obj in objects)
            {
                if (obj.shapeType.Equals("Line"))
                {
                    var newLine = new myLine(new Point((double)obj.x1, (double)obj.y1), new Point((double)obj.x2, (double)obj.y2));
                    _pen.Down(newLine);
                }
                else if (obj.shapeType.Equals("Rectangle"))
                {
                    var newRectangle = new myRectangle(new Point((double)obj.x1, (double)obj.y1), new Point((double)obj.x1 + (double)obj.width, (double)obj.y1 + (double)obj.height));
                    _pen.Down(newRectangle);
                }
                else if (obj.shapeType.Equals("Ellipse"))
                {
                    var newCricle = new myCircle(new Point((double)obj.x1 + (double)obj.r, (double)obj.y1 + (double)obj.r), (double)obj.r);
                    _pen.Down(newCricle);
                }
            }
        }

        private void SaveCanvas_Click(object sender, RoutedEventArgs e)
        {
            var shapes = Canvas.Children;

            List<JsonObject> objects = new List<JsonObject>();

            foreach (object obj in shapes)
            {
                if (obj is Line)
                {
                    Line line = (Line)obj;
                    JsonObject newObj = new JsonObject()
                    {
                        shapeType = "Line",
                        x1 = line.X1,
                        y1 = line.Y1,
                        x2 = line.X2,
                        y2 = line.Y2
                    };

                    if (!objects.Any(x => x.shapeType.Equals(newObj.shapeType) &&
                                         x.x1 == newObj.x1 &&
                                         x.y1 == newObj.y1 &&
                                         x.x2 == newObj.x2 &&
                                         x.y2 == newObj.y2))
                    {
                        objects.Add(newObj);
                    }

                }
                else if (obj is Rectangle)
                {
                    Rectangle rec = (Rectangle)obj;
                    JsonObject newObj = new JsonObject()
                    {
                        shapeType = "Rectangle",
                        x1 = Canvas.GetLeft(rec),
                        y1 = Canvas.GetTop(rec),
                        width = rec.Width,
                        height = rec.Height
                    };

                    objects.Add(newObj);
                }
                else if (obj is Ellipse)
                {
                    Ellipse eli = (Ellipse)obj;
                    JsonObject newObj = new JsonObject()
                    {
                        shapeType = "Ellipse",
                        x1 = Canvas.GetLeft(eli),
                        y1 = Canvas.GetTop(eli),
                        r = eli.Width / 2
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

        public bool checkInputs(Shapes shape)
        {
            switch (shape)
            {
                case Shapes.Circle:
                    if (String.IsNullOrEmpty(((TextBox)inputs.FindName("x1")).Text) ||
                        String.IsNullOrEmpty(((TextBox)inputs.FindName("y1")).Text) ||
                        String.IsNullOrEmpty(((TextBox)inputs.FindName("r")).Text))
                    {
                        MessageBoxResult result = MessageBox.Show("Aby narysować/zmodyfikować okrąg należy uzupełnić wszystkie pola",
                                          "Error",
                                          MessageBoxButton.OK,
                                          MessageBoxImage.Error);
                        return true;
                    }
                    break;
                case Shapes.Rectangle:
                    if (String.IsNullOrEmpty(((TextBox)inputs.FindName("x1")).Text) ||
                        String.IsNullOrEmpty(((TextBox)inputs.FindName("y1")).Text) ||
                        String.IsNullOrEmpty(((TextBox)inputs.FindName("x2")).Text) ||
                        String.IsNullOrEmpty(((TextBox)inputs.FindName("y2")).Text))
                    {
                        MessageBoxResult result = MessageBox.Show("Aby narysować/zmodyfikować prostokąt należy uzupełnić wszystkie pola",
                                          "Error",
                                          MessageBoxButton.OK,
                                          MessageBoxImage.Error);
                        return true;
                    }
                    break;
                case Shapes.Line:
                    if (String.IsNullOrEmpty(((TextBox)inputs.FindName("x1")).Text) ||
                        String.IsNullOrEmpty(((TextBox)inputs.FindName("y1")).Text) ||
                        String.IsNullOrEmpty(((TextBox)inputs.FindName("x2")).Text) ||
                        String.IsNullOrEmpty(((TextBox)inputs.FindName("y2")).Text))
                    {
                        MessageBoxResult result = MessageBox.Show("Aby narysować/zmodyfikować linię należy uzupełnić wszystkie pola",
                                          "Error",
                                          MessageBoxButton.OK,
                                          MessageBoxImage.Error);
                        return true;
                    }
                    break;
                default:
                    break;
            }
            return false;
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            mw.MainFrame.Content = new SecondPage();
        }
    }
}
