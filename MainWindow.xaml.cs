
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Geometry_Modeling_6
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Point3D> points3dObj = new List<Point3D>();
        List<Point> points2dObj = new List<Point>();
        List<Point> bottomCircle = new List<Point>();
        List<Point> topCircle = new List<Point>();
        List<Point> middleCircle = new List<Point>();
        List<Line> obj = new List<Line>();
        List<Path> contour = new List<Path>();
        List<Point3D> contourList3D = new List<Point3D>();
        List<Point> controurlList = new List<Point>();
        public List<Point> animatedFigureList = new List<Point>();
        BrushConverter brushConverter = new BrushConverter();
        private double pxPerCm = 15;
        private int segments = 50;
        private Point origin = new Point(100, 100);
        private Point originContour = new Point(82.5, 112.5);
        double currR =60, currH =180;
        public MainWindow()
        {
            InitializeComponent();
            ContourPointsToTexture();
            InitializeCylinder(origin.X, origin.Y, 60, 180, 45*Math.PI/180);
            CoordGrid3D();
        }
        private void InitializeCylinder(double originX, double originY, double radius, double height, double alpha)
        {
            

            for (int i = 0; i < segments; i++)
            {
                double angle = 2 * Math.PI * i / segments;
                double x = originX + radius * Math.Cos(angle);
                double y = originY + radius * Math.Sin(angle);

                
                
                Point bottomPoint = ProjectToCabinet(x, y, 0, alpha);
                bottomCircle.Add(bottomPoint);
                points3dObj.Add(new Point3D(x, y, 0));

             
            }

            for (int i = 0; i < segments; i++)
            {
                double angle = 2 * Math.PI * i / segments;
                double x = originX + radius * Math.Cos(angle);
                double y = originY + radius * Math.Sin(angle);


                
                Point topPoint = ProjectToCabinet(x, y, height, alpha);
                topCircle.Add(topPoint);
                points3dObj.Add(new Point3D(x, y, height));
            }
          

            foreach (var point in points3dObj)
            {
                points2dObj.Add(ProjectToCabinet(point, alpha));
            }

            DrawCylinder();
        }
        private void DrawCylinder()
        {
            // Малювання основ та бокових ліній
            for (int i = 0; i < segments - 1; i++)
            {
                // Нижня основа
                Line bottomEdge = new Line
                {
                    X1 = points2dObj[i].X,
                    Y1 = points2dObj[i].Y,
                    X2 = points2dObj[i + 1].X,
                    Y2 = points2dObj[i + 1].Y,
                    StrokeThickness = 1,
                    Stroke = Brushes.Black
                };
                PlotArea.Children.Add(bottomEdge);
                obj.Add(bottomEdge);

                if (i == segments - 2)
                {
                    Line connectBottomEdge = new Line
                    {
                        X1 = points2dObj[i + 1].X,
                        Y1 = points2dObj[i + 1].Y,
                        X2 = points2dObj[0].X,
                        Y2 = points2dObj[0].Y,
                        StrokeThickness = 1,
                        Stroke = Brushes.Black
                    };
                    PlotArea.Children.Add(connectBottomEdge);
                    obj.Add(connectBottomEdge);
                }

                // Верхня основа
                Line topEdge = new Line
                {
                    X1 = points2dObj[bottomCircle.Count + i].X,
                    Y1 = points2dObj[bottomCircle.Count + i].Y,
                    X2 = points2dObj[bottomCircle.Count + i + 1].X,
                    Y2 = points2dObj[bottomCircle.Count + i + 1].Y,
                    StrokeThickness = 1,
                    Stroke = Brushes.Black
                };
                PlotArea.Children.Add(topEdge);
                obj.Add(topEdge);

                // Бокова сторона
                Line sideEdge = new Line
                {
                    X1 = points2dObj[i].X,
                    Y1 = points2dObj[i].Y,
                    X2 = points2dObj[bottomCircle.Count + i].X,
                    Y2 = points2dObj[bottomCircle.Count + i].Y,
                    StrokeThickness = 1,
                    Stroke = Brushes.Gray
                };
                PlotArea.Children.Add(sideEdge);
                obj.Add(sideEdge);

                if (i == segments - 2)
                {
                    Line connectTopEdge = new Line
                    {
                        X1 = points2dObj[bottomCircle.Count + i + 1].X,
                        Y1 = points2dObj[bottomCircle.Count + i + 1].Y,
                        X2 = points2dObj[bottomCircle.Count].X,
                        Y2 = points2dObj[bottomCircle.Count].Y,
                        StrokeThickness = 1,
                        Stroke = Brushes.Black
                    };
                    PlotArea.Children.Add(connectTopEdge);
                    obj.Add(connectTopEdge);

                    Line connectSideEdge = new Line
                    {
                        X1 = points2dObj[i + 1].X,
                        Y1 = points2dObj[i + 1].Y,
                        X2 = points2dObj[bottomCircle.Count + i + 1].X,
                        Y2 = points2dObj[bottomCircle.Count + i + 1].Y,
                        StrokeThickness = 1,
                        Stroke = Brushes.Gray
                    };
                    PlotArea.Children.Add(connectSideEdge);
                    obj.Add(connectSideEdge);
                }
            }

      
            DrawParabolicCurve();
        }


        private void CoordGrid3D()
        {
            PlotArea.Children.Clear();
            foreach (Line l in obj)
                PlotArea.Children.Remove(l);
            obj.Clear();

            double originX = 50;
            double originY = 50;


            double scale = 500;

            double.TryParse(ozAlpha.Text, out double alpha);

            alpha = alpha * Math.PI / 180;
            // Лінія для осі X
            Point xStart = ProjectToCabinet(originX, originY, 0, alpha);
            Point xEnd = ProjectToCabinet(originX + scale, originY, 0, alpha);

            Line xAxis = new Line
            {
                X1 = xStart.X,
                Y1 = xStart.Y,
                X2 = xEnd.X,
                Y2 = xEnd.Y,
                StrokeThickness = 2,
                Stroke = Brushes.Teal
            };
            PlotArea.Children.Add(xAxis);

            // Лінія для осі Y
            Point yStart = ProjectToCabinet(originX, originY, 0, alpha);
            Point yEnd = ProjectToCabinet(originX, originY + scale, 0, alpha);

            Line yAxis = new Line
            {
                X1 = yStart.X,
                Y1 = yStart.Y,
                X2 = yEnd.X,
                Y2 = yEnd.Y,
                StrokeThickness = 2,
                Stroke = Brushes.Purple
            };
            PlotArea.Children.Add(yAxis);

            // Лінія для осі Z
            Point zStart = ProjectToCabinet(originX, originY, 0, alpha);
            Point zEnd = ProjectToCabinet(originX, originY, scale, alpha);

            Line zAxis = new Line
            {
                X1 = zStart.X,
                Y1 = zStart.Y,
                X2 = zEnd.X,
                Y2 = zEnd.Y,
                StrokeThickness = 2,
                Stroke = Brushes.Peru
            };
            PlotArea.Children.Add(zAxis);

            DrawCylinder();
           

        }

        private Point ProjectToCabinet(double x, double y, double z, double alpha)
        {

            double projectedX = x + 0.5 * z * Math.Cos(alpha);
            double projectedY = y + 0.5 * z * Math.Sin(alpha);

            return new Point(projectedX, projectedY);
        }
        private Point ProjectToCabinet(Point3D p, double alpha)
        {

            double projectedX = p.X + 0.5 * p.Z * Math.Cos(alpha);
            double projectedY = p.Y + 0.5 * p.Z * Math.Sin(alpha);

            return new Point(projectedX, projectedY);
        }
        public void DrawParabolicCurve()
        {
            
            for (int j = 0; j < controurlList.Count(); j += 3)
            {
                int curveIndex = j / 3;

                Path path = new Path
                {
                    Stroke = (Brush)brushConverter.ConvertFrom("#5E35B1"),
                    StrokeThickness = 2,
                    Tag = curveIndex
                };

                PathGeometry geometry = new PathGeometry();
                PathFigure figure = new PathFigure
                {
                    StartPoint = new Point(controurlList[j].X * pxPerCm, controurlList[j].Y * pxPerCm)
                };

                PolyLineSegment segment = new PolyLineSegment();

                for (double i = 0.05; i < 1; i += 0.05)
                {
                    double x = controurlList[j].X * pxPerCm * Math.Pow(1 - i, 2) +
                               2 * controurlList[j + 1].X * pxPerCm * i * (1 - i) +
                               controurlList[j + 2].X * pxPerCm * i * i;

                    double y = controurlList[j].Y * pxPerCm * Math.Pow(1 - i, 2) +
                               2 * controurlList[j + 1].Y * pxPerCm * i * (1 - i) +
                              controurlList[j + 2].Y * pxPerCm * i * i;

                    segment.Points.Add(new Point(x, y));
                }

                segment.Points.Add(new Point(controurlList[j + 2].X * pxPerCm, controurlList[j + 2].Y * pxPerCm));
                figure.Segments.Add(segment);
                geometry.Figures.Add(figure);
                path.Data = geometry;
             
                PlotArea.Children.Add(path);
                contour.Add(path);
            }


        }
        private void ContourPointsToTexture()
        {
            animatedFigureList.Clear();
            InitializeAnimatedPoints();
            contourList3D.Clear();
            controurlList.Clear();
            double.TryParse(ozAlpha.Text, out double alpha);
            alpha = alpha * Math.PI / 180;
            for (int i = 0; i< animatedFigureList.Count; i++)
            {
  
                  contourList3D.Add(PointToTexture(animatedFigureList[i].X*pxPerCm, animatedFigureList[i].Y*pxPerCm));
              
            }

            for(int i = 0;i< animatedFigureList.Count; i++)
            {
                controurlList.Add(ProjectToCabinet(contourList3D[i], alpha));
            }
          
        }

        private Point3D PointToTexture(double x, double y)
        {
            double.TryParse(S.Text, out double r);
            double.TryParse(H.Text, out double h);
            double.TryParse(du.Text, out double u);
            double.TryParse(dv.Text, out double v);
    
            double D = x;
            double T = y * Math.PI / 6;
            D *= 0.8;
            T *= 0.8;

            D += u;
            T += v;

            double X =  r * Math.Cos(T * Math.PI / 180) / pxPerCm + origin.X/pxPerCm;
            double Y =  r * Math.Sin(T * Math.PI / 180) / pxPerCm + origin.Y/pxPerCm;
            double Z =  D / pxPerCm;


            return new Point3D(X, Y, Z);
        }

       

        private void RotateContour()
        {
            var rotatedContour = new List<Point>();

            double.TryParse(angle.Text, out double angleRadians);
            angleRadians = angleRadians * Math.PI / 180;
            animatedFigureList.Clear();
            InitializeAnimatedPoints();
            foreach (var point in animatedFigureList)
            {
                // Віднімаємо центр
                double translatedX = point.X - originContour.X/pxPerCm;
                double translatedY = point.Y - originContour.Y/pxPerCm;

                // Виконуємо обертання
                double rotatedX = translatedX * Math.Cos(angleRadians) - translatedY * Math.Sin(angleRadians);
                double rotatedY = translatedX * Math.Sin(angleRadians) + translatedY * Math.Cos(angleRadians);

            
                // Додаємо центр назад
                rotatedX += originContour.X/pxPerCm;
                rotatedY += originContour.Y/pxPerCm ; 

                rotatedContour.Add(new Point(rotatedX, rotatedY));
            }
            animatedFigureList.Clear();
            // Замінюємо старий список точок новим оберненим

            for (int i = 0; i < rotatedContour.Count; i++)
            {
                animatedFigureList.Add(rotatedContour[i]);
            }
            

        }
    private Point3D RotateAroundX(Point3D point, double angle)
        {
            double radians = angle * Math.PI / 180;
            double cosAngle = Math.Cos(radians);
            double sinAngle = Math.Sin(radians);

            double y = point.Y * cosAngle - point.Z * sinAngle;
            double z = point.Y * sinAngle + point.Z * cosAngle;

            return new Point3D(point.X, y, z);
        }
        private Point3D RotateAroundY(Point3D point, double angle)
        {
            double radians = angle * Math.PI / 180;
            double cosAngle = Math.Cos(radians);
            double sinAngle = Math.Sin(radians);

            double x = point.X * cosAngle + point.Z * sinAngle;
            double z = -point.X * sinAngle + point.Z * cosAngle;

            return new Point3D(x, point.Y, z);
        }
        private Point3D RotateAroundZ(Point3D point, double angle)
        {
            double radians = angle * Math.PI / 180;
            double cosAngle = Math.Cos(radians);
            double sinAngle = Math.Sin(radians);

            double x = point.X * cosAngle - point.Y * sinAngle;
            double y = point.X * sinAngle + point.Y * cosAngle;

            return new Point3D(x, y, point.Z);
        }
        private Point3D RotatePoint(Point3D point, double angleX, double angleY, double angleZ)
        {
            Point3D rotatedPoint = RotateAroundX(point, angleX);
            rotatedPoint = RotateAroundY(rotatedPoint, angleY);
            rotatedPoint = RotateAroundZ(rotatedPoint, angleZ);

            return rotatedPoint;
        }
        private void RotateObject(double angleX, double angleY, double angleZ)
        {
            for (int i = 0; i < points3dObj.Count; i++)
            {
                points3dObj[i] = RotatePoint(points3dObj[i], angleX, angleY, angleZ);
            }
            for (int i = 0;i < contourList3D.Count; i++)
            {
                contourList3D[i] = RotatePoint(contourList3D[i], angleX, angleY, angleZ);
            }
            points2dObj.Clear();
            controurlList.Clear();
            double.TryParse(ozAlpha.Text, out double alpha);
            alpha = alpha * Math.PI / 180;

            foreach (var point in points3dObj)
            {
                points2dObj.Add(ProjectToCabinet(point, alpha));
            }
            foreach (var point in contourList3D)
            {
                controurlList.Add(ProjectToCabinet(point, alpha));
            }

        }
        private void TextBoxMoveKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                double.TryParse(ozAlpha.Text, out double alpha);
                alpha = alpha * Math.PI / 180;
                double.TryParse(FigurePosX.Text, out double dx);
                double.TryParse(FigurePosY.Text, out double dy);
                double.TryParse(FigurePosZ.Text, out double dz);
                for (int i = 0; i < points2dObj.Count; i++)
                {
                    points3dObj[i] = new Point3D(points3dObj[i].X + dx * 50, points3dObj[i].Y + dy * 50, points3dObj[i].Z + dz * 50);
                   
                    points2dObj[i] = ProjectToCabinet(points3dObj[i], alpha);
                    
                }
                foreach (Line l in obj)
                    PlotArea.Children.Remove(l);
                obj.Clear();
                foreach (Path s in contour)
                    PlotArea.Children.Remove(s);
                contour.Clear();
                for (int i = 0;i < controurlList.Count;i++)
                {
                    contourList3D[i] = new Point3D(contourList3D[i].X + (dx * 50) / pxPerCm, contourList3D[i].Y + (dy * 50) / pxPerCm, contourList3D[i].Z + dz * 50 / pxPerCm);
                    controurlList[i] = ProjectToCabinet(contourList3D[i], alpha);
                }
               

                DrawCylinder();
            }
        }

        private void TextBoxRotateKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                double.TryParse(RotatePosX.Text, out double rx);
                double.TryParse(RotatePosY.Text, out double ry);
                double.TryParse(RotatePosZ.Text, out double rz);

                RotateObject(rx, ry, rz);


        
                foreach (Line l in obj)
                    PlotArea.Children.Remove(l);
                obj.Clear();
                foreach (Path l in contour)
                    PlotArea.Children.Remove(l);
     
                DrawCylinder();
            }
        }
        private void TextBoxAxisAngleKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                double.TryParse(ozAlpha.Text, out double alpha);
                alpha = alpha * Math.PI / 180;
                for (int i = 0; i < points2dObj.Count; i++)
                    points2dObj[i] = ProjectToCabinet(points3dObj[i], alpha);
              
                foreach (Path p in contour)
                    PlotArea.Children.Remove(p);
                contour.Clear();
                ContourPointsToTexture();
                CoordGrid3D();
            }
        }
        private void TextBoxFigureChangeKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                double.TryParse(ozAlpha.Text, out double alpha);
                alpha = alpha * Math.PI / 180;

                double.TryParse(S.Text, out double s);

                double.TryParse(H.Text, out double h);

                currR = s;
                currH = h;
                points3dObj.Clear();
                points2dObj.Clear();
                bottomCircle.Clear();
                topCircle.Clear();
                foreach (Line l in obj)
                    PlotArea.Children.Remove(l);
                obj.Clear();
                foreach (Path p in contour)
                    PlotArea.Children.Remove(p);
                contour.Clear();
                ContourPointsToTexture();
                InitializeCylinder(origin.X, origin.Y, s, h, alpha);


            }
        }

        private void TextBoxRotateContour(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
              
               
                RotateContour();
                foreach (Path p in contour)
                    PlotArea.Children.Remove(p);
                contour.Clear();

                contourList3D.Clear();
                controurlList.Clear();
                double.TryParse(ozAlpha.Text, out double alpha);
                alpha = alpha * Math.PI / 180;
                
                for (int i = 0; i < animatedFigureList.Count; i++)
                {

                    contourList3D.Add(PointToTexture(animatedFigureList[i].X * pxPerCm, animatedFigureList[i].Y * pxPerCm));

                }

                for (int i = 0; i < animatedFigureList.Count; i++)
                {
                    controurlList.Add(ProjectToCabinet(contourList3D[i], alpha));
                }

                DrawParabolicCurve();
               
            }
        }



        private void AnimateCylinderScaling(
  double targetScaleRadius, double targetScaleHeight,
  double alpha, int animationSteps, int animationDelay)
        {
            // Початкові розміри
            double initialRadius = currR;
            double initialHeight = currH;

            // Кроки зміни розмірів
            double deltaScaleRadius = (targetScaleRadius - initialRadius) / animationSteps;
            double deltaScaleHeight = (targetScaleHeight - initialHeight) / animationSteps;

            int currentStep = 0;
            bool isReturning = false;

            // Таймер для виконання анімації
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(animationDelay)
            };

            timer.Tick += (s, e) =>
            {
                if (currentStep > animationSteps)
                {
                    if (!isReturning)
                    {
                        // Почати зворотну анімацію
                        isReturning = true;
                        currentStep = 0;
                        deltaScaleRadius = -deltaScaleRadius; // Змінюємо напрямок
                        deltaScaleHeight = -deltaScaleHeight; // Змінюємо напрямок
                    }
                    else
                    {
                        // Завершення анімації
                        timer.Stop();
                        return;
                    }
                }

                // Розрахунок нових розмірів
                double scaleRadius = currR + deltaScaleRadius;
                double scaleHeight = currH + deltaScaleHeight;

                // Оновлення розмірів
                currR = scaleRadius;
                currH = scaleHeight;

                // Відображення поточних значень (для наочності)
                S.Text = currR.ToString();
                H.Text = currH.ToString();

                // Перетворення кута в радіани
                double.TryParse(ozAlpha.Text, out double alphaRadians);
                alphaRadians = alpha * Math.PI / 180;

                // Очистка та оновлення фігури
                points3dObj.Clear();
                points2dObj.Clear();
                bottomCircle.Clear();
                topCircle.Clear();

                foreach (Line l in obj)
                    PlotArea.Children.Remove(l);
                obj.Clear();

                foreach (Path p in contour)
                    PlotArea.Children.Remove(p);
                contour.Clear();

                // Побудова нового циліндра
                ContourPointsToTexture();
                InitializeCylinder(origin.X, origin.Y, scaleRadius, scaleHeight, alphaRadians);

                currentStep++;
            };

            timer.Start();
        }



        private void StartAnimation_Click(object sender, RoutedEventArgs e)
        {
            double.TryParse(ozAlpha.Text, out double alpha);


            double.TryParse(FigurePosX.Text, out double tx);
            double.TryParse(FigurePosY.Text, out double ty);
            double.TryParse(FigurePosZ.Text, out double tz);

            double.TryParse(RotatePosX.Text, out double rx);
            double.TryParse(RotatePosY.Text, out double ry);
            double.TryParse(RotatePosZ.Text, out double rz);

            double.TryParse(S.Text, out double s);
            double.TryParse(H.Text, out double h);


        AnimateCylinderScaling(
                
                s, h, alpha, 100, 10);

            

           
        }

        internal void InitializeAnimatedPoints()
        {
            animatedFigureList.Add(new Point(4.5, 2.2));
            animatedFigureList.Add(new Point(5.7, 1.8));//1
            animatedFigureList.Add(new Point(6.2, 2.2));

            animatedFigureList.Add(new Point(3, 5.5));
            animatedFigureList.Add(new Point(3.8, 4.8));//2
            animatedFigureList.Add(new Point(4.5, 2));

            animatedFigureList.Add(new Point(6.2, 2.2));
            animatedFigureList.Add(new Point(6.5, 4.5));//3
            animatedFigureList.Add(new Point(7.5, 5.5));

            animatedFigureList.Add(new Point(7, 4.8));
            animatedFigureList.Add(new Point(7.3, 5));//4
            animatedFigureList.Add(new Point(6.8, 5.5));

            animatedFigureList.Add(new Point(5.8, 6));
            animatedFigureList.Add(new Point(5, 5.5));//5
            animatedFigureList.Add(new Point(5.2, 4.5));

            animatedFigureList.Add(new Point(5, 6));
            animatedFigureList.Add(new Point(5.3, 6));//6
            animatedFigureList.Add(new Point(5.4, 5.4));

            animatedFigureList.Add(new Point(9, 5.8));
            animatedFigureList.Add(new Point(11.5, 4));//7
            animatedFigureList.Add(new Point(9, 2.5));

            animatedFigureList.Add(new Point(1.8, 5.8));
            animatedFigureList.Add(new Point(0, 4));//8
            animatedFigureList.Add(new Point(1.9, 2.5));

            animatedFigureList.Add(new Point(2.5, 7.9));
            animatedFigureList.Add(new Point(0.8, 5));//9
            animatedFigureList.Add(new Point(3, 2.3));

            animatedFigureList.Add(new Point(8.1, 8));
            animatedFigureList.Add(new Point(10, 5.5));//10
            animatedFigureList.Add(new Point(8, 2.5));

            animatedFigureList.Add(new Point(9, 2.5));
            animatedFigureList.Add(new Point(10, 2));//11
            animatedFigureList.Add(new Point(8, 1.5));

            animatedFigureList.Add(new Point(8.9, 2.3));
            animatedFigureList.Add(new Point(9.3, 1.8));//12
            animatedFigureList.Add(new Point(8.4, 1.5));

            animatedFigureList.Add(new Point(8.3, 2.3));
            animatedFigureList.Add(new Point(8.8, 1.3));//13
            animatedFigureList.Add(new Point(8, 1.5));

            animatedFigureList.Add(new Point(7.5, 2.1));
            animatedFigureList.Add(new Point(7.2, 2.5));//14
            animatedFigureList.Add(new Point(6.8, 2.3));

            animatedFigureList.Add(new Point(8, 2.5));
            animatedFigureList.Add(new Point(8.2, 1.1));//15
            animatedFigureList.Add(new Point(7.8, 1.3));

            animatedFigureList.Add(new Point(7.5, 1.5));
            animatedFigureList.Add(new Point(7.2, 0.9));//16
            animatedFigureList.Add(new Point(7, 1.7));

            animatedFigureList.Add(new Point(7, 1.2));
            animatedFigureList.Add(new Point(6.4, 0.8));//17
            animatedFigureList.Add(new Point(6.5, 1.9));

            animatedFigureList.Add(new Point(6.3, 2.1));
            animatedFigureList.Add(new Point(5.5, 1.5));//18
            animatedFigureList.Add(new Point(6.3, 1.3));

            animatedFigureList.Add(new Point(4.5, 2));
            animatedFigureList.Add(new Point(5, 1.5));//19
            animatedFigureList.Add(new Point(4.3, 1.1));

            animatedFigureList.Add(new Point(4.5, 1.8));
            animatedFigureList.Add(new Point(4.1, 0.6));//20
            animatedFigureList.Add(new Point(3.9, 1.6));

            animatedFigureList.Add(new Point(4, 1));
            animatedFigureList.Add(new Point(3.2, 0.8));//21
            animatedFigureList.Add(new Point(3.2, 1.8));

            animatedFigureList.Add(new Point(3.1, 2.2));
            animatedFigureList.Add(new Point(2.3, 1));//22
            animatedFigureList.Add(new Point(3.5, 1));

            animatedFigureList.Add(new Point(3.2, 2.2));
            animatedFigureList.Add(new Point(3.8, 2.5));//23
            animatedFigureList.Add(new Point(4.2, 2.2));

            animatedFigureList.Add(new Point(3, 1));
            animatedFigureList.Add(new Point(2.1, 0.9));//24
            animatedFigureList.Add(new Point(2.5, 2));

            animatedFigureList.Add(new Point(2.5, 1.3));
            animatedFigureList.Add(new Point(1.7, 1));//25
            animatedFigureList.Add(new Point(2, 2));

            animatedFigureList.Add(new Point(2, 1.5));
            animatedFigureList.Add(new Point(1, 1.8));//26
            animatedFigureList.Add(new Point(1.9, 2.5));

            animatedFigureList.Add(new Point(3, 12.5));
            animatedFigureList.Add(new Point(5, 13));//27
            animatedFigureList.Add(new Point(3.9, 10.8));

            animatedFigureList.Add(new Point(7, 12.5));
            animatedFigureList.Add(new Point(5.2, 13));//28
            animatedFigureList.Add(new Point(6.5, 10.8));

            animatedFigureList.Add(new Point(2.5, 7.8));
            animatedFigureList.Add(new Point(1.2, 8));//29
            animatedFigureList.Add(new Point(1.8, 10));

            animatedFigureList.Add(new Point(8.1, 8));
            animatedFigureList.Add(new Point(9.5, 9));//30
            animatedFigureList.Add(new Point(8.8, 10.3));

            animatedFigureList.Add(new Point(3.5, 14.1));
            animatedFigureList.Add(new Point(5.2, 15));//31
            animatedFigureList.Add(new Point(7.1, 14));

            animatedFigureList.Add(new Point(7, 14));
            animatedFigureList.Add(new Point(7.5, 17.1));//32
            animatedFigureList.Add(new Point(9.3, 16.6));

            animatedFigureList.Add(new Point(3.5, 14));
            animatedFigureList.Add(new Point(0.8, 18.2));//33
            animatedFigureList.Add(new Point(0.4, 13.8));

            animatedFigureList.Add(new Point(0.4, 13.8));
            animatedFigureList.Add(new Point(0.4, 12.6));//34
            animatedFigureList.Add(new Point(0.7, 12.1));

            animatedFigureList.Add(new Point(0.7, 12.2));
            animatedFigureList.Add(new Point(0.5, 11.7));//35
            animatedFigureList.Add(new Point(1.2, 12));

            animatedFigureList.Add(new Point(1.1, 12));
            animatedFigureList.Add(new Point(1.1, 11.5));//36
            animatedFigureList.Add(new Point(1.5, 11.4));

            animatedFigureList.Add(new Point(1.5, 11.5));
            animatedFigureList.Add(new Point(2, 10.5));//37
            animatedFigureList.Add(new Point(1.9, 9.9));

            animatedFigureList.Add(new Point(9.2, 16.7));
            animatedFigureList.Add(new Point(10.2, 12.8));//38
            animatedFigureList.Add(new Point(9, 12));

            animatedFigureList.Add(new Point(9, 12));
            animatedFigureList.Add(new Point(9.2, 11));//39
            animatedFigureList.Add(new Point(8.5, 10.2));

            animatedFigureList.Add(new Point(3.5, 7.9));
            animatedFigureList.Add(new Point(4.3, 7.2));//40
            animatedFigureList.Add(new Point(4.8, 7.5));

            animatedFigureList.Add(new Point(4.8, 7.5));
            animatedFigureList.Add(new Point(5.2, 7));//41
            animatedFigureList.Add(new Point(5.8, 7.5));

            animatedFigureList.Add(new Point(5.8, 7.5));
            animatedFigureList.Add(new Point(6.2, 7.2));//42
            animatedFigureList.Add(new Point(6.9, 7.8));

            animatedFigureList.Add(new Point(2.5, 8.3));
            animatedFigureList.Add(new Point(5.1, 13));//43
            animatedFigureList.Add(new Point(8, 8.5));

            animatedFigureList.Add(new Point(5.2, 9.2));
            animatedFigureList.Add(new Point(2.8, 6.8));//44
            animatedFigureList.Add(new Point(2.5, 8.5));

            animatedFigureList.Add(new Point(8, 8.5));
            animatedFigureList.Add(new Point(7.3, 6.5));//45
            animatedFigureList.Add(new Point(5.2, 9.2));

            animatedFigureList.Add(new Point(3.2, 9));
            animatedFigureList.Add(new Point(5.1, 12));//46
            animatedFigureList.Add(new Point(7.1, 9));

            animatedFigureList.Add(new Point(4.2, 9.8));
            animatedFigureList.Add(new Point(5.3, 10.8));//47
            animatedFigureList.Add(new Point(6, 9.8));

            animatedFigureList.Add(new Point(4.2, 9.8));
            animatedFigureList.Add(new Point(5.5, 8.5));//48
            animatedFigureList.Add(new Point(6, 9.8));

            animatedFigureList.Add(new Point(3.3, 12.1));
            animatedFigureList.Add(new Point(2.3, 12.7));//49
            animatedFigureList.Add(new Point(2.1, 11));

            animatedFigureList.Add(new Point(7, 12));
            animatedFigureList.Add(new Point(8.3, 12.5));//50
            animatedFigureList.Add(new Point(8.4, 11));

            animatedFigureList.Add(new Point(2.1, 10.5));
            animatedFigureList.Add(new Point(3, 11.7));//51
            animatedFigureList.Add(new Point(3.5, 10.5));

            animatedFigureList.Add(new Point(7, 10.7));
            animatedFigureList.Add(new Point(7.7, 11.7));//52
            animatedFigureList.Add(new Point(8.3, 10.7));

            animatedFigureList.Add(new Point(2.6, 14));
            animatedFigureList.Add(new Point(0.5, 16.2));//53
            animatedFigureList.Add(new Point(1, 13.8));

            animatedFigureList.Add(new Point(8, 14));
            animatedFigureList.Add(new Point(8.2, 16.2));//54
            animatedFigureList.Add(new Point(9.1, 15.8));

            animatedFigureList.Add(new Point(7.8, 10.3));
            animatedFigureList.Add(new Point(8.1, 10.3));//55
            animatedFigureList.Add(new Point(8.4, 10.7));

            animatedFigureList.Add(new Point(2.1, 10.6));
            animatedFigureList.Add(new Point(2.5, 10));//56
            animatedFigureList.Add(new Point(2.9, 10));
        }


    }
}