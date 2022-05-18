using System.Collections.Generic;
using System.Drawing;

namespace Rasterization.Engine
{
    public interface IDrawable : IMemorizedDrawable, IAntiaAliased
    {
        void Draw(IGraphicsEngine engine);
        void Erase(IGraphicsEngine engine);
        void IndicateSelection(IGraphicsEngine engine);
        void Stretch(IGraphicsEngine engine, int dx, int dy, int idx);
        void Move(IGraphicsEngine engine, int dx, int dy, int idx);

        void CalculateBrush();

        void CalculatePoints();

        bool HitTest(Point p);

        CircleBrush Brush { get; set; }

    }

    public interface IMemorizedDrawable
    {
        string Name { get; set; }
        Color Color { get; set; }
        List<ColoredPoint> Points { get; set; }
        List<Point> BasePoints { get; set; }
        List<Point> StretchablePoints { get; set; }
    }

    public interface IAntiaAliased
    {
        void CalculateAntiAliased(IGraphicsEngine engine);

        void UpScale(IGraphicsEngine engine);
        bool IsAntiAliased { get; set; }

    }

    public interface ILine
    {
        public Point StartingPoint { get; set; }
        public Point EndingPoint { get; set; }
    }

    public interface ILinePolygon : ILine
    {
        public List<Line> Lines { get; set; }
    }

    public interface ICircle
    {
        public int Radius { get; set; }
        public Point Center { get; set; }
    }
}
