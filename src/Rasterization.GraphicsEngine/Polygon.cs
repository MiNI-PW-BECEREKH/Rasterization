using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rasterization.Engine
{
    public class Polygon : IDrawable
    {
        public Color Color { get; set; }
        public List<Point> StretchablePoints { get; set; }
        public List<Point> Points { get; set; } = new();

        public List<Line> Lines { get; set; } = new();
        public FilledCircle Brush { get; set; }
        public List<Point> BrushPoints { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public List<Point> BasePoints { get; set; } = new();
        public bool IsAA { get; set; }
        public string Name { get; set; } = "Polygon";

        public Polygon()
        {

        }

        public Polygon(List<Point> stretchablePoints, Color c, int bs)
        {
            StretchablePoints = new List<Point>(stretchablePoints);
            Color = c;
            Brush = new FilledCircle(new Point(0, 0), new Point(bs, bs), Color);

            CalculateBrush();
            //for(int i = 1; i < StretchablePoints.Count; i++)
            //{
            //    Lines.Add(new Line(StretchablePoints[i - 1], StretchablePoints[i], Color));
            //}


        }

        public void CalculatePoints()
        {
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
            return Points.Contains(p);
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
            foreach (var line in Lines)
            {
                line.Move(engine, dx, dy, idx);
            }
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
            foreach (var line in Lines)
            {
                line.Brush = Brush;
                line.Brush.CalculatePoints();
            }
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
    }
}
