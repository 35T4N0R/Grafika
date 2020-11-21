using System.Windows;
using System.Windows.Controls;

namespace Grafika.Shapes
{
    public class myPen
    {
        public myPen(Canvas holder)
        {
            _holder = holder;
        }

        private readonly Canvas _holder;

        public void Down(IDrawable obj)
        {
            if (obj is myCircle)
                _holder.Children.Add((obj as myCircle).Circle);
            if (obj is myLine)
                _holder.Children.Add((obj as myLine).Line);
            if (obj is myRectangle)
                _holder.Children.Add((obj as myRectangle).Rectangle);
            if (obj is CurvePoint)
                _holder.Children.Add((obj as CurvePoint).Circle);

        }

        public void Draw(IDrawable obj, Point location, Point start)
        {
            obj.Draw(location, start);
        }
    }
}
