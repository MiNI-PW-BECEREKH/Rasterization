using System;
using System.Media;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace Rasterization.Engine
{
    public class Line : IDrawable
    {
        public Point StartingPoint { get; set; }
        public Point EndingPoint { get; set; }

        public List<Point> Points { get; set; } = new();
        public Color Color { get ; set; }
        public List<Point> StretchablePoints { get; set; } = new();

        public Line(Point sp, Point ep,Color color)
        {
            StartingPoint = sp;
            EndingPoint = ep;
            Color = color;

            StretchablePoints.Add(sp);
            StretchablePoints.Add(ep);

        }

        public void CalculatePoints()
        {
            Points.Clear();
            //https://en.wikipedia.org/wiki/Bresenham's_line_algorithm
            int x1 = StretchablePoints[0].X;
            int y1 = StretchablePoints[0].Y;
            int x2 = StretchablePoints[1].X;
            int y2 = StretchablePoints[1].Y;

            if( Math.Abs(y2 -y1) < Math.Abs(x2 -x1))
            {
                if(x1 > x2)
                {
                    CalculateLowPoints(x2, y2, x1, y1);
                }
                else
                    CalculateLowPoints(x1, y1, x2, y2);
            }
            else
            {
                if( y1 > y2)
                {
                    CalculateHighPoints(x2,y2, x1, y1);
                }
                else
                    CalculateHighPoints(x1, y1, x2, y2);

            }
        }

        void CalculateLowPoints(int x1, int y1, int x2, int y2)
        {
            var dx = x2 - x1;
            var dy = y2 - y1;
            var yi = 1;
            if(dy < 0)
            {
                yi = -1;
                dy = -dy;
            }
            var  d = 2 *dy - dx;
            var y = y1;

            foreach(var x in Enumerable.Range(Math.Min(x1,x2),Math.Abs(x1 - x2)))
            {
                Points.Add(new Point(x, y));
                if (d > 0)
                {
                    y = y + yi;
                    d = d + (2 * dy - 2 * dx);
                }
                else
                    d = d + 2 * dy;

            }
        }

        void CalculateHighPoints(int x1, int y1, int x2, int y2)
        {
            var dx = x2 - x1;
            var dy = y2 - y1;

            var xi = 1;
            if(dx < 0)
            {
                xi = -1;
                dx = -dx;
            }
            var d = 2 * dx - dy;
            var x = x1;

            foreach (var y in Enumerable.Range(Math.Min(y1, y2), Math.Abs(y1 - y2)))
            {
                Points.Add(new Point(x,y));
                if (d > 0)
                {
                    x = x + xi;
                    d = d + (2 * dx - 2* dy);
                }
                else
                    d = d + 2 * dx;
            }

        }

        public void Draw(IGraphicsEngine engine)
        {
            engine.Draw(this);
        }

        public bool HitTest(Point p)
        {
            return Points.Contains(p);
        }

        public void Erase(IGraphicsEngine engine)
        {
            engine.Erase(this);
        }

        public void IndicateSelection(IGraphicsEngine engine)
        {
            engine.Transparent(this);
        }

        public void Stretch(IGraphicsEngine engine, int dx, int dy, int idx)
        {
            var p = StretchablePoints[idx];
            StretchablePoints.Remove(p);
            StretchablePoints.Insert(idx,new Point(p.X + dx , p.Y + dy ));
            CalculatePoints();
            engine.Stretch(this);
        }

        public void Move(IGraphicsEngine engine, int dx, int dy, int idx)
        {
            //int sign = 1;
            //if (dx < 0 || dy < 0)
            //    sign = -sign;
            //var delta =  (int)Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
            //for (int i = 0; i < StretchablePoints.Count; i++)
            //{
            //    var p = StretchablePoints[i];
            //    StretchablePoints.RemoveAt(i);
            //    StretchablePoints.Add(new Point(p.X + Math.Sign(dx)*dx, p.Y + Math.Sign(dy)*dy));
            //}
            //CalculatePoints();
            //engine.Move(this);
            throw new NotImplementedException();
        }
    }
}
