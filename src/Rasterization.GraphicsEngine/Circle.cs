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
        public Color Color { get ; set ; }
        public List<Point> StretchablePoints { get ; set ; }
        public List<Point> Points { get; set; } = new();
        public FilledCircle Brush { get ; set ; }

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
            int x = Radius, y = 0;

            Points.Add(new Point(x + Center.X, y + Center.Y));

            if (Radius > 0)
            {
                Points.Add(new Point(x + Center.X, y + Center.Y));
                Points.Add(new Point(y + Center.X, x + Center.Y));
                Points.Add(new Point(-y + Center.X, x + Center.Y));
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

                Points.Add(new Point(x + Center.X, y + Center.Y));
                Points.Add(new Point(-x + Center.X, y + Center.Y));
                Points.Add(new Point(x + Center.X, -y + Center.Y));
                Points.Add(new Point(-x + Center.X, -y + Center.Y));

                if(x != y)
                {
                    Points.Add(new Point(y + Center.X, x + Center.Y));
                    Points.Add(new Point(-y + Center.X, x + Center.Y));
                    Points.Add(new Point(y + Center.X, -x + Center.Y));
                    Points.Add(new Point(-y + Center.X, -x + Center.Y));

                }

            }

            StretchablePoints = new List<Point>(Points);
        }

        public void Draw(IGraphicsEngine engine)
        {
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
            engine.Move(this);
        }

        public void CalculateBrush()
        {
            Brush.CalculatePoints();
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
            Draw(engine);
        }
    }
}
