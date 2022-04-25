using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Rasterization.Engine
{
    public class GraphicsEngine : IGraphicsEngine
    {
        bool lazyMansStackOverflowProtection = false;
        public WriteableBitmap Bitmap { get; set; }

        public Color BackGround { get; set; } = Color.FromArgb(240, 240, 240);

        public Color GrabbedItemColor { get; set; } = Color.FromArgb(0, 0, 0);

        public GraphicsEngine(WriteableBitmap writeableBitmap)
        {
            Bitmap = writeableBitmap;
        }
        public void Draw(Line line)
        {
            Bitmap.Lock();
            try
            {
               foreach(var point in line.Points)
                {
                    SetPixel(point.X, point.Y, line.Color);
                    line.Brush.Center = point;
                    line.Brush.CalculatePoints();
                    Draw(line.Brush);

                }
            }
            finally
            {
                Bitmap.Unlock();
            }

        }

        public void Erase(Line line)
        {
            Bitmap.Lock();
            try
            {
                foreach (var point in line.Points)
                {
                    SetPixel(point.X, point.Y, BackGround);
                    line.Brush.Center = point;
                    line.Brush.CalculatePoints();
                    Erase(line.Brush);
                }
            }
            finally
            {
                Bitmap.Unlock();
            }
        }

        void SetPixel(int x, int y, Color color)
        {
            if (y > Bitmap.PixelHeight - 1 || x > Bitmap.PixelWidth - 1)
                return;
                //throw new Exception("Position for (x,y) is not in Bitmap");
            if (y < 0 || x < 0)
                return;
                //throw new Exception("Position for (x,y) is not in Bitmap");

            IntPtr pBackBuffer = Bitmap.BackBuffer;
            int stride = Bitmap.BackBufferStride;

            unsafe
            {
                byte* pBuffer = (byte*)pBackBuffer.ToPointer();
                int location = y * stride + x * 4;

                pBuffer[location] = color.B;
                pBuffer[location + 1] = color.G;
                pBuffer[location + 2] = color.R;
                pBuffer[location + 3] = color.A;

            }

            Bitmap.AddDirtyRect(new Int32Rect(x, y, 1, 1));
        }

        public void Transparent(Line line)
        {
            Erase(line);
            GrabbedItemColor = Color.FromArgb(128,line.Color.R,line.Color.G,line.Color.B);
            Bitmap.Lock();
            try
            {
                foreach (var point in line.Points)
                {
                    SetPixel(point.X, point.Y, GrabbedItemColor);
                    line.Brush.Center = point;
                    line.Brush.CalculatePoints();
                    Transparent(line.Brush);
                }
            }
            finally
            {
                Bitmap.Unlock();
            }
        }

        public void Stretch(Line line)
        {
            GrabbedItemColor = Color.FromArgb(128, line.Color.R, line.Color.G, line.Color.B);
            Bitmap.Lock();
            try
            {
                foreach (var point in line.Points)
                {
                    SetPixel(point.X, point.Y, GrabbedItemColor);
                    line.Brush.Center = point;
                    line.Brush.CalculatePoints();
                    Stretch(line.Brush);
                }
            }
            finally
            {
                Bitmap.Unlock();
            }
        }

        public void Draw(Circle circle)
        {
            Bitmap.Lock();
            try
            {
                foreach (var point in circle.Points)
                {
                    SetPixel(point.X, point.Y, circle.Color);
                    circle.Brush.Center = point;
                    circle.Brush.CalculatePoints();
                    Draw(circle.Brush);
                }
            }
            finally
            {
                Bitmap.Unlock();
            }
        }

        public void Erase(Circle circle)
        {
            Bitmap.Lock();
            try
            {
                foreach (var point in circle.Points)
                {
                    SetPixel(point.X, point.Y, BackGround);
                    circle.Brush.Center = point;
                    circle.Brush.CalculatePoints();
                    Erase(circle.Brush);
                }
            }
            finally
            {
                Bitmap.Unlock();
            }
        }

        public void Transparent(Circle circle)
        {
            Erase(circle);
            GrabbedItemColor = Color.FromArgb(128, circle.Color.R, circle.Color.G, circle.Color.B);
            Bitmap.Lock();
            try
            {
                foreach (var point in circle.Points)
                {
                    SetPixel(point.X, point.Y, GrabbedItemColor);
                    circle.Brush.Center = point;
                    circle.Brush.CalculatePoints();
                    Transparent(circle.Brush);
                }
            }
            finally
            {
                Bitmap.Unlock();
            }
        }

        public void Stretch(Circle circle)
        {
            GrabbedItemColor = Color.FromArgb(128, circle.Color.R, circle.Color.G, circle.Color.B);
            Bitmap.Lock();
            try
            {
                foreach (var point in circle.Points)
                {
                    SetPixel(point.X, point.Y, GrabbedItemColor);
                    circle.Brush.Center = point;
                    circle.Brush.CalculatePoints();
                    Stretch(circle.Brush);
                }
            }
            finally
            {
                Bitmap.Unlock();
            }
        }

        public void Move(Circle circle)
        {
            GrabbedItemColor = Color.FromArgb(128, circle.Color.R, circle.Color.G, circle.Color.B);
            Bitmap.Lock();
            try
            {
                foreach (var point in circle.Points)
                {
                    SetPixel(point.X, point.Y, GrabbedItemColor);
                    circle.Brush.Center = point;
                    circle.Brush.CalculatePoints();
                    Move(circle.Brush);
                }
            }
            finally
            {
                Bitmap.Unlock();
            }
        }


        public void Draw(FilledCircle circle)
        {
            Bitmap.Lock();
            try
            {
                foreach (var point in circle.Points)
                {
                    SetPixel(point.X, point.Y, circle.Color);

                }
            }
            finally
            {
                Bitmap.Unlock();
            }
        }

        public void Erase(FilledCircle circle)
        {
            Bitmap.Lock();
            try
            {
                foreach (var point in circle.Points)
                {
                    SetPixel(point.X, point.Y, BackGround);
                }
            }
            finally
            {
                Bitmap.Unlock();
            }
        }

        public void Transparent(FilledCircle circle)
        {
            Erase(circle);
            GrabbedItemColor = Color.FromArgb(128, circle.Color.R, circle.Color.G, circle.Color.B);
            Bitmap.Lock();
            try
            {
                foreach (var point in circle.Points)
                {
                    SetPixel(point.X, point.Y, GrabbedItemColor);
                }
            }
            finally
            {
                Bitmap.Unlock();
            }
        }

        public void Stretch(FilledCircle circle)
        {
            GrabbedItemColor = Color.FromArgb(128, circle.Color.R, circle.Color.G, circle.Color.B);
            Bitmap.Lock();
            try
            {
                foreach (var point in circle.Points)
                {
                    SetPixel(point.X, point.Y, GrabbedItemColor);
                }
            }
            finally
            {
                Bitmap.Unlock();
            }
        }

        public void Move(FilledCircle circle)
        {
            GrabbedItemColor = Color.FromArgb(128, circle.Color.R, circle.Color.G, circle.Color.B);
            Bitmap.Lock();
            try
            {
                foreach (var point in circle.Points)
                {
                    SetPixel(point.X, point.Y, GrabbedItemColor);
                }
            }
            finally
            {
                Bitmap.Unlock();
            }
        }

        public void Move(Line line)
        {
            GrabbedItemColor = Color.FromArgb(128, line.Color.R, line.Color.G, line.Color.B);
            Bitmap.Lock();
            try
            {
                foreach (var point in line.Points)
                {
                    SetPixel(point.X, point.Y, GrabbedItemColor);
                    line.Brush.Center = point;
                    line.Brush.CalculatePoints();
                    Move(line.Brush);
                }
            }
            finally
            {
                Bitmap.Unlock();
            }
        }

    }
}
