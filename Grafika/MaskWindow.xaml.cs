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
        public MaskWindow()
        {
            InitializeComponent();
            PrintInputs(7);
        }

        public void PrintInputs(int number)
        {
            var list = new List<TextBox>();
            for (int i = 0; i < number; i++)
            {
                var stackPanel = new StackPanel();
                stackPanel.Orientation = Orientation.Horizontal;
                stackPanel.Margin = new Thickness(0, 10, 0, 0);
                for (int j = 0; j < number; j++)
                {
                    var textBox = new TextBox();
                    textBox.Width = 50;
                    textBox.Height = 50;
                    textBox.Margin = new Thickness(10, 10, 10, 10);

                    stackPanel.Children.Add(textBox);
                    list.Add(textBox);
                }

                boxes.Children.Add(stackPanel);
            }
        }
    }
}
