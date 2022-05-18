using System;
using System.Media;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Rasterization.Engine
{
    public class Line : IDrawable, ILine
    {
        public Point StartingPoint { get; set; }
        public Point EndingPoint { get; set; }

        public Color Color { get ; set; } = Color.Black;
        [XmlIgnore]
        public CircleBrush Brush { get ; set ; }
        public List<ColoredPoint> Points { get; set; } = new();
        public List<Point> StretchablePoints { get; set; } = new();
        public List<Point> BasePoints { get; set; } = new ();
        public bool IsAntiAliased { get ; set ; } = false;
        public string Name { get; set; } = "Line";
        public Color GrabbedCollor { get; set ; }

        public Line()
        {

        }
        public Line(Point sp, Point ep,Color color, int bs)
        {
            StartingPoint = sp;
            EndingPoint = ep;
            Color = color;
            GrabbedCollor = Color.FromArgb(128, Color.R, Color.G, Color.B);
            Brush = new CircleBrush(new Point(0,0),bs,Color);

            CalculateBrush();

            StretchablePoints.Add(new Point(sp.X,sp.Y));
            StretchablePoints.Add(new Point(ep.X, ep.Y));

        }

        public void CalculatePoints()
        {
            Points.Clear();
            BasePoints.Clear();
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

            CalculateBrush();
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
                BasePoints.Add(new Point(x, y));
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
                BasePoints.Add(new Point(x,y));
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
            if(IsAntiAliased)
            {
                engine.DrawAALine(this);
            }
            else
            engine.Draw(this);
        }

        public bool HitTest(Point p)
        {
            return Points.Any(point => Math.Sqrt(Math.Pow(p.X - point.X, 2) + Math.Pow(p.Y - point.Y, 2)) < 10);
        }

        public void Erase(IGraphicsEngine engine)
        {
            engine.Erase(this);
        }

        public void IndicateSelection(IGraphicsEngine engine)
        {
            if (IsAntiAliased)
            {
                engine.DrawAALine(this);
            }
            else
                engine.Transparent(this);
        }

        public void Stretch(IGraphicsEngine engine, int dx, int dy, int idx)
        {
            var p = StretchablePoints[idx];
            StretchablePoints.Remove(p);
            StretchablePoints.Insert(idx,new Point(p.X + dx , p.Y + dy));
            CalculatePoints();
            if (IsAntiAliased)
            {
                engine.DrawAALine(this);
            }
            else
                engine.Stretch(this);
        }

        public void Move(IGraphicsEngine engine, int dx, int dy, int idx)
        {
            var p = StretchablePoints[0];
            StretchablePoints.RemoveAt(0);
            StretchablePoints.Insert(0,new Point(p.X + dx ,  p.Y +  dy ));
            p = StretchablePoints[1];
            StretchablePoints.RemoveAt(1);
            StretchablePoints.Insert(1, new Point(p.X + dx,  p.Y + dy ));
            CalculatePoints();
            if (IsAntiAliased)
            {
                engine.DrawAALine(this);
            }
            else
                engine.Move(this);
        }

        public void CalculateBrush()
        {
            Brush.CalculatePoints(BasePoints);
            Points.AddRange(Brush.Points.Select(p => p.AsColoredPoint(Color)));
            //Points.AddRange(BasePoints.Select(p => p.AsColoredPoint(Color)));
        }

        public void DrawAA(IGraphicsEngine engine)
        {
            engine.DrawAALine(this);
        }

        public void UpScale(IGraphicsEngine engine)
        {
            Erase(engine);
            Brush.Radius = 2 * Brush.Radius;
            StretchablePoints[0] = new Point(StretchablePoints[0].X * 2, StretchablePoints[0].Y * 2);
            StretchablePoints[1] = new Point(StretchablePoints[1].X * 2, StretchablePoints[1].Y * 2);
            CalculatePoints();
            Draw(engine);




        }

        public void CalculateAntiAliased(IGraphicsEngine engine)
        {
            throw new NotImplementedException();
        }
    }
}
