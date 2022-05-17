using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rasterization.Engine
{
    public class Circle : IDrawable
    {
        public int Radius { get; set; }
        public Point Center {  get; set; }
        public Color Color { get ; set ; } = Color.Black;
        public List<Point> StretchablePoints { get ; set ; }
        public List<Point> Points { get; set; } = new();
        public FilledCircle Brush { get ; set ; }
        public List<Point> BrushPoints { get ; set ; }
        public List<Point> BasePoints { get; set; } = new();
        public bool IsAA { get ; set ; }
        public string Name { get; set; } = "Circle";
        public List<Line> Lines { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Circle()
        {

        }
        public Circle(Point center, Point radiusIndicator, Color color, int bs)
        {
            Center = center;
            Radius = (int)Math.Sqrt(Math.Pow(center.X - radiusIndicator.X, 2) + Math.Pow(center.Y - radiusIndicator.Y, 2));
            Color = color;
            Brush = new FilledCircle(new Point(0, 0), new Point(bs, bs), Color);
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
            StretchablePoints = new List<Point>(Points);
        }

        public void Draw(IGraphicsEngine engine)
        {
            if (IsAA)
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
            return Points.Contains(p);
        }

        public void IndicateSelection(IGraphicsEngine engine)
        {
            if (IsAA)
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
            if (IsAA)
            {
                engine.DrawAACircle(this);
            }
            else
                engine.Move(this);
        }

        public void CalculateBrush()
        {
            Brush.CalculatePoints(BasePoints);
            Points.AddRange(Brush.Points);
            Points.AddRange(BasePoints);
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
            if (IsAA)
            {
                engine.DrawAACircle(this);
            }
            else
                Draw(engine);
        }
    }
}
