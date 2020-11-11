using System.Drawing;
using System.Windows.Forms;
using ZedGraph;

namespace HistSomething
{
    public partial class UserControl1: UserControl
    {
        public UserControl1(int[] values)
        {
            InitializeComponent();
            CreateGraph(zed, values);
            SetSize();
        }

        public new int Width
        {
            get { return zed.Width; }
            set { zed.Width = value; }
        }
        public new int Height
        {
            get { return zed.Height; }
            set { zed.Height = value; }
        }
        private void CreateGraph(ZedGraphControl zed, int[] values)
        {
            // get a reference to the GraphPane
            GraphPane myPane = zed.GraphPane;

            // Set the Titles
            myPane.Title.Text = "Histogram";
            myPane.XAxis.Title.Text = "X Axis";
            myPane.YAxis.Title.Text = "Y Axis";
            myPane.XAxis.Scale.MajorStep = 5;

            // Make up some data arrays based on the Sine function
            double x;
            PointPairList list = new PointPairList();
            for (int i = 0; i < values.Length; i++)
            {
                x = (double)i;
                list.Add(x, values[i]);
            }

            // Generate a red curve with diamond
            // symbols, and "Porsche" in the legend
            BarItem myBar = myPane.AddBar(null,list, Color.Black);

            myBar.Bar.Border.IsVisible = false;

            // Tell ZedGraph to refigure the
            // axes since the data have changed
            zed.AxisChange();
        }

        private void SetSize()
        {
            zed.Location = new Point(10, 10);

            // Leave a small margin around the outside of the control
            zed.Size = new Size((int)this.Width - 20,
                                    (int)this.Height - 20);
        }
    }
}
