using System;
using System.Drawing;

namespace Rasterization.Engine
{
    public class FilledCircle : Circle
    {
        public FilledCircle(Point center, Point radiusIndicator, Color color) : base(center, radiusIndicator, color)
        {
        }

        public override void CalculatePoints()
        {
            Points.Clear();

            for(int x = -Radius; x <= Radius; x++)
            {
                int height = (int)Math.Sqrt(Radius * Radius - x * x);

                for (int y = -height; y < height; y++)
                {
                    Points.Add(new Point(x + Center.X, y + Center.Y));
                }
            }
        }
    }
}
