using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Rasterization.Engine
{
    public class Rectangle : IDrawable, ILinePolygon
    {
        public Color Color { get; set; } = Color.Black;
        public List<Point> StretchablePoints { get; set; } = new();
        public List<ColoredPoint> Points { get; set; } = new();
        public List<Point> BasePoints { get; set; } = new();

        public List<Line> Lines { get; set; } = new();
        public CircleBrush Brush { get; set; }
        public bool IsAntiAliased { get; set; }
        public string Name { get; set; } = "Polygon";
        public Point StartingPoint { get; set; }
        public Point EndingPoint { get; set; }
        public List<EdgeTableEntry> ActiveEdgeTable { get; set; } = new();
        public List<EdgeTableEntry> EdgeTable { get; set; } = new();
        public Point NormalToOutside { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsFilled { get; set; } = false;
        public Color fillColor { get; set; }
        public bool IsFilledImage { get; set; } = false;
        public WriteableBitmap FillBitmap { get; set; }

        public Rectangle(Point p1, Point p2)
        {
            StretchablePoints.Add(p1);
            StretchablePoints.Add(new Point(p1.X, p2.Y));
            StretchablePoints.Add(p2);
            StretchablePoints.Add(new Point(p2.X,p1.Y));

            for (int i = 1; i < StretchablePoints.Count; i++)
            {
                Lines.Add(new Line(StretchablePoints[i - 1], StretchablePoints[i], Color, 0));
            }


        }

        public void CalculateAntiAliased(IGraphicsEngine engine)
        {
            throw new NotImplementedException();
        }

        public void CalculateBrush()
        {
            throw new NotImplementedException();
        }

        public void CalculatePoints()
        {
            Points.Clear();
            BasePoints.Clear();
            Lines.Clear();

            for (int i = 0; i < StretchablePoints.Count; i++)
            {
                Point end;
                if (i + 1 >= StretchablePoints.Count)
                {
                    end = StretchablePoints.FirstOrDefault();
                }
                else
                    end = StretchablePoints[i + 1];
                Lines.Add(new Line(StretchablePoints[i], end, Color, 0));
            }

            foreach (var line in Lines)
            {
                line.CalculatePoints();
                Points.AddRange(line.Points);
                BasePoints.AddRange(line.BasePoints);
            }
        }

        public void Clip(IGraphicsEngine engine, IDrawable se)
        {
            throw new NotImplementedException();
        }

        public void Draw(IGraphicsEngine engine)
        {
            engine.Draw(this);
        }

        public void Erase(IGraphicsEngine engine)
        {
            engine.Erase(this);
        }

        public void FillImage(IGraphicsEngine engine, WriteableBitmap resource)
        {
            throw new NotImplementedException();
        }

        public void FillPolygon(IGraphicsEngine engine, Color c)
        {
            throw new NotImplementedException();
        }

        public bool HitTest(Point p)
        {
            return Points.Any(point => Math.Sqrt(Math.Pow(p.X - point.X, 2) + Math.Pow(p.Y - point.Y, 2)) < 10);
        }

        public void IndicateSelection(IGraphicsEngine engine)
        {
            foreach (var line in Lines)
            {
                line.IndicateSelection(engine);
            }
        }

        public void Move(IGraphicsEngine engine, int dx, int dy, int idx)
        {
            Erase(engine);
            for (int i = 0; i < StretchablePoints.Count; i++)
            {
                var point = StretchablePoints[i];
                StretchablePoints.RemoveAt(i);
                var movedPoint = new Point(point.X + dx, point.Y + dy);
                StretchablePoints.Insert(i, movedPoint);
            }
            CalculatePoints();
            engine.Move(this);
            Lines.ForEach(l => l.CalculateNormal());
        }

        public void Stretch(IGraphicsEngine engine, int dx, int dy, int idx)
        {
            //int lineIdx;
            //var hitLine = Lines.Where(line => line.StretchablePoints.Contains(StretchablePoints[idx])).FirstOrDefault();

            //var linesToChange = Lines.Where(line => line.StretchablePoints.Contains(hitLine.StretchablePoints[0]) || line.StretchablePoints.Contains(hitLine.StretchablePoints[1])).ToList();

            //linesToChange.ForEach(line =>
            //{
               
            //});

            //CalculatePoints();
           
            //Lines.ForEach(l => l.IndicateSelection(engine));
            //Lines.ForEach(l => l.CalculateNormal());
        }

        public void UpScale(IGraphicsEngine engine)
        {
            throw new NotImplementedException();
        }
    }
}
