using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KG2
{
    class Camera
    {
        Point fpos; //позиция камеры
        Point fdirection;  //направление камеры
        double fangle; //угол обзора камеры в радианах
        public Matrix toRotate;

        public Camera(Point _pos, Point _direction, double _angle)
        {
            fpos = _pos;
            fdirection = _direction;
            if (direction.length == 0)
            {
                direction.applyMatrix(Matrix.getShiftMatr(0, 0, 1)); //если вектор направления нулевой, то мы его в сторону оси z направляем
            }
            fangle = _angle * Math.PI / 180 ; //переводим угол обзора из градусов в радианы
            calcMatrixToRotate();
        }

        public double angle
        {
            get { return fangle; }
        }

        public Point pos
        {
            get { return fpos; }
        }

        public Point direction
        {
            get { return fdirection; }
        }

        // Процедура высчитывает матрицу преобразования координатных осей так, чтобы ось Z смотрела в направлении камеры
        void calcMatrixToRotate()
        {
            Matrix shift = Matrix.getShiftMatr(-pos.getX(), -pos.getY(), -pos.getZ()); // для переноса начала координат в точку с камерой
            Point proj = new Point(direction.getX(), direction.getY(), 0); // проекция направления на плоскость ХОY
            double ang = proj.findAngle(new Point(0, 1, 0)); //угол между OY и этой проекцией
            if (proj.getX() < 0)
                ang = -ang;
            Matrix rotZ = Matrix.getRorateZMatr(ang); // поворот вокруг OZ, чтобы OY совпала сэтой проекцией
            Matrix rotX = Matrix.getRorateXMatr(direction.findAngle(new Point(0, 0, 1))); //а это матрица поворота, чтобы Z совпало с направлением камеры
            toRotate = shift.mulMatrs(rotZ.mulMatrs(rotX));
        }
    }
}
