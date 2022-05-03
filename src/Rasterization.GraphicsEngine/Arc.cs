using Rasterization.Engine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rasterization.Engine
{
    public class Arc : IDrawable
    {
        public Color Color { get; set; } = Color.Black;
        public List<Point> StretchablePoints { get; set; }
        public List<Point> Points { get; set; } = new();
        public FilledCircle Brush { get; set; }

        public List<Circle> Circles { get; set; } = new();

        public Arc(List<Point> pts)
        {
            StretchablePoints = new List<Point>(pts);
        }
        public void CalculateBrush()
        {

            throw new NotImplementedException();
        }

        public void CalculatePoints()
        {
            Points.Clear();
            var b = StretchablePoints[0];
            var a = StretchablePoints[1];
            var c = StretchablePoints[2];
            int x = (int)Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2)), y = 0;


            if (Determinant(a, b, c) > 0)
            {


                var D = new Point(x + a.X, y + a.Y);
                //if (Determinant(a, b, D) > 0 || Determinant(a, c, D) < 0)
                //    Points.Add(D);

                //if (x > 0)
                //{
                //    D = new Point(x + a.X, y + a.Y);
                //    if (Determinant(a, b, D) > 0 || Determinant(a, c, D) < 0)
                //        Points.Add(D);
                //    D = new Point(y + a.X, x + a.Y);
                //    if (Determinant(a, b, D) > 0 || Determinant(a, c, D) < 0)
                //        Points.Add(D);
                //    D = new Point(-y + a.X, x + a.Y);
                //    if (Determinant(a, b, D) > 0 || Determinant(a, c, D) < 0)
                //        Points.Add(D);
                //}

                int d = 1 - x;
                while (y < x)
                {
                    y++;
                    if (d <= 0)
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

                    D = new Point(x + a.X, y + a.Y);
                    if (Determinant(a, b, D) > 0 && Determinant(a, c, D) < 0)
                        Points.Add(D);
                    D = new Point(-x + a.X, y + a.Y);
                    if (Determinant(a, b, D) > 0 && Determinant(a, c, D) < 0)
                        Points.Add(D);
                    //Points.Add(new Point(-x + a.X, y + a.Y));
                    D = new Point(x + a.X, -y + a.Y);
                    if (Determinant(a, b, D) > 0 && Determinant(a, c, D) < 0)
                        Points.Add(D);
                    //Points.Add(new Point(x + a.X, -y + a.Y));
                    D = new Point(-x + a.X, -y + a.Y);
                    if (Determinant(a, b, D) > 0 && Determinant(a, c, D) < 0)
                        Points.Add(D);
                    //Points.Add(new Point(-x + a.X, -y + a.Y));

                    //if (x != y)
                    {
                        //Points.Add(new Point(y + a.X, x + a.Y));
                        //Points.Add(new Point(-y + a.X, x + a.Y));
                        //Points.Add(new Point(y + a.X, -x + a.Y));
                        //Points.Add(new Point(-y + a.X, -x + a.Y));
                        D = new Point(y + a.X, x + a.Y);
                        if (Determinant(a, b, D) > 0 && Determinant(a, c, D) < 0)
                            Points.Add(D);
                        D = new Point(-y + a.X, x + a.Y);
                        if (Determinant(a, b, D) > 0 && Determinant(a, c, D) < 0)
                            Points.Add(D);
                        //Points.Add(new Point(-x + a.X, y + a.Y));
                        D = new Point(y + a.X, -x + a.Y);
                        if (Determinant(a, b, D) > 0 && Determinant(a, c, D) < 0)
                            Points.Add(D);
                        //Points.Add(new Point(x + a.X, -y + a.Y));
                        D = new Point(-y + a.X, -x + a.Y);
                        if (Determinant(a, b, D) > 0 && Determinant(a, c, D) < 0)
                            Points.Add(D);

                    }
                }


            }
            else if(Determinant(a, b, c) < 0)
            {


                var D = new Point(x + a.X, y + a.Y);
                //if (Determinant(a, b, D) > 0 && Determinant(a, c, D) < 0)
                //    Points.Add(D);

                //if (x > 0)
                //{
                //    D = new Point(x + a.X, y + a.Y);
                //    if (Determinant(a, b, D) > 0 && Determinant(a, c, D) < 0)
                //        Points.Add(D);
                //    D = new Point(y + a.X, x + a.Y);
                //    if (Determinant(a, b, D) > 0 && Determinant(a, c, D) < 0)
                //        Points.Add(D);
                //    D = new Point(-y + a.X, x + a.Y);
                //    if (Determinant(a, b, D) > 0 && Determinant(a, c, D) < 0)
                //        Points.Add(D);
                //}

                int d = 1 - x;
                while (y < x)
                {
                    y++;
                    if (d <= 0)
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

                    D = new Point(x + a.X, y + a.Y);
                    if (Determinant(a, b, D) > 0 || Determinant(a, c, D) < 0)
                        Points.Add(D);
                    D = new Point(-x + a.X, y + a.Y);
                    if (Determinant(a, b, D) > 0 || Determinant(a, c, D) < 0)
                        Points.Add(D);
                    //Points.Add(new Point(-x + a.X, y + a.Y));
                    D = new Point(x + a.X, -y + a.Y);
                    if (Determinant(a, b, D) > 0 || Determinant(a, c, D) < 0)
                        Points.Add(D);
                    //Points.Add(new Point(x + a.X, -y + a.Y));
                    D = new Point(-x + a.X, -y + a.Y);
                    if (Determinant(a, b, D) > 0 || Determinant(a, c, D) < 0)
                        Points.Add(D);
                    //Points.Add(new Point(-x + a.X, -y + a.Y));

                    //if (x != y)
                    {
                        //Points.Add(new Point(y + a.X, x + a.Y));
                        //Points.Add(new Point(-y + a.X, x + a.Y));
                        //Points.Add(new Point(y + a.X, -x + a.Y));
                        //Points.Add(new Point(-y + a.X, -x + a.Y));
                        D = new Point(y + a.X, x + a.Y);
                        if (Determinant(a, b, D) > 0 || Determinant(a, c, D) < 0)
                            Points.Add(D);
                        D = new Point(-y + a.X, x + a.Y);
                        if (Determinant(a, b, D) > 0 || Determinant(a, c, D) < 0)
                            Points.Add(D);
                        //Points.Add(new Point(-x + a.X, y + a.Y));
                        D = new Point(y + a.X, -x + a.Y);
                        if (Determinant(a, b, D) > 0 || Determinant(a, c, D) < 0)
                            Points.Add(D);
                        //Points.Add(new Point(x + a.X, -y + a.Y));
                        D = new Point(-y + a.X, -x + a.Y);
                        if (Determinant(a, b, D) > 0 || Determinant(a, c, D) < 0)
                            Points.Add(D);

                    }
                }


            }
        }

        public void Draw(IGraphicsEngine engine)
        {
            engine.Draw(this);
        }

        public void Erase(IGraphicsEngine engine)
        {
            throw new NotImplementedException();
        }

        public bool HitTest(Point p)
        {
            throw new NotImplementedException();
        }

        public void IndicateSelection(IGraphicsEngine engine)
        {
            throw new NotImplementedException();
        }

        public void Move(IGraphicsEngine engine, int dx, int dy, int idx)
        {
            throw new NotImplementedException();
        }

        public void Stretch(IGraphicsEngine engine, int dx, int dy, int idx)
        {
            throw new NotImplementedException();
        }

        int Determinant(Point a, Point b, Point c)
        {
            return a.X * b.Y - a.X * c.Y - a.Y * b.X + a.Y * c.X + b.X * c.Y - b.Y * c.X;
        }

        public void DrawAA(IGraphicsEngine engine)
        {
            throw new NotImplementedException();
        }

        public void UpScale(IGraphicsEngine engine)
        {
            throw new NotImplementedException();
        }
    }
}
