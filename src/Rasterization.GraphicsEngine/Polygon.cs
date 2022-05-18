using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rasterization.Engine
{
    public class Polygon : IDrawable, ILinePolygon
    {
        public Color Color { get; set; }
        public List<Point> StretchablePoints { get; set; }
        public List<ColoredPoint> Points { get; set; } = new();
        public List<Point> BasePoints { get; set; } = new();

        public List<Line> Lines { get; set; } = new();
        public CircleBrush Brush { get; set; }
        public bool IsAntiAliased { get; set; }
        public string Name { get; set; } = "Polygon";
        public Point StartingPoint { get ; set ; }
        public Point EndingPoint { get ; set ; }

        public Polygon()
        {

        }

        public Polygon(List<Point> stretchablePoints, Color c, int bs)
        {
            StartingPoint = EndingPoint = stretchablePoints[0];
            StretchablePoints = new List<Point>(stretchablePoints);
            Color = c;
            Brush = new CircleBrush(new Point(0, 0), bs, Color);

            CalculateBrush();
            for (int i = 1; i < StretchablePoints.Count; i++)
            {
                Lines.Add(new Line(StretchablePoints[i - 1], StretchablePoints[i], Color, bs));
            }


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
                Lines.Add(new Line(StretchablePoints[i], end, Color, Brush.Radius));
            }

            foreach (var line in Lines)
            {
                line.CalculatePoints();
                Points.AddRange(line.Points);
                BasePoints.AddRange(line.BasePoints);
            }
        }

        public void Draw(IGraphicsEngine engine)
        {
            if (Lines.Count == 0)
                engine.Draw(this);
            else
                foreach (var line in Lines)
                {
                    line.Draw(engine);
                }
        }

        public void Erase(IGraphicsEngine engine)
        {
            foreach (var line in Lines)
                line.Erase(engine);
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
            for(int i = 0; i < StretchablePoints.Count; i++)
            {
                var point = StretchablePoints[i];
                StretchablePoints.RemoveAt(i);
                var movedPoint = new Point(point.X + dx, point.Y + dy);
                StretchablePoints.Insert(i, movedPoint);
            }
            CalculatePoints();
            engine.Move(this);
        }

        public void Stretch(IGraphicsEngine engine, int dx, int dy, int idx)
        {
            int lineIdx;
            foreach (var line in Lines)
            {

                if (line.StretchablePoints.Contains(StretchablePoints[idx]))
                {
                    line.Erase(engine);
                    lineIdx = line.StretchablePoints.IndexOf(StretchablePoints[idx]);
                    line.Stretch(engine, dx, dy, lineIdx);
                    StretchablePoints.RemoveAt(idx);
                    StretchablePoints.Insert(idx, line.StretchablePoints[lineIdx]);
                }
            }
            CalculatePoints();
            Lines.ForEach(l => l.IndicateSelection(engine));
        }

        public void CalculateBrush()
        {
            Lines.ForEach(l =>
            {
                l.Brush = Brush;
                l.Brush.CalculatePoints(l.BasePoints);
            });
        }

        public void DrawAA(IGraphicsEngine engine)
        {
            foreach (var line in Lines)
            {
                line.DrawAA(engine);
            }
        }

        public void UpScale(IGraphicsEngine engine)
        {
            throw new NotImplementedException();
        }

        public void CalculateAntiAliased(IGraphicsEngine engine)
        {
            throw new NotImplementedException();
        }
    }
}
