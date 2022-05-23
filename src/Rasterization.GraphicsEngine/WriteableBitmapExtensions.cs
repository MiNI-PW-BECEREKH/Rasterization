using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
