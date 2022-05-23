using System.Drawing;
using System.Windows.Media.Imaging;

namespace Rasterization.Engine
{
    public interface IGraphicsEngine
    {
        WriteableBitmap Bitmap { get; set; }
        void Draw(IDrawable line);
        void Erase(IDrawable line);
        void Transparent(IDrawable line);
        void Stretch(IDrawable line);
        void Move(IDrawable line);

        void FillLine(int y, int x1, int x2, Color c);

        void SetPixel(int x, int y, Color color);

        //void Draw(Circle circle);
        //void Erase(Circle circle);
        //void Transparent(Circle circle);
        //void Stretch(Circle circle);
        //void Move(Circle circle);

        //void Draw(Polygon arc);

        void DrawAALine(Line line);

        void DrawAACircle(Circle circle);

    }
}
