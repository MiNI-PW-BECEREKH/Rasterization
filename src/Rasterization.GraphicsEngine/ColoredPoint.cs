using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rasterization.Engine
{
    public class ColoredPoint 
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Color Color { get; set; } 
        public ColoredPoint(int x, int y, Color color)
        {
            X = x;
            Y = y;
            Color = color;
        }

        public Point AsPoint()
        {
            return new Point(X, Y);
        }
    }

    public static class PointExtensions
    {
        public static ColoredPoint AsColoredPoint(this Point point, Color c)
        {
            return new ColoredPoint(point.X, point.Y, c);
        }
    }
}
