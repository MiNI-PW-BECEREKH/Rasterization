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
                        Line line = new Line(CachedPoints[0], CachedPoints[1], ((System.Windows.Media.Color)ColorPicker.SelectedColor).ToColor());
                        CachedPoints.Clear();
                        line.CalculatePoints();
                        Drawables.Add(line);
                        line.Draw(GE);

                    }
                }
                if((bool)CircleDrawingButton.IsChecked)
                {
                    CachedPoints.Add(pointWithInts);

                    if (CachedPoints.Count == 2)
                    {
                        Circle circle = new Circle(CachedPoints[0], CachedPoints[1], ((System.Windows.Media.Color)ColorPicker.SelectedColor).ToColor());
                        CachedPoints.Clear();
                        circle.CalculatePoints();
                        Drawables.Add(circle);
                        circle.Draw(GE);

                    }
                }
                if((bool)PolygonDrawingButton.IsChecked)
                {
                    if(CachedPoints.Any(x => (int)Math.Sqrt(Math.Pow(x.X - pointWithInts.X, 2) + Math.Pow(x.Y - pointWithInts.Y, 2)) < 50))
                    {
                        CachedPoints.Add(CachedPoints.First());
                        Polygon polygon= new Polygon(CachedPoints, ((System.Windows.Media.Color)ColorPicker.SelectedColor).ToColor());
                        polygon.CalculatePoints();
                        Drawables.Add(polygon);
                        polygon.Draw(GE);
                    }
                    else
                        CachedPoints.Add(pointWithInts);
                    
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
            int width = 600;
            int height = 600;
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
            if (SelectedDrawable != null && e.LeftButton == MouseButtonState.Pressed && SelectedDrawable.StretchablePoints.Any(p => Math.Sqrt(Math.Pow(pointWithInts.X - p.X, 2) + Math.Pow(pointWithInts.Y - p.Y, 2)) < 100))
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

            if (SelectedDrawable != null && e.MiddleButton == MouseButtonState.Pressed && SelectedDrawable.StretchablePoints.Any(p => Math.Sqrt(Math.Pow(pointWithInts.X - p.X, 2) + Math.Pow(pointWithInts.Y - p.Y, 2)) < 100))
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
                SelectedDrawable.IndicateSelection(GE);

            }
        }
    }
}
