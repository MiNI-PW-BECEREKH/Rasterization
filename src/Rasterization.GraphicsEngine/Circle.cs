using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Rasterization.Engine
{
    public class Circle : IDrawable, ICircle
    {
        public int Radius { get; set; }
        public Point Center {  get; set; }
        public Color Color { get ; set ; } = Color.Black;
        public CircleBrush Brush { get ; set ; }
        public List<Point> StretchablePoints { get ; set ; }
        public List<ColoredPoint> Points { get; set; } = new();
        public List<Point> BasePoints { get; set; } = new();
        public bool IsAntiAliased { get ; set ; }
        public string Name { get; set; } = "Circle";
        public Point NormalToOutside { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsFilled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Color fillColor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsFilledImage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public WriteableBitmap FillBitmap { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Circle()
        {

        }
        public Circle(Point center, Point radiusIndicator, Color color, int bs)
        {
            Center = center;
            Radius = (int)Math.Sqrt(Math.Pow(center.X - radiusIndicator.X, 2) + Math.Pow(center.Y - radiusIndicator.Y, 2));
            Color = color;
            Brush = new CircleBrush(new Point(0, 0), bs, Color);
            if(Brush.Radius > 0)
            CalculateBrush();
        }

        public Circle(Point center, Point radiusIndicator, Color color)
        {
            Center = center;
            Radius = (int)Math.Sqrt(Math.Pow(center.X - radiusIndicator.X, 2) + Math.Pow(center.Y - radiusIndicator.Y, 2));
            Color = color;
        }
        public virtual void CalculatePoints()
        {
            Points.Clear();
            BasePoints.Clear();
            int x = Radius, y = 0;

            BasePoints.Add(new Point(x + Center.X, y + Center.Y));

            if (Radius > 0)
            {
                BasePoints.Add(new Point(x + Center.X, y + Center.Y));
                BasePoints.Add(new Point(y + Center.X, x + Center.Y));
                BasePoints.Add(new Point(-y + Center.X, x + Center.Y));
            }

            int d = 1 - Radius;
            while(y < x)
            {
                y++;
                if(d <= 0)
                {
                    d = d + 2 * y + 1;
                }
                else
                {
                    x--;
                    d = d + 2 * y - 2 * x + 1;
                }

                if (x < y)
                    break;

                BasePoints.Add(new Point(x + Center.X, y + Center.Y));
                BasePoints.Add(new Point(-x + Center.X, y + Center.Y));
                BasePoints.Add(new Point(x + Center.X, -y + Center.Y));
                BasePoints.Add(new Point(-x + Center.X, -y + Center.Y));

                if(x != y)
                {
                    BasePoints.Add(new Point(y + Center.X, x + Center.Y));
                    BasePoints.Add(new Point(-y + Center.X, x + Center.Y));
                    BasePoints.Add(new Point(y + Center.X, -x + Center.Y));
                    BasePoints.Add(new Point(-y + Center.X, -x + Center.Y));

                }

            }

            CalculateBrush();
            StretchablePoints = new List<Point>(Points.Select(p => p.AsPoint()));
        }

        public void Draw(IGraphicsEngine engine)
        {
            if (IsAntiAliased)
            {
                engine.DrawAACircle(this);
            }
            else
                engine.Draw(this);
        }

        public void Erase(IGraphicsEngine engine)
        {
            engine.Erase(this);
        }

        public bool HitTest(Point p)
        {
            return Points.Any(point => Math.Sqrt(Math.Pow(p.X - point.X, 2) + Math.Pow(p.Y - point.Y, 2)) < 10);
        }

        public void IndicateSelection(IGraphicsEngine engine)
        {
            if (IsAntiAliased)
            {
                engine.DrawAACircle(this);
            }
            else
                engine.Transparent(this);
        }

        public void Stretch(IGraphicsEngine engine, int dx, int dy, int idx)
        {
            int sign = 1;
            if (dx < 0 || dy < 0)
                sign = -sign;
            if(Radius <= 0 && sign < 0) return;


            Radius += sign*(int)Math.Sqrt(Math.Pow(dx,2) + Math.Pow(dy,2));
            CalculatePoints();
            engine.Stretch(this);
        }

        public void Move(IGraphicsEngine engine, int dx, int dy, int idx)
        {
            Center = new Point(Center.X + dx, Center.Y + dy);
            CalculatePoints();
            if (IsAntiAliased)
            {
                engine.DrawAACircle(this);
            }
            else
                engine.Move(this);
        }

        public void CalculateBrush()
        {
            Brush.CalculatePoints(BasePoints);
            Points.AddRange(Brush.Points.Select(p => p.AsColoredPoint(Color)));
            Points.AddRange(BasePoints.Select(p => p.AsColoredPoint(Color)));
        }

        public void DrawAA(IGraphicsEngine engine)
        {
            engine.DrawAACircle(this);
        }

        public void UpScale(IGraphicsEngine engine)
        {
            Erase(engine);
            Brush.Radius = 2 * Brush.Radius;
            Center = new Point(Center.X * 2, Center.Y * 2);
            Radius = 2 * Radius;
            CalculatePoints();
            if (IsAntiAliased)
            {
                engine.DrawAACircle(this);
            }
            else
                Draw(engine);
        }

        Color CircleC2(Color B, Color L, float T)
        {
            var r = L.R * (1 - T) + B.R * T;
            var g = L.G * (1 - T) + B.G * T;
            var b = L.B * (1 - T) + B.B * T;

            return Color.FromArgb(255, (int)r, (int)g, (int)b);
        }

        Color CircleC1(Color B, Color L, float T)
        {
            var r = B.R * (1 - T) + L.R * T;
            var g = B.G * (1 - T) + L.G * T;
            var b = B.B * (1 - T) + L.B * T;

            return Color.FromArgb(255, (int)r, (int)g, (int)b);
        }


        public void CalculateAntiAliased(IGraphicsEngine engine)
        {
                Points.Clear();
                //Erase(circle)
                //Draw(circle);
                Color L = this.Color;
                Color B = Color.FromArgb(250, 250, 250); 
                var Center = this.Center;
                int r = this.Radius;
                int x = this.Radius;
                int y = 0;
                //SetPixel(x, y, L);

                //SetPixel(Center.X + x, Center.Y + y, L);
                //SetPixel(Center.X + x - 1, Center.Y + y, L);

                //SetPixel(Center.X + y, Center.Y + x, L);
                //SetPixel(Center.X + y - 1, Center.Y + x, L);

                //SetPixel(Center.X - y, Center.Y + x, L);
                //SetPixel(Center.X - y + 1, Center.Y + x, L);

                //SetPixel(Center.X - x, Center.Y + y, L);
                //SetPixel(Center.X - x + 1, Center.Y + y, L);

                //SetPixel(Center.X - x, Center.Y - y, L);
                //SetPixel(Center.X - x + 1, Center.Y - y, L);

                //SetPixel(Center.X - y, Center.Y - x, L);
                //SetPixel(Center.X - y + 1, Center.Y - x, L);

                //SetPixel(Center.X + y, Center.Y - x, L);
                //SetPixel(Center.X + y - 1, Center.Y - x, L);

                //SetPixel(Center.X + x, Center.Y - y, L);
                //SetPixel(Center.X + x - 1, Center.Y - y, L);

                while (x > y)
                {
                    ++y;
                    x = (int)(Math.Ceiling(Math.Sqrt((r * r) - (y * y))));

                    float T = (float)(Math.Ceiling(Math.Sqrt(r * r - y * y)) - Math.Sqrt(r * r - y * y));

                    var c2 = CircleC2(B, L, T);
                    var c1 = CircleC1(B, L, T);

                    if (y > x)
                        break;


                    Points.Add(new ColoredPoint(Center.X + x, Center.Y + y, c2)); //7
                    Points.Add(new ColoredPoint(Center.X + x - Brush.Radius - 1, Center.Y + y, c1));

                    Points.Add(new ColoredPoint(Center.X + y, Center.Y + x, c2)); //8
                    Points.Add(new ColoredPoint(Center.X + y, Center.Y + x - Brush.Radius - 1, c1));

                    Points.Add(new ColoredPoint(Center.X - y, Center.Y + x, c2)); //5
                    Points.Add(new ColoredPoint(Center.X - y, Center.Y + x - Brush.Radius - 1, c1));

                    Points.Add(new ColoredPoint(Center.X - x, Center.Y + y, c2)); //6
                    Points.Add(new ColoredPoint(Center.X - x + Brush.Radius + 1, Center.Y + y, c1));

                    Points.Add(new ColoredPoint(Center.X - x, Center.Y - y, c2)); //3
                    Points.Add(new ColoredPoint(Center.X - x + Brush.Radius + 1, Center.Y - y, c1));

                    Points.Add(new ColoredPoint(Center.X - y, Center.Y - x, c2)); //4
                    Points.Add(new ColoredPoint(Center.X - y, Center.Y - x + Brush.Radius + 1, c1));

                    Points.Add(new ColoredPoint(Center.X + y, Center.Y - x, c2)); //1
                    Points.Add(new ColoredPoint(Center.X + y, Center.Y - x + Brush.Radius + 1, c1));

                    Points.Add(new ColoredPoint(Center.X + x, Center.Y - y, c2)); //2
                    Points.Add(new ColoredPoint(Center.X + x - Brush.Radius - 1, Center.Y - y, c1));


                }
            
                engine.Draw(this);

        }

        public void Clip(IGraphicsEngine engine, IDrawable se)
        {
            throw new NotImplementedException();
        }
    }
}
