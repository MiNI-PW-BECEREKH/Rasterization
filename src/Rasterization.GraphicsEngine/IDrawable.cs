using System.Collections.Generic;
using System.Drawing;

namespace Rasterization.Engine
{
    public interface IDrawable
    {
        void Draw(IGraphicsEngine engine);
        void Erase(IGraphicsEngine engine);
        void IndicateSelection(IGraphicsEngine engine);
        void Stretch(IGraphicsEngine engine, int dx, int dy, int idx);

        void Move(IGraphicsEngine engine, int dx, int dy, int idx);

        void CalculateBrush();

        void CalculatePoints();

        bool HitTest(Point p);

        void DrawAA(IGraphicsEngine engine);

        void UpScale(IGraphicsEngine engine);

        Color Color { get; set; }
        List<Point> StretchablePoints { get; set; }
        List<Point> Points { get; set; }

        FilledCircle Brush { get; set; }

    }
}
