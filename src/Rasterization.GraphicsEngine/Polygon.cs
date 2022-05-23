using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Media.Imaging;

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
        public Point StartingPoint { get; set; }
        public Point EndingPoint { get; set; }
        public List<EdgeTableEntry> ActiveEdgeTable { get; set; } = new();
        public List<EdgeTableEntry> EdgeTable { get; set; } = new();
        public Point NormalToOutside { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsFilled { get; set; } = false;
        public Color fillColor { get; set; }
        public bool IsFilledImage { get; set; } = false;
        public WriteableBitmap FillBitmap { get; set; }

        public Polygon()
        {

        }

        public Polygon(List<Point> stretchablePoints, Color c, int bs)
        {
            StartingPoint = EndingPoint = stretchablePoints[0];
            StretchablePoints = new List<Point>(stretchablePoints);
            Color = c;
            Brush = new CircleBrush(new Point(0, 0), bs, Color);
            if (Brush.Radius > 0)
                CalculateBrush();
            for (int i = 1; i < StretchablePoints.Count; i++)
            {
                Lines.Add(new Line(StretchablePoints[i - 1], StretchablePoints[i], Color, bs));
            }

            //GetEdgeTable();
        }

        public List<EdgeTableEntry> GetEdgeTable()
        {
            List<EdgeTableEntry> edgeTable = new List<EdgeTableEntry>();
            Point temp = StretchablePoints.Last();

            foreach (var v in StretchablePoints)
            {
                EdgeTableEntry et = new EdgeTableEntry();
                et.yMin = Math.Min(v.Y, temp.Y);
                et.yMax = Math.Max(v.Y, temp.Y);

                if (v.Y < temp.Y)
                    et.xMin = v.X;
                else
                    et.xMin = temp.X;
                int dy = (v.Y - temp.Y);
                int dx = (v.X - temp.X);

                if (dy == 0) et.SlopeInverted = 0;

                if (dy != 0) et.SlopeInverted = (float)dx / (float)dy;

                edgeTable.Add(et);

                temp = v;
            }

            edgeTable.Sort((p, q) => p.yMin.CompareTo(q.yMin));

            return edgeTable;
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
            engine.Draw(this);
            //if (Lines.Count == 0)
            //    engine.Draw(this);
            //else
            //    foreach (var line in Lines)
            //    {
            //        line.Draw(engine);
            //    }
        }

        public void Erase(IGraphicsEngine engine)
        {
            engine.Erase(this);
            //foreach (var line in Lines)
            //    line.Erase(engine);
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
            if (IsFilled)
                FillPolygon(engine, fillColor);
            else if (IsFilledImage)
            {
                FillImage(engine, FillBitmap);
            }
            engine.Move(this);
            Lines.ForEach(l => l.CalculateNormal());

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
            if (IsFilled)
                FillPolygon(engine, fillColor);
            else if (IsFilledImage)
            {
                FillImage(engine, FillBitmap);
            }
            Lines.ForEach(l => l.IndicateSelection(engine));
            Lines.ForEach(l => l.CalculateNormal());
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
            foreach (var line in Lines)
            {
                line.DrawAA(engine);
            }
        }

        public void FillPolygon(IGraphicsEngine engine, Color c)
        {
            fillColor = c;
            EdgeTable = GetEdgeTable();
            var ETmin = EdgeTable[0];
            int y = ETmin.yMin;
            List<EdgeTableEntry> activeEdgeTable = new List<EdgeTableEntry>();

            while (EdgeTable.Count != 0 || activeEdgeTable.Count != 0)
            {
                List<EdgeTableEntry> toRemove = new List<EdgeTableEntry>();
                foreach (var et in EdgeTable)
                {
                    if (et.yMin == y)
                    {
                        activeEdgeTable.Add(et);
                        toRemove.Add(et);
                    }
                    else
                    {
                        break;
                    }
                }
                foreach (var et in toRemove)
                {
                    EdgeTable.Remove(et);
                }

                toRemove.Clear();
                activeEdgeTable.Sort((p, q) => p.xMin.CompareTo(q.xMin));

                for (int n = 0; n + 1 < activeEdgeTable.Count; n += 2)
                {

                    //engine.FillLine(y, (int)activeEdgeTable[n].xMin, (int)activeEdgeTable[n + 1].xMin, c);
                    for (int a = (int)activeEdgeTable[n].xMin; (int)a <= activeEdgeTable[n + 1].xMin; a++)
                    {
                        //engine.SetPixel(a, y, resource.GetPixel(a - xMin, y - yMin));
                        Points.Add(new ColoredPoint(a, y, c));
                    }

                }

                ++y;

                foreach (var e in activeEdgeTable.ToList())
                {
                    if (e.yMax == y)
                        activeEdgeTable.Remove(e);
                }

                foreach (var e in activeEdgeTable.ToList())
                {
                    e.xMin += e.SlopeInverted;
                }

            }

            //engine.Draw(this);
        }

        public void FillImage(IGraphicsEngine engine, WriteableBitmap resource)
        {
            FillBitmap = resource;
            int xMin = StretchablePoints.OrderBy(p => p.X).First().X;
            int yMin = StretchablePoints.OrderBy(p => p.Y).First().Y;
            int xMax = StretchablePoints.OrderBy(p => p.X).Last().X;
            int yMax = StretchablePoints.OrderBy(p => p.Y).Last().Y;


            List<EdgeTableEntry> edgeTable = GetEdgeTable();
            EdgeTableEntry ETmin = edgeTable[0];
            int y = ETmin.yMin;
            List<EdgeTableEntry> activeEdgeTable = new List<EdgeTableEntry>();

            while (edgeTable.Count != 0 || activeEdgeTable.Count != 0)
            {
                List<EdgeTableEntry> toRemove = new List<EdgeTableEntry>();
                foreach (var et in edgeTable)
                {
                    if (et.yMin == y)
                    {
                        activeEdgeTable.Add(et);
                        toRemove.Add(et);
                    }
                    else
                    {
                        break;
                    }
                }
                foreach (var et in toRemove)
                {
                    edgeTable.Remove(et);
                }
                toRemove.Clear();
                activeEdgeTable.Sort((p, q) => p.xMin.CompareTo(q.xMin));
                for (int n = 0; n + 1 < activeEdgeTable.Count; n += 2)
                {
                    for (int a = (int)activeEdgeTable[n].xMin; (int)a <= activeEdgeTable[n + 1].xMin; a++)
                    {
                        //engine.SetPixel(a, y, resource.GetPixel(a - xMin, y - yMin));
                        var c = resource.GetPixel(a - xMin, y - yMin);
                        Points.Add(new ColoredPoint(a, y, c));

                    }
                }


                ++y;

                foreach (var e in activeEdgeTable.ToList())
                {
                    if (e.yMax == y)
                        activeEdgeTable.Remove(e);
                }

                foreach (var e in activeEdgeTable.ToList())
                {
                    e.xMin += e.SlopeInverted;
                }
            }

            //engine.Draw(this);
        }

        public void Clip(IGraphicsEngine engine, IDrawable se)
        {
            foreach (var line in Lines)
            {
                line.Clip(engine, se);
            }
        }



    }
}
