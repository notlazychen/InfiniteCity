using System;
using System.Collections.Generic;
using System.Text;

namespace Roy.Utils.Geometry
{
    public static class Transform
    {
        public static Vector2 Rotate(Vector2 vector, double angle)
        {
            var matrix = new Matrix(new double[,] {
                { Math.Cos(angle), Math.Sin(angle) },
                { -Math.Sin(angle), Math.Cos(angle)}
            });
            var v = new Matrix(new double[,] {
                { vector.X, vector.Y }
            });

            var result = v * matrix;
            return new Vector2() { X= result[0, 0], Y = result[0,1] };
        }

    }
}
