using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rasterization.Engine
{
    public class Polygon : IDrawable
    {
        public Color Color { get ; set ; }
        public List<Point> StretchablePoints { get ; set ; }
        public List<Point> Points { get ; set ; }

        public void CalculatePoints()
        {
            throw new NotImplementedException();
        }

        public void Draw(IGraphicsEngine engine)
        {
            throw new NotImplementedException();
        }

        public void Erase(IGraphicsEngine engine)
        {
            throw new NotImplementedException();
        }

        public bool HitTest(Point p)
        {
            throw new NotImplementedException();
        }

        public void IndicateSelection(IGraphicsEngine engine)
        {
            throw new NotImplementedException();
        }

        public void Move(IGraphicsEngine engine, int dx, int dy, int idx)
        {
            throw new NotImplementedException();
        }

        public void Stretch(IGraphicsEngine engine, int dx, int dy, int idx)
        {
            throw new NotImplementedException();
        }
    }
}
