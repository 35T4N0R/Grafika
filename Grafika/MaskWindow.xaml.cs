using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Grafika
{
    /// <summary>
    /// Logika interakcji dla klasy MaskWindow.xaml
    /// </summary>
    public partial class MaskWindow : Window
    {
        List<TextBox> list = new List<TextBox>();
        public int[,] mask = null;

        private int boxNumber;

        public MaskWindow(int _boxNumber)
        {
            boxNumber = _boxNumber;
            InitializeComponent();
            PrintInputs();
        }

        public void PrintInputs()
        {
            for (int i = 0; i < boxNumber; i++)
            {
                var stackPanel = new StackPanel();
                stackPanel.Orientation = Orientation.Horizontal;
                stackPanel.Margin = new Thickness(0, 10, 0, 0);
                for (int j = 0; j < boxNumber; j++)
                {
                    var textBox = new TextBox();
                    textBox.Width = 25;
                    textBox.Height = 20;
                    textBox.Margin = new Thickness(10, 10, 10, 10);

                    stackPanel.Children.Add(textBox);
                    list.Add(textBox);
                }

                boxes.Children.Add(stackPanel);
            }
        }

        private void SaveMaskButton_Click(object sender, RoutedEventArgs e)
        {
            mask = new int[boxNumber,boxNumber];
            var iter = 0;
            for (int i = 0; i < mask.GetLength(0); i++)
            {
                for (int j = 0; j < mask.GetLength(1); j++)
                {
                    mask[i, j] = Convert.ToInt32(list[iter++].Text);
                }
            }

            this.DialogResult = true;
            Close();
        }
    }
}
