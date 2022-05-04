using System;
using System.Collections.Generic;
using System.Drawing;

namespace Rasterization.Engine
{
    public class FilledCircle : Circle
    {
        public FilledCircle(Point center, Point radiusIndicator, Color color) : base(center, radiusIndicator, color)
        {
        }

        public void CalculatePoints(List<Point> points)
        {
            Points.Clear();

            foreach (var point in points)
            {
                //for(int x = -Radius; x <= Radius; x++)
                //{
                //    int height = (int)Math.Sqrt(Radius * Radius - x * x);

                //    for (int y = -height; y < height; y++)
                //    {
                //        Points.Add(new Point(x + point.X, y + point.Y));
                //    }
                //}
                int r2 = Radius * Radius;
                int area = r2 << 2;
                int rr = Radius << 1;

                for (int i = 0; i < area; i++)
                {
                    int tx = (i % rr) - Radius;
                    int ty = (i / rr) - Radius;

                    if (tx * tx + ty * ty <= r2 && !Points.Contains(new Point(point.X + tx, point.Y + ty)))
                        Points.Add(new Point(point.X + tx, point.Y + ty));
                        //SetPixel(x + tx, y + ty, c);
                }

            }

        }
    }
}
