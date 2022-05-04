using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Rasterization.Engine
{
    public class GraphicsEngine : IGraphicsEngine
    {
        public WriteableBitmap Bitmap { get; set; }

        public Color BackGround { get; set; } = Color.FromArgb(240, 240, 240);

        public Color GrabbedItemColor { get; set; } = Color.FromArgb(0, 0, 0);

        public GraphicsEngine(WriteableBitmap writeableBitmap)
        {
            Bitmap = writeableBitmap;
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

        #region lineprocedures
        public void Draw(Line line)
        {
            Bitmap.Lock();
            try
            {
                foreach (var point in line.Points)
                {
                    SetPixel(point.X, point.Y, line.Color);
                    //line.Brush.Center = point;
                    //line.Brush.CalculatePoints();
                    //Draw(line.Brush);

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
                    //line.Brush.Center = point;
                    //line.Brush.CalculatePoints();
                    //Erase(line.Brush);
                }
            }
            finally
            {
                Bitmap.Unlock();
            }
        }



        public void Transparent(Line line)
        {
            Erase(line);
            GrabbedItemColor = Color.FromArgb(128, line.Color.R, line.Color.G, line.Color.B);
            Bitmap.Lock();
            try
            {
                foreach (var point in line.Points)
                {
                    SetPixel(point.X, point.Y, GrabbedItemColor);
                    //line.Brush.Center = point;
                    //line.Brush.CalculatePoints();
                    //Transparent(line.Brush);
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
                    //line.Brush.Center = point;
                    //line.Brush.CalculatePoints();
                    //Stretch(line.Brush);
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
                    //line.Brush.Center = point;
                    ////TODO: fix brush
                    //line.Brush.CalculatePoints();
                    //Move(line.Brush);
                }
            }
            finally
            {
                Bitmap.Unlock();
            }
        }

        #endregion

        #region circleprocedures
        public void Draw(Circle circle)
        {
            Bitmap.Lock();
            try
            {
                foreach (var point in circle.Points)
                {
                    SetPixel(point.X, point.Y, circle.Color);
                    //circle.Brush.Center = point;
                    //circle.Brush.CalculatePoints();
                    //Draw(circle.Brush);
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
                    //circle.Brush.Center = point;
                    //circle.Brush.CalculatePoints();
                    //Erase(circle.Brush);
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
                    //circle.Brush.Center = point;
                    //circle.Brush.CalculatePoints();
                    //Transparent(circle.Brush);
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
                    //circle.Brush.Center = point;
                    //circle.Brush.CalculatePoints();
                    //Stretch(circle.Brush);
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
                    //circle.Brush.Center = point;
                    //circle.Brush.CalculatePoints();
                    //Move(circle.Brush);
                }
            }
            finally
            {
                Bitmap.Unlock();
            }
        }
        #endregion

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



        public void Draw(Arc arc)
        {
            Bitmap.Lock();
            try
            {
                foreach (var point in arc.Points)
                {
                    SetPixel(point.X, point.Y, arc.Color);

                }
            }
            finally
            {
                Bitmap.Unlock();
            }
        }

        public void Draw(Polygon arc)
        {
            Bitmap.Lock();
            try
            {
                foreach (var point in arc.Points)
                {
                    SetPixel(point.X, point.Y, arc.Color);

                }
            }
            finally
            {
                Bitmap.Unlock();
            }
        }

        int iPartOfNumber(float x)
        {
            return (int)x;
        }

        //rounds off a number 
        int roundNumber(float x)
        {
            return iPartOfNumber(x + 0.5f);
        }

        //returns fractional part of a number 
        float fPartOfNumber(float x)
        {
            if (x > 0) return x - iPartOfNumber(x);
            else return x - (iPartOfNumber(x) + 1);

        }


        Color C1(Color B, Color L, float y)
        {
            var r = L.R * (1 - fPartOfNumber(y)) + B.R * fPartOfNumber(y);
            var g = L.G * (1 - fPartOfNumber(y)) + B.G * fPartOfNumber(y);
            var b = L.B * (1 - fPartOfNumber(y)) + B.B * fPartOfNumber(y);

            return Color.FromArgb(255, (int)r, (int)g, (int)b);
        }

        Color C2(Color B, Color L, float y)
        {
            var r = B.R * (1 - fPartOfNumber(y)) + L.R * fPartOfNumber(y);
            var g = B.G * (1 - fPartOfNumber(y)) + L.G * fPartOfNumber(y);
            var b = B.B * (1 - fPartOfNumber(y)) + L.B * fPartOfNumber(y);

            return Color.FromArgb(255, (int)r, (int)g, (int)b);
        }

        Color CircleC2(Color B, Color L, float T)
        {
            var r = L.R * (1 - T) + B.R * T;
            var g = L.G * (1 - T) + B.G * T;
            var b = L.B * (1 - T) + B.B * T;

            return Color.FromArgb(255, (int)r, (int)g, (int)b);
        }

        Color CircleC1(Color B, Color L, float T)
        {
            var r = B.R * (1 - T) + L.R * T;
            var g = B.G * (1 - T) + L.G * T;
            var b = B.B * (1 - T) + L.B * T;

            return Color.FromArgb(255, (int)r, (int)g, (int)b);
        }

        public void DrawAALine(Line line)
        {
            try
            {
                Bitmap.Lock();

                int x1 = line.StretchablePoints[0].X;
                int y1 = line.StretchablePoints[0].Y;
                int x2 = line.StretchablePoints[1].X;
                int y2 = line.StretchablePoints[1].Y;

                bool steep = Math.Abs(y2 - y1) > Math.Abs(x2 - x1);

                if (steep)
                {
                    (x1, y1) = (y1, x1);
                    (x2, y2) = (y2, x2);
                }

                if (x1 > x2)
                {
                    (x1, x2) = (x2, x1);
                    (y1, y2) = (y2, y1);
                }

                float dx = x2 - x1;
                float dy = y2 - y1;

                float gradient = dy / dx;

                if (dx == 0) gradient = 1;

                int xPixel1 = x1;
                int xPixel2 = x2;

                float intersectY = y1;

                Color L = line.Color;
                Color B = BackGround;

                if (steep)
                {
                    for (int x = xPixel1; x <= xPixel2; x++)
                    {


                        SetPixel(iPartOfNumber(intersectY), x, C2(B, L, intersectY));
                        SetPixel(iPartOfNumber(intersectY) - 1, x, C1(B, L, intersectY));
                        intersectY += gradient;
                    }
                }
                else
                {
                    for (int x = xPixel1; x <= xPixel2; x++)
                    {
                        SetPixel(x, iPartOfNumber(intersectY), C2(B, L, intersectY));
                        SetPixel(x, iPartOfNumber(intersectY) - 1, C1(B, L, intersectY));
                        intersectY += gradient;
                    }
                }
            }
            finally
            {
                Bitmap.Unlock();
            }


        }

        public void DrawAACircle(Circle circle)
        {
            try
            {
                Bitmap.Lock();
                //Erase(circle)
                //Draw(circle);
                Color L = circle.Color;
                Color B = BackGround;
                var Center = circle.Center;
                int r = circle.Radius;
                int x = circle.Radius;
                int y = 0;
                //SetPixel(x, y, L);

                //SetPixel(Center.X + x, Center.Y + y, L);
                //SetPixel(Center.X + x - 1, Center.Y + y, L);

                //SetPixel(Center.X + y, Center.Y + x, L);
                //SetPixel(Center.X + y - 1, Center.Y + x, L);

                //SetPixel(Center.X - y, Center.Y + x, L);
                //SetPixel(Center.X - y + 1, Center.Y + x, L);

                //SetPixel(Center.X - x, Center.Y + y, L);
                //SetPixel(Center.X - x + 1, Center.Y + y, L);

                //SetPixel(Center.X - x, Center.Y - y, L);
                //SetPixel(Center.X - x + 1, Center.Y - y, L);

                //SetPixel(Center.X - y, Center.Y - x, L);
                //SetPixel(Center.X - y + 1, Center.Y - x, L);

                //SetPixel(Center.X + y, Center.Y - x, L);
                //SetPixel(Center.X + y - 1, Center.Y - x, L);

                //SetPixel(Center.X + x, Center.Y - y, L);
                //SetPixel(Center.X + x - 1, Center.Y - y, L);

                while (x > y)
                {
                    ++y;
                    x = (int)(Math.Ceiling(Math.Sqrt((r * r) - (y * y))));

                    float T = (float)(Math.Ceiling(Math.Sqrt(r * r - y * y)) - Math.Sqrt(r * r - y * y));

                    var c2 = CircleC2(B, L, T);
                    var c1 = CircleC1(B, L, T);

                    if (y > x)
                        break;


                    SetPixel(Center.X + x , Center.Y + y , c2); //7
                    SetPixel(Center.X + x  - circle.Brush.Radius -1, Center.Y + y, c1);

                    SetPixel(Center.X + y, Center.Y + x, c2); //8
                    SetPixel(Center.X + y, Center.Y + x - circle.Brush.Radius -1, c1);

                    SetPixel(Center.X - y, Center.Y + x, c2); //5
                    SetPixel(Center.X - y, Center.Y + x - circle.Brush.Radius -1, c1);

                    SetPixel(Center.X - x, Center.Y + y, c2); //6
                    SetPixel(Center.X - x + circle.Brush.Radius +1, Center.Y + y, c1);

                    SetPixel(Center.X - x, Center.Y - y, c2); //3
                    SetPixel(Center.X - x + circle.Brush.Radius+1, Center.Y - y, c1);

                    SetPixel(Center.X - y, Center.Y - x, c2); //4
                    SetPixel(Center.X - y, Center.Y - x + circle.Brush.Radius +1, c1);

                    SetPixel(Center.X + y, Center.Y - x, c2); //1
                    SetPixel(Center.X + y, Center.Y - x + circle.Brush.Radius + 1, c1);

                    SetPixel(Center.X + x, Center.Y - y, c2); //2
                    SetPixel(Center.X + x - circle.Brush.Radius -1, Center.Y - y, c1);


                }
            }
            finally
            {
                Bitmap.Unlock();
            }
        }

    }
}
