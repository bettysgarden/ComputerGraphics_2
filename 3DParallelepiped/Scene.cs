using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace KG2
{
    class Scene
    {
        public Color brush = Color.RoyalBlue;
        public Bitmap pic;
        public int height, width;
        public Camera cam;
        double del; 
        public List<Point> verts;
        public List<Triangle> polys;
        public Point lightPoint;
        double[,] Zbuffer;

        public Scene(int aheight, int awidth)
        {
            height = aheight;
            width = awidth;
            pic = new Bitmap(width, height);
            verts = new List<Point>();
            polys = new List<Triangle>();
        }
        public void addCylinder(double radius, double height)
        {
            List<Point> circle = new List<Point>(); // Окружность при z=0
            int step = 15; // шаг по окружности, в градусах
            for (int i = 0; i < 360; i += step)
            {
                double rad = i * Math.PI / 180;
                circle.Add(new Point(radius * Math.Cos(rad), radius * Math.Sin(rad), 0));
            }

            List<Point> circleTop = new List<Point>(); // Окружность сверху
            List<Point> circleBottom = new List<Point>(); // Окружность снизу
            for (int i = 0; i < circle.Count; i++)
            {
                circleTop.Add(new Point(circle[i].X, circle[i].Y, height / 2));
                circleBottom.Add(new Point(circle[i].X, circle[i].Y, -height / 2));
            }

            for (int i = 0; i < circle.Count - 1; i++) // Добавление боковых поверхностей
            {
                polys.Add(new Triangle(circle[i], circle[i + 1], circleTop[i], brush));
                polys.Add(new Triangle(circle[i + 1], circleTop[i + 1], circleTop[i], brush));
                polys.Add(new Triangle(circle[i + 1], circle[i], circleBottom[i], brush));
                polys.Add(new Triangle(circleBottom[i + 1], circle[i + 1], circleBottom[i], brush));
            }
            polys.Add(new Triangle(circle[circle.Count - 1], circle[0], circleTop[circleTop.Count - 1], brush));
            polys.Add(new Triangle(circle[0], circleTop[0], circleTop[circleTop.Count - 1], brush));
            polys.Add(new Triangle(circle[0], circle[circle.Count - 1], circleBottom[circleBottom.Count - 1], brush));
            polys.Add(new Triangle(circleBottom[0], circle[0], circleBottom[circleBottom.Count - 1], brush));

            verts.Add(new Point(0, 0, height / 2)); // Верхняя точка
            for (int i = 0; i < circleTop.Count - 1; i++)
            {
                polys.Add(new Triangle(circleTop[i], circleTop[i + 1], verts[0], brush)); // Поверхность от верха до 1/4
            }
            polys.Add(new Triangle(circleTop[circleTop.Count - 1], circleTop[0], verts[0], brush));

            verts.Add(new Point(0, 0, -height / 2)); // нижняя точка
            for (int i = 0; i < circleBottom.Count - 1; i++)
            {
                polys.Add(new Triangle(circleBottom[i + 1], circleBottom[i], verts[1], brush)); // Поверхность от низа до 3/4
            }
            polys.Add(new Triangle(circleBottom[0], circleBottom[circleBottom.Count - 1], verts[1], brush));

            verts.AddRange(circle);
            verts.AddRange(circleTop);
            verts.AddRange(circleBottom);
        }

            public void addCamera(Camera acam)
        {
            cam = acam;
            del = 1 / Math.Tan(cam.angle / 2); // отношение z к половине ширины
        }

        public System.Drawing.Point convertToScreenPoint(Point v)
        {
            System.Drawing.Point result = new System.Drawing.Point();
            result.X = (int)Math.Round(v.Vcam.getX() / v.Vcam.getZ() * del * (double)width / 2 + (double)width / 2); //преобразовываем в экранные координаты
            result.Y = (int)Math.Round(v.Vcam.getY() / v.Vcam.getZ() * del * (double)width / 2 + (double)height / 2);
            return result;
        }

        // проверяет, находится ли точка в треугольнике или нет
        public bool CheckPoint(System.Drawing.Point pnt, List<System.Drawing.Point> pnts)
        {
            Point v0, v1, v2;
            v0 = new Point(pnt).minus(new Point(pnts[0]));
            v1 = new Point(pnts[1]).minus(new Point(pnts[0]));
            v2 = new Point(pnts[2]).minus(new Point(pnts[0]));
            if (Math.Abs(v2.findAngle(v1) - v2.findAngle(v0) - v1.findAngle(v0)) > 1e-3)
                return false;

            v0 = new Point(pnt).minus(new Point(pnts[1]));
            v1 = new Point(pnts[0]).minus(new Point(pnts[1]));
            v2 = new Point(pnts[2]).minus(new Point(pnts[1]));
            if (Math.Abs(v2.findAngle(v1) - v2.findAngle(v0) - v1.findAngle(v0)) > 1e-3)
                return false;

            return true;
        }

        // Находит точку на грани, которая смотрит в заданную точку на экране
        public Point findVertex(System.Drawing.Point pnt, Triangle poly)
        {
            Point onScreen = new Point((pnt.X - (double)width / 2) / (del * (double)width / 2), (pnt.Y - (double)height / 2) / (del * (double)width / 2), 1);
            Point norm = poly.normalCam;
            double A = norm.X,
                B = norm.Y,
                C = norm.Z,
                D = -(A * poly.v0.Vcam.getX() + B * poly.v0.Vcam.getY() + C * poly.v0.Vcam.getZ());
            double t = -D / (A * onScreen.getX() + B * onScreen.getY() + C * onScreen.getZ());
            return onScreen.scale(t);
        }

        public Color colorWithLight(Point t, Triangle poly)
        {
            double R = 20, G = 20, B = 20;

            Point norm = poly.normalCam;
            Point ray = lightPoint.Vcam.minus(t); //от точки до источника освещения
            if (ray.findAngle(norm) < Math.PI / 2)
            {
                double cos = Math.Pow(Math.Cos(ray.findAngle(norm)), 0.5);
                R = cos * poly.color.R;
                G = cos * poly.color.G;
                B = cos * poly.color.B;
            }
            return Color.FromArgb(Math.Min((int)R, 255), Math.Min((int)G, 255), Math.Min((int)B, 255));
        }

        // рисует полигон на экране
        public void DrawPolygon(Triangle poly)
        {
            List<System.Drawing.Point> pnts = new List<System.Drawing.Point>();
            pnts.Add(convertToScreenPoint(poly.v0));
            pnts.Add(convertToScreenPoint(poly.v1));
            pnts.Add(convertToScreenPoint(poly.v2));

            int minX, minY, maxX, maxY;
            minX = maxX = pnts[0].X;
            minY = maxY = pnts[0].Y;
            foreach (System.Drawing.Point pnt in pnts)
            {
                minX = Math.Min(minX, pnt.X);
                maxX = Math.Max(maxX, pnt.X);
                minY = Math.Min(minY, pnt.Y);
                maxY = Math.Max(maxY, pnt.Y);
            }
            minX = Math.Max(minX, 0);
            minY = Math.Max(minY, 0);
            maxX = Math.Min(maxX, width - 1);
            maxY = Math.Min(maxY, height - 1);
            for (int X = minX; X <= maxX; X++)
                for (int Y = minY; Y <= maxY; Y++)
                {
                    System.Drawing.Point curp = new System.Drawing.Point(X, Y);
                    if (CheckPoint(curp, pnts))
                    {
                        Point curV = findVertex(curp, poly);
                        if (curV.getZ() < Zbuffer[X, Y])
                        {
                            Zbuffer[X, Y] = curV.getZ();
                            pic.SetPixel(X, Y, colorWithLight(curV, poly));
                        }
                    }
                }
        }

        public void Render()
        {
            if (cam != null)
            {
                //вычисление проекционных координат
                foreach (Point vr in verts)
                    vr.rotateForCam(cam);
                lightPoint.rotateForCam(cam);
                Graphics gr = Graphics.FromImage(pic);
                gr.Clear(Color.White);
                Zbuffer = new double[width, height];
                for (int i = 0; i < width; i++)
                    for (int j = 0; j < height; j++)
                        Zbuffer[i, j] = 10000;  //устанавливаем линию горизонта

                foreach (Triangle pl in polys)
                    if (pl.v0.minus(cam.pos).findAngle(pl.normal) > Math.PI / 2)  //если плоскость смотрит в сторону камеры
                        if (pl.v0.Vcam.getZ() > 0.2 && pl.v1.Vcam.getZ() > 0.2 && pl.v2.Vcam.getZ() > 0.2) //и если она не лежит за ней
                            DrawPolygon(pl);
            }
        }
    }
}
