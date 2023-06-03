using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace KG2
{
    class Triangle
    {

        Point fv0, fv1, fv2;
        public Color color;

        public Triangle(Point av0, Point av1, Point av2, Color aColor)
        {
            fv0 = av0;
            fv1 = av1;
            fv2 = av2;
            color = aColor;
        }

        public Point v0
        {
            get { return fv0; }
        }

        public Point v1
        {
            get { return fv1; }
        }

        public Point v2
        {
            get { return fv2; }
        }

        /// возвращает вектор нормали для плоскости
        public Point normal
        {
            get { return v1.minus(v0).getVectorMult(v2.minus(v0)); }
        }

        public Point normalCam
        {
            get { return v1.Vcam.minus(v0.Vcam).getVectorMult(v2.Vcam.minus(v0.Vcam)); }
        }
    }
}
