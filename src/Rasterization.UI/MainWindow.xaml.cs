using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Rasterization.Engine;
using System.Drawing;
using Point = System.Drawing.Point;
using Color = System.Drawing.Color;
using System.Windows.Controls.Primitives;
using Rasterization.UI.ConversionExtension;

namespace Rasterization.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    //TODO: CLEAN THIS UP WHEN U HAVE TIME IT GOT VERY MESSY. YOU WILL USE IT FOR PROJ 4 
    public partial class MainWindow : Window
    {
        List<Point> CachedPoints = new List<Point>();
        IGraphicsEngine GE;
        List<IDrawable> Drawables = new List<IDrawable>();
        Color tempColor = Color.FromArgb(0, 0, 0);

        Point MousePointRegister;

        IDrawable SelectedDrawable;
        public MainWindow()
        {
            InitializeComponent();

        }


        private void ToolSetClick(object sender, RoutedEventArgs e)
        {
            LineDrawingButton.IsChecked = false;
            GrabToolButton.IsChecked = false;
            CircleDrawingButton.IsChecked = false;
            PolygonDrawingButton.IsChecked = false;
            ArcDrawingButton.IsChecked = false;
            WuLineDrawingButton.IsChecked = false;
            WuCircleDrawingButton.IsChecked = false;
            try
            {
                var btn = (ToggleButton)sender;
                btn.IsChecked = true;
                if (SelectedDrawable != null)
                {
                    SelectedDrawable.Erase(GE);
                    SelectedDrawable.Draw(GE);
                    SelectedDrawable = null;
                }

            }
            catch { }
        }

        private void ToolSetUnClick(object sender, RoutedEventArgs e)
        {
            CachedPoints.Clear();
        }

        private void ImageCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var p = e.GetPosition(ImageCanvas);
            var pointWithInts = new Point((int)p.X, (int)p.Y);
            //used when mouse is moved
            try
            {
                if ((bool)LineDrawingButton.IsChecked)
                {
                    CachedPoints.Add(pointWithInts);

                    if (CachedPoints.Count == 2)
                    {
                        Line line = new Line(CachedPoints[0], CachedPoints[1], ((System.Windows.Media.Color)ColorPicker.SelectedColor).ToColor(), (int)BrushInput.Value);
                        CachedPoints.Clear();
                        line.CalculatePoints();
                        Drawables.Add(line);
                        line.Draw(GE);

                    }
                }
                if ((bool)WuLineDrawingButton.IsChecked)
                {
                    CachedPoints.Add(pointWithInts);

                    if (CachedPoints.Count == 2)
                    {
                        Line line = new Line(CachedPoints[0], CachedPoints[1], ((System.Windows.Media.Color)ColorPicker.SelectedColor).ToColor(), (int)BrushInput.Value);
                        CachedPoints.Clear();
                        Drawables.Add(line);
                        line.DrawAA(GE);

                    }
                }
                if ((bool)CircleDrawingButton.IsChecked)
                {
                    CachedPoints.Add(pointWithInts);

                    if (CachedPoints.Count == 2)
                    {
                        Circle circle = new Circle(CachedPoints[0], CachedPoints[1], ((System.Windows.Media.Color)ColorPicker.SelectedColor).ToColor(), (int)BrushInput.Value);
                        CachedPoints.Clear();
                        circle.CalculatePoints();
                        Drawables.Add(circle);
                        circle.Draw(GE);

                    }
                }
                if ((bool)WuCircleDrawingButton.IsChecked)
                {
                    CachedPoints.Add(pointWithInts);

                    if (CachedPoints.Count == 2)
                    {
                        Circle circle = new Circle(CachedPoints[0], CachedPoints[1], ((System.Windows.Media.Color)ColorPicker.SelectedColor).ToColor(), (int)BrushInput.Value);
                        CachedPoints.Clear();
                        Drawables.Add(circle);
                        circle.DrawAA(GE);

                    }
                }
                if ((bool)PolygonDrawingButton.IsChecked)
                {
                    var initialPoint = CachedPoints.FirstOrDefault();
                    if(((int)Math.Sqrt(Math.Pow(initialPoint.X - pointWithInts.X, 2) + Math.Pow(initialPoint.Y - pointWithInts.Y, 2)) < 50))
                    {
                        //CachedPoints.Add(CachedPoints.First());
                        Polygon polygon= new Polygon(CachedPoints, ((System.Windows.Media.Color)ColorPicker.SelectedColor).ToColor(), (int)BrushInput.Value);
                        polygon.CalculatePoints();
                        Drawables.Add(polygon);
                        polygon.Draw(GE);
                        CachedPoints.Clear();
                    }
                    else
                        CachedPoints.Add(pointWithInts);
                    
                }
                if ((bool)ArcDrawingButton.IsChecked)
                {
                    CachedPoints.Add(pointWithInts);

                    if (CachedPoints.Count == 3)
                    {
                        Arc arc = new Arc(CachedPoints);
                        CachedPoints.Clear();
                        arc.CalculatePoints();
                        Drawables.Add(arc);
                        arc.Draw(GE);

                    }
                }
                if ((bool)GrabToolButton.IsChecked)
                {
                    foreach (var drawable in Drawables)
                    {
                        if (drawable.HitTest(pointWithInts))
                        {
                            if (SelectedDrawable != null)
                            {
                                SelectedDrawable.Erase(GE);
                                SelectedDrawable.Draw(GE);
                            }
                            SelectedDrawable = drawable;
                            SelectedDrawable.IndicateSelection(GE);
                            //MessageBox.Show("Found Something");
                        }
                    }
                }

                GC.Collect();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PixelFormat pf = PixelFormats.Bgra32;
            int width = 1200/2;
            int height = 1200/2;
            int rawStride = (width * pf.BitsPerPixel + 7) / 8;
            byte[] rawImage = new byte[rawStride * height];
            rawImage = Enumerable.Repeat((byte)240, rawImage.Length).ToArray();

            BitmapSource bitmap = BitmapSource.Create(width, height,
                96, 96, pf, null,
                rawImage, rawStride);
            var writeable = new WriteableBitmap(bitmap);
            ImageCanvas.Source = writeable;
            GE = new GraphicsEngine(writeable);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedDrawable != null)
            {
                ToolSetClick(sender, e);

                SelectedDrawable.Erase(GE);
                Drawables.Remove(SelectedDrawable);
                SelectedDrawable = null;
            }
        }

        private void ImageCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            var p = e.GetPosition(ImageCanvas);
            var pointWithInts = new Point((int)p.X, (int)p.Y);
            if (SelectedDrawable != null && e.LeftButton == MouseButtonState.Pressed && SelectedDrawable.StretchablePoints.Any(point => Math.Sqrt(Math.Pow(pointWithInts.X - point.X, 2) + Math.Pow(pointWithInts.Y - point.Y, 2)) < 150))
            {
                var min = 35000;
                int closestPointIdx = 0;
                foreach (var pt in SelectedDrawable.StretchablePoints)
                {
                    var minToCheck = (int)Math.Sqrt(Math.Pow(pointWithInts.X - pt.X, 2) + Math.Pow(pointWithInts.Y - pt.Y, 2));
                    var isNewMin = minToCheck < min;
                    if (isNewMin)
                    {
                        closestPointIdx = SelectedDrawable.StretchablePoints.IndexOf(pt);
                        min = minToCheck;
                    }
                }

                var dx = (pointWithInts.X - MousePointRegister.X);
                var dy = (pointWithInts.Y - MousePointRegister.Y);
                //dx *= dx / dy;
                //dy *= dx / dy;
                SelectedDrawable.Erase(GE);
                SelectedDrawable.Stretch(GE, dx, dy, closestPointIdx);
                MousePointRegister = pointWithInts;
                Drawables.Where(x => x != SelectedDrawable).ToList().ForEach(x => x.Draw(GE));
            }

            if (SelectedDrawable != null && e.MiddleButton == MouseButtonState.Pressed && SelectedDrawable.StretchablePoints.Any(p => Math.Sqrt(Math.Pow(pointWithInts.X - p.X, 2) + Math.Pow(pointWithInts.Y - p.Y, 2)) < 50))
            {
                var min = 35000;
                int closestPointIdx = 0;
                foreach (var pt in SelectedDrawable.StretchablePoints)
                {
                    var minToCheck = (int)Math.Sqrt(Math.Pow(pointWithInts.X - pt.X, 2) + Math.Pow(pointWithInts.Y - pt.Y, 2));
                    var isNewMin = minToCheck < min;
                    if (isNewMin)
                    {
                        closestPointIdx = SelectedDrawable.StretchablePoints.IndexOf(pt);
                        min = minToCheck;
                    }
                }

                var dx = (pointWithInts.X - MousePointRegister.X);
                var dy = (pointWithInts.Y - MousePointRegister.Y);
                //dx *= dx / dy;
                //dy *= dx / dy;
                SelectedDrawable.Erase(GE);
                SelectedDrawable.Move(GE, dx, dy, closestPointIdx);
                MousePointRegister = pointWithInts;
                Drawables.Where(x => x != SelectedDrawable).ToList().ForEach(x => x.Draw(GE));

            }
        }

        private void ImageCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var p = e.GetPosition(ImageCanvas);
            var pointWithInts = new Point((int)p.X, (int)p.Y);
            //used when mouse is moved
            MousePointRegister = pointWithInts;
        }

        private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            if (SelectedDrawable != null)
            {
                SelectedDrawable.Color = ((System.Windows.Media.Color)ColorPicker.SelectedColor).ToColor();
                SelectedDrawable.Brush.Color = ((System.Windows.Media.Color)ColorPicker.SelectedColor).ToColor();
                SelectedDrawable.IndicateSelection(GE);

            }
        }

        private void BrushInput_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if(SelectedDrawable != null)
            {
                SelectedDrawable.Erase(GE);
                SelectedDrawable.Brush.Radius = (int)e.NewValue;
                SelectedDrawable.CalculateBrush();
                SelectedDrawable.IndicateSelection(GE);
            }
        }

        private void SuperSample_Click(object sender, RoutedEventArgs e)
        {

            PixelFormat pf = PixelFormats.Bgra32;
            int width3 = 1200;
            int height3 = 1200;
            int rawStride2 = (width3 * pf.BitsPerPixel + 7) / 8;
            byte[] rawImage2 = new byte[rawStride2 * height3];
            rawImage2 = Enumerable.Repeat((byte)240, rawImage2.Length).ToArray();

            BitmapSource bitmap = BitmapSource.Create(width3, height3,
                96, 96, pf, null,
                rawImage2, rawStride2);
            var writeable = new WriteableBitmap(bitmap);
            WriteableBitmap readBmp = (WriteableBitmap)ImageCanvas.Source;


            GE.Bitmap = writeable;
            foreach(var vector in Drawables)
            {

                    vector.UpScale(GE);

            }



            int width = 1200/2;
            int height = 1200/2;
            int rawStride = (width * pf.BitsPerPixel + 7) / 8;
            byte[] rawImage = new byte[rawStride * height];
            rawImage = Enumerable.Repeat((byte)240, rawImage.Length).ToArray();

            BitmapSource bitmap2 = BitmapSource.Create(width, height,
                96, 96, pf, null,
                rawImage, rawStride);
            var writeable2 = new WriteableBitmap(bitmap2);
            readBmp = writeable.Clone();
            List<Point> pts = new();
            try
            {
                readBmp.Lock();
                writeable2.Lock();

                unsafe
                {
                    var width2 = readBmp.PixelWidth;
                    var height2 = readBmp.PixelHeight;

                    unsafe
                    {
                        for (int col = 0; col < 1200 ; col++)
                        {
                            for (int row = 0; row < 1200 ; row++)
                            {
                                Color pixelColor = Color.Empty;
                                List<int> redChannelValues = new();
                                List<int> blueChannelValues = new();
                                List<int> greenChannelValues = new();


                                for (int kernelCol = 0; kernelCol < 2; kernelCol++)
                                {
                                    for (int kernelRow = 0; kernelRow < 2; kernelRow++)
                                    {
                                        //TODO: If you have time after doing everything try MIRRORING for edges
                                        var pixelX = col + kernelCol - 1;
                                        var pixelY = row + kernelRow - 1;
                                        //pixelColor = writeableBitmap.GetPixel(pixelX, pixelY);

                                        //var kernelIntensity = kernel.Matrix[kernelRow, kernelCol];

                                        //Extend for out of bound ones

                                        if (pixelX < 0)
                                            pixelX = 0;

                                        if (pixelX >= readBmp.PixelWidth)
                                            pixelX = readBmp.PixelWidth - 1;

                                        if (pixelY < 0)
                                            pixelY = 0;

                                        if (pixelY >= readBmp.PixelHeight)
                                            pixelY = readBmp.PixelHeight - 1;


                                        pixelColor = readBmp.GetPixel(pixelX, pixelY);
                                        //Console.WriteLine($"Kernel({kernelCol},{kernelRow})*Bitmap({pixelX},{pixelY})");
                                        redChannelValues.Add(pixelColor.R);
                                        blueChannelValues.Add(pixelColor.B);
                                        greenChannelValues.Add(pixelColor.G);


                                    }
                                }
                                 
                                var red = redChannelValues.Average();
                                var blue = blueChannelValues.Average();
                                var green = greenChannelValues.Average();
                                //here get median
                                    writeable2.SetPixel(col/2 , row/2 , Color.FromArgb((int)red, (int)green, (int)blue));
                            }
                        }
                    }

                }

            }
            finally
            {
                readBmp.Unlock();
                writeable2.Unlock();

            }

            ImageCanvas.Source = writeable2;
            GE.Bitmap = (WriteableBitmap)ImageCanvas.Source;

        }
    }
}
