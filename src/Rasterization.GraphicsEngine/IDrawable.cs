using System.Collections.Generic;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace Rasterization.Engine
{
    public interface IDrawable : IMemorizedDrawable, IAntiaAliased
    {
        void Draw(IGraphicsEngine engine);
        void Erase(IGraphicsEngine engine);
        void IndicateSelection(IGraphicsEngine engine);
        void Stretch(IGraphicsEngine engine, int dx, int dy, int idx);
        void Move(IGraphicsEngine engine, int dx, int dy, int idx);

        void Clip(IGraphicsEngine engine, IDrawable se);

        void CalculateBrush();

        void CalculatePoints();

        bool HitTest(Point p);

        CircleBrush Brush { get; set; }

    }

    public interface IMemorizedDrawable
    {
        public Point NormalToOutside { get; set; }
        string Name { get; set; }
        Color Color { get; set; }
        List<ColoredPoint> Points { get; set; }
        List<Point> BasePoints { get; set; }
        List<Point> StretchablePoints { get; set; }
        bool IsFilled { get; set; }

        bool IsFilledImage { get; set; }

        Color fillColor { get; set; }

        WriteableBitmap FillBitmap { get; set; }
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

        void FillImage(IGraphicsEngine engine, WriteableBitmap resource);
        public void FillPolygon(IGraphicsEngine engine, Color c);

    }

    public interface ICircle
    {
        public int Radius { get; set; }
        public Point Center { get; set; }
    }

    public class EdgeTableEntry
    {
        public int yMax { get; set; }
        public int yMin { get; set; }
        public float xMin { get; set; }
        public float SlopeInverted { get; set; }
    }
}
