using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Rasterization.Engine
{
    public static class WriteableBitmapExtensions
    {
        public static Color GetPixel(this WriteableBitmap writeableBitmap, int x, int y)
        {
            if (y > writeableBitmap.PixelHeight - 1 || x > writeableBitmap.PixelWidth - 1)
                return Color.Empty;
            if (y < 0 || x < 0)
                return Color.Empty;

            IntPtr pBackBuffer = writeableBitmap.BackBuffer;
            int stride = writeableBitmap.BackBufferStride;

            Color colorToReturn;

            unsafe
            {
                byte* pBuffer = (byte*)pBackBuffer.ToPointer();
                int location = y * stride + x * 4;
                colorToReturn = Color.FromArgb(pBuffer[location + 3], pBuffer[location + 2], pBuffer[location + 1], pBuffer[location]);
            }

            return colorToReturn;
        }

        //public static unsafe void FloodFill(WriteableBitmap bitmap, int x, int y, Color pattern)
        //{
        //    Stack<(int, int)> myStack = new Stack<(int, int)>();
        //    myStack.Push((x, y));
        //    Color oldColor = bitmap.GetPixel( x, y);

        //     pattern.Lock();
        //    byte* scan0 = (byte*)bData.Scan0.ToPointer();
        //    byte bitsPerPixel = (byte)(Image.GetPixelFormatSize(bData.PixelFormat));

        //    bitmap.Lock();
        //    while (myStack.Count != 0)
        //    {
        //        var item = myStack.Pop();
        //        if (item.Item1 < 0 || item.Item1 >= bitmap.PixelWidth || item.Item2 < 0 || item.Item2 >= bitmap.PixelHeight)
        //            continue;

        //        var col = bitmap.GetPixel( item.Item1, item.Item2);
        //        if (col.Equals(oldColor))
        //        {
        //            System.Windows.Media.Color newColor = new System.Windows.Media.Color();
        //            byte* tmp = scan0 + (item.Item2 % bData.Height) * bData.Stride + (item.Item1 % bData.Width) * bitsPerPixel / 8;
        //            newColor.B = tmp[0];
        //            newColor.G = tmp[1];
        //            newColor.R = tmp[2];
        //            newColor.A = 255;

        //            DrawPixel(bitmap, item.Item1, item.Item2, newColor);
        //            myStack.Push((item.Item1 + 1, item.Item2));
        //            myStack.Push((item.Item1 - 1, item.Item2));
        //            myStack.Push((item.Item1, item.Item2 + 1));
        //            myStack.Push((item.Item1, item.Item2 - 1));
        //        }
        //    }
        //    pattern.Unlock();
        //    bitmap.Unlock();
        //}
        public static void SetPixel(this WriteableBitmap writeableBitmap, int x, int y, Color color)
        {
            if (y > writeableBitmap.PixelHeight - 1 || x > writeableBitmap.PixelWidth - 1)
                return;
            //    throw new Exception("Position for (x,y) is not in Bitmap");
            if (y < 0 || x < 0)
                return;
            //    throw new Exception("Position for (x,y) is not in Bitmap");

            IntPtr pBackBuffer = writeableBitmap.BackBuffer;
            int stride = writeableBitmap.BackBufferStride;

            unsafe
            {
                byte* pBuffer = (byte*)pBackBuffer.ToPointer();
                int location = y * stride + x * 4;

                pBuffer[location] = color.B;
                pBuffer[location + 1] = color.G;
                pBuffer[location + 2] = color.R;
                pBuffer[location + 3] = color.A;

            }

            writeableBitmap.AddDirtyRect(new Int32Rect(x, y, 1, 1));
        }

        public static void boundaryFill4(this WriteableBitmap bitmap,int x, int y,  Color boundary, Color newc, bool opt)
        {
            bitmap.Lock();
            Stack<(int, int)> myStack = new Stack<(int, int)>();
            myStack.Push((x, y));

            while (myStack.Count != 0)
            {
                var item = myStack.Pop();
                if (item.Item1 < 0 || item.Item1 >= bitmap.PixelWidth || item.Item2 < 0 || item.Item2 >= bitmap.PixelHeight)
                    continue;
                var col = bitmap.GetPixel(item.Item1, item.Item2);
                if(!col.CompareTo(boundary) && !col.CompareTo(newc))
                {
                    bitmap.SetPixel(item.Item1,item.Item2,newc);

                    myStack.Push((item.Item1 + 1, item.Item2));
                    myStack.Push((item.Item1 - 1, item.Item2));
                    myStack.Push((item.Item1 , item.Item2 + 1));
                    myStack.Push((item.Item1 , item.Item2 - 1));
                
                if(opt)
                    {
                    myStack.Push((item.Item1 - 1, item.Item2 - 1));
                    myStack.Push((item.Item1 + 1, item.Item2 - 1));
                    myStack.Push((item.Item1 - 1, item.Item2 + 1));
                    myStack.Push((item.Item1 + 1, item.Item2 + 1));

                    }
                }

            }
            bitmap.Unlock();
        }

        static bool CompareTo(this Color c1, Color c2)
        {
            return c1.R == c2.R && c1.G == c2.G && c1.B == c2.B;
        }
    }
}
