﻿using System.Drawing;
using System.Windows.Media.Imaging;

namespace Rasterization.Engine
{
    public interface IGraphicsEngine
    {
        WriteableBitmap Bitmap { get; set; }
        void Draw(Line line);
        void Erase(Line line);
        void Transparent(Line line);
        void Stretch(Line line);
        void Move(Line line);

        void Draw(Circle circle);
        void Erase(Circle circle);
        void Transparent(Circle circle);
        void Stretch(Circle circle);
        void Move(Circle circle);

    }
}
