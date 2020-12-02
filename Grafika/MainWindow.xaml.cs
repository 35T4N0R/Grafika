using System.Windows;

namespace Grafika
{

    public interface IDrawable
    {
        void Draw(Point location, Point start);
    }

    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Content = new SeventhPage();
        }
        
    }

}
