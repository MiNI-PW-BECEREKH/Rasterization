using System;
using System.Media;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Numerics;
using System.Windows.Media.Imaging;

namespace Rasterization.Engine
{
    public class Line : IDrawable, ILine
    {
        public Point StartingPoint { get; set; }
        public Point EndingPoint { get; set; }

        public Color Color { get; set; } = Color.Black;
        [XmlIgnore]
        public CircleBrush Brush { get; set; }
        public List<ColoredPoint> Points { get; set; } = new();
        public List<Point> StretchablePoints { get; set; } = new();
        public List<Point> BasePoints { get; set; } = new();
        public bool IsAntiAliased { get; set; } = false;
        public string Name { get; set; } = "Line";
        public Color GrabbedCollor { get; set; }

        public Point NormalToOutside { get; set; }
        public bool IsFilled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Color fillColor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsFilledImage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public WriteableBitmap FillBitmap { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Line()
        {

        }
        public Line(Point sp, Point ep, Color color, int bs)
        {
            StartingPoint = sp;
            EndingPoint = ep;
            Color = color;
            GrabbedCollor = Color.FromArgb(128, Color.R, Color.G, Color.B);
            Brush = new CircleBrush(new Point(0, 0), bs, Color);

            if(Brush.Radius > 0)
            CalculateBrush();

            StretchablePoints.Add(new Point(sp.X, sp.Y));
            StretchablePoints.Add(new Point(ep.X, ep.Y));


            CalculateNormal();

        }

        public void CalculateNormal()
        {
            var dx = EndingPoint.X - StartingPoint.X;
            var dy = EndingPoint.Y - StartingPoint.Y;

            NormalToOutside = new Point(dx, dy);
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

            if (Math.Abs(y2 - y1) < Math.Abs(x2 - x1))
            {
                if (x1 > x2)
                {
                    CalculateLowPoints(x2, y2, x1, y1);
                }
                else
                    CalculateLowPoints(x1, y1, x2, y2);
            }
            else
            {
                if (y1 > y2)
                {
                    CalculateHighPoints(x2, y2, x1, y1);
                }
                else
                    CalculateHighPoints(x1, y1, x2, y2);

            }

            if(Brush.Radius > 0)
                CalculateBrush();
            Points.AddRange(BasePoints.Select(p => p.AsColoredPoint(Color)));
        }

        void CalculateLowPoints(int x1, int y1, int x2, int y2)
        {
            var dx = x2 - x1;
            var dy = y2 - y1;
            var yi = 1;
            if (dy < 0)
            {
                yi = -1;
                dy = -dy;
            }
            var d = 2 * dy - dx;
            var y = y1;

            foreach (var x in Enumerable.Range(Math.Min(x1, x2), Math.Abs(x1 - x2)))
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
            if (dx < 0)
            {
                xi = -1;
                dx = -dx;
            }
            var d = 2 * dx - dy;
            var x = x1;

            foreach (var y in Enumerable.Range(Math.Min(y1, y2), Math.Abs(y1 - y2)))
            {
                BasePoints.Add(new Point(x, y));
                if (d > 0)
                {
                    x = x + xi;
                    d = d + (2 * dx - 2 * dy);
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
            StretchablePoints.Insert(idx, new Point(p.X + dx, p.Y + dy));
            CalculatePoints();
            if (IsAntiAliased)
            {
                engine.DrawAALine(this);
            }
            else
                engine.Stretch(this);

            StartingPoint = StretchablePoints[0];
            EndingPoint = StretchablePoints[1];

            CalculateNormal();
        }

        public void Move(IGraphicsEngine engine, int dx, int dy, int idx)
        {
            var p = StretchablePoints[0];
            StretchablePoints.RemoveAt(0);
            StretchablePoints.Insert(0, new Point(p.X + dx, p.Y + dy));
            p = StretchablePoints[1];
            StretchablePoints.RemoveAt(1);
            StretchablePoints.Insert(1, new Point(p.X + dx, p.Y + dy));
            
            if (IsAntiAliased)
            {
                CalculateAntiAliased(engine);
            }
            else
                CalculatePoints();
            engine.Move(this);

            StartingPoint = StretchablePoints[0];
            EndingPoint = StretchablePoints[1];

            CalculateNormal();
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
            BasePoints.Clear();
            Points.Clear();
            int x1 = this.StretchablePoints[0].X;
            int y1 = this.StretchablePoints[0].Y;
            int x2 = this.StretchablePoints[1].X;
            int y2 = this.StretchablePoints[1].Y;

            bool steep = Math.Abs(y2 - y1) > Math.Abs(x2 - x1);

            if (steep)
            {
                (x1, y1) = (y1, x1);
                (x2, y2) = (y2, x2);
            }

            if (x1 > x2)
            {
                (x1, x2) = (x2, x1);
                (y1, y2) = (y2, y1);
            }

            float dx = x2 - x1;
            float dy = y2 - y1;

            float gradient = dy / dx;

            if (dx == 0) gradient = 1;

            int xPixel1 = x1;
            int xPixel2 = x2;

            float intersectY = y1;

            Color L = this.Color;
            Color B = Color.FromArgb(250, 250, 250);

            if (steep)
            {
                for (int x = xPixel1; x <= xPixel2; x++)
                {


                    Points.Add(new ColoredPoint(iPartOfNumber(intersectY), x, C2(B, L, intersectY)));
                    Points.Add(new ColoredPoint(iPartOfNumber(intersectY) - 1, x, C1(B, L, intersectY)));
                    intersectY += gradient;
                }
            }
            else
            {
                for (int x = xPixel1; x <= xPixel2; x++)
                {
                    Points.Add(new ColoredPoint(x, iPartOfNumber(intersectY), C2(B, L, intersectY)));
                    Points.Add(new ColoredPoint(x, iPartOfNumber(intersectY) - 1, C1(B, L, intersectY)));
                    intersectY += gradient;
                }
            }

            //engine.Draw(this);
            Draw(engine);
        }

        int iPartOfNumber(float x)
        {
            return (int)x;
        }

        //returns fractional part of a number 
        float fPartOfNumber(float x)
        {
            if (x > 0) return x - iPartOfNumber(x);
            else return x - (iPartOfNumber(x) + 1);

        }


        Color C1(Color B, Color L, float y)
        {
            var r = L.R * (1 - fPartOfNumber(y)) + B.R * fPartOfNumber(y);
            var g = L.G * (1 - fPartOfNumber(y)) + B.G * fPartOfNumber(y);
            var b = L.B * (1 - fPartOfNumber(y)) + B.B * fPartOfNumber(y);

            return Color.FromArgb(255, (int)r, (int)g, (int)b);
        }

        Color C2(Color B, Color L, float y)
        {
            var r = B.R * (1 - fPartOfNumber(y)) + L.R * fPartOfNumber(y);
            var g = B.G * (1 - fPartOfNumber(y)) + L.G * fPartOfNumber(y);
            var b = B.B * (1 - fPartOfNumber(y)) + L.B * fPartOfNumber(y);

            return Color.FromArgb(255, (int)r, (int)g, (int)b);
        }


        public EdgeTableEntry AsEdgeInformation()
        {
            float slope = 0;
            if (StretchablePoints[1].Y - StretchablePoints[0].Y != 0)
                slope = (StretchablePoints[1].X - StretchablePoints[0].X) /(StretchablePoints[1].Y - StretchablePoints[0].Y) ;

            var sp = new List<Point>(StretchablePoints);
            sp.Sort((p, q) => p.Y.CompareTo(q.Y));

            return new EdgeTableEntry { yMin = sp.First().Y, yMax = StretchablePoints.Max(y => y.Y), xMin = sp.First().X, SlopeInverted =  slope };
        }

        public void Clip(IGraphicsEngine engine, IDrawable se)
        {
            //engine.Erase(this);
            var poly = (Polygon)se;
            if (StartingPoint == EndingPoint)
                return;
            else
            {
                List<float> tU = new();
                List<float> tL = new();
                //tU.Add(0.0F);
                //tL.Add(1.0F);
                var D = Distance(EndingPoint, StartingPoint);
                foreach (var e in poly.Lines)
                {
                    var boundray = e.StartingPoint;
                    var normalVector = e.NormalToOutside;
                    var w = Distance(StartingPoint, boundray);
                    var wn = dotProduct(w, normalVector);
                    var dn = dotProduct(D, normalVector);

                    if (dotProduct(normalVector,D) == 0)
                    {
                        if (dotProduct(NormalToOutside, w) > 0)
                        {
                            continue;
                        }
                    }


                    if (dn == 0)
                        continue;
                    var t = -1 * wn / dn;
                    if (t < 0 || t > 1)
                        continue;

                    if (dn > 0)
                    {
                        tL.Add(t);
                    }
                    if (dn < 0)
                    {
                        tU.Add(t);
                    }

                    //else
                    //{
                    //    var PEi = e.BasePoints.FirstOrDefault() ;
                    //    var denom = dotProduct(new Point(e.NormalToOutside.X, e.NormalToOutside.Y), new Point(EndingPoint.X - StartingPoint.X, EndingPoint.Y - StartingPoint.Y));
                    //    //denom = denom == 0 ? 1 : denom;
                    //    var t = -1*dotProduct(e.NormalToOutside, new Point(StartingPoint.X - PEi.X, StartingPoint.Y - PEi.Y)) / denom;

                    //    var sign = Math.Sign(dotProduct(e.NormalToOutside, new Point(EndingPoint.X - StartingPoint.X, EndingPoint.Y - StartingPoint.Y)));

                    //    if (t < 0 || t > 1)
                    //        continue;
                    //    else
                    //    {
                    //        if (sign < 0)
                    //        {
                    //            //PE
                    //            tE = Math.Max(tE, t);
                    //        }
                    //        else if (sign > 0)
                    //        {
                    //            tL = Math.Min(tL, t);
                    //        }

                    //    }

                    //}
                }
                var tLL = 0F;
                var tUU = 1F;
                if (tU.Count > 0)
                    tUU = tU.Min();
                if (tL.Count > 0)
                    tLL = tL.Max();
                var ptE = new Point((int)(StartingPoint.X + tUU * (EndingPoint.X - StartingPoint.X)), (int)(StartingPoint.Y + tUU * (EndingPoint.Y - StartingPoint.Y)));
                var ptL = new Point((int)(StartingPoint.X + tLL * (EndingPoint.X - StartingPoint.X)), (int)(StartingPoint.Y + tLL * (EndingPoint.Y - StartingPoint.Y)));
                Line line = new Line(ptE, ptL, Color.Red, Brush.Radius);
                line.CalculatePoints();


                //poly.StretchablePoints.AddRange(line.StretchablePoints);

                //foreach line point find closest index place add after it


                engine.Draw(line);






            }
        }

        public Point Distance(Point p1, Point p2)
        {
            var dx = p1.X - p2.X;
            var dy = p1.Y - p2.Y;


            return new Point(dx, dy);
        }

        float dotProduct(Point p1, Point p2)
        {
            //dotproduct
            return p1.X * p2.X + p1.Y * p2.Y;
        }

    }
}
