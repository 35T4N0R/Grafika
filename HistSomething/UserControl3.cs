using System.Drawing;
using System.Windows.Forms;
using ZedGraph;

namespace HistSomething
{
    public partial class UserControl3 : UserControl
    {
        public UserControl3(int[] values_r, int[] values_g, int[] values_b)
        {
            InitializeComponent();
            CreateGraph(zed1, zed2, zed3, values_r, values_g, values_b);
            SetSize();
        }
        public new int Width
        {
            get { return zed1.Width + zed2.Width; }
            set { zed1.Width = value; }
        }
        public new int Height
        {
            get { return zed1.Height + zed3.Height; }
            set { zed1.Height = value; }
        }
        private void CreateGraph(ZedGraphControl zed1, ZedGraphControl zed2, ZedGraphControl zed3, int[] values_r, int[] values_g, int[] values_b)
        {
            // get a reference to the GraphPane
            GraphPane myPane1 = zed1.GraphPane;
            GraphPane myPane2 = zed2.GraphPane;
            GraphPane myPane3 = zed3.GraphPane;

            // Set the Titles
            myPane1.Title.Text = "Histogram R";
            myPane1.XAxis.Title.Text = "X Axis";
            myPane1.YAxis.Title.Text = "Y Axis";
            myPane1.XAxis.Scale.MajorStep = 5;

            myPane2.Title.Text = "Histogram G";
            myPane2.XAxis.Title.Text = "X Axis";
            myPane2.YAxis.Title.Text = "Y Axis";
            myPane2.XAxis.Scale.MajorStep = 5;

            myPane3.Title.Text = "Histogram B";
            myPane3.XAxis.Title.Text = "X Axis";
            myPane3.YAxis.Title.Text = "Y Axis";
            myPane3.XAxis.Scale.MajorStep = 5;

            // Make up some data arrays based on the Sine function
            double x;
            PointPairList list_r = new PointPairList();
            PointPairList list_g = new PointPairList();
            PointPairList list_b = new PointPairList();
            for (int i = 0; i < values_r.Length; i++)
            {
                x = (double)i;
                list_r.Add(x, values_r[i]);
                list_g.Add(x, values_g[i]);
                list_b.Add(x, values_b[i]);
            }

            // Generate a red curve with diamond
            // symbols, and "Porsche" in the legend
            BarItem myBar1 = myPane1.AddBar(null, list_r, Color.Red);

            myBar1.Bar.Border.IsVisible = false;

            BarItem myBar2 = myPane2.AddBar(null, list_g, Color.Green);

            myBar2.Bar.Border.IsVisible = false;

            BarItem myBar3 = myPane3.AddBar(null, list_b, Color.Blue);

            myBar3.Bar.Border.IsVisible = false;

            // Tell ZedGraph to refigure the
            // axes since the data have changed
            zed1.AxisChange();
            zed2.AxisChange();
            zed3.AxisChange();
        }

        private void SetSize()
        {
            zed1.Location = new Point(10, 10);
            zed2.Location = new Point(zed1.Width + 20, 10);
            zed3.Location = new Point(10, zed1.Height + 20);

            // Leave a small margin around the outside of the control
            //zed1.Size = new Size((int)this.Width - 20, (int)this.Height - 20);
            //zed2.Size = new Size((int)this.Width - 20, (int)this.Height - 20);
            //zed3.Size = new Size((int)this.Width - 20, (int)this.Height - 20);
        }

    }
}
