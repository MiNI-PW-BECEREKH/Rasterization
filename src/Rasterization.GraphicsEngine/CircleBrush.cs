using System;
using System.Collections.Generic;
using System.Drawing;

namespace Rasterization.Engine
{
    public class CircleBrush : ICircle
    {
        public CircleBrush(Point center, int radiusIndicator, Color color) 
        {
            Center = center;
            Radius = radiusIndicator;
            Color = color;
        }

        public int Radius { get ; set; }
        public Point Center { get; set ; }
        public string Name { get; set ; }
        public Color Color { get ; set; }
        public List<Point> Points { get; set; } = new();
        public List<Point> BasePoints { get ; set; }
        public List<Point> StretchablePoints { get ; set ; }

        public void CalculatePoints(List<Point> points)
        {
            Points.Clear();

            foreach (var point in points)
            {

                int r2 = Radius * Radius;
                int area = r2 << 2;
                int rr = Radius << 1;

                for (int i = 0; i < area; i++)
                {
                    int tx = (i % rr) - Radius;
                    int ty = (i / rr) - Radius;

                    if (tx * tx + ty * ty <= r2 && !Points.Contains(new Point(point.X + tx, point.Y + ty)))
                        Points.Add(new Point(point.X + tx, point.Y + ty));
                }

            }

        }
    }
}
