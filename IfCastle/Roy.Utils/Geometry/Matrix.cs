using System;
using System.Collections.Generic;
using System.Text;

namespace Roy.Utils.Geometry
{
    public sealed class Matrix
    {
        private double[,] values;
        public int Row { get; }
        public int Column { get; }

        public Matrix(int row, int column)
        {
            Row = row;
            Column = column;
            values = new double[row, column];
        }

        public Matrix(double[,] members)
        {
            Row = members.GetUpperBound(0) + 1;
            Column = members.GetUpperBound(1) + 1;
            values = new double[Row, Column];
            Array.Copy(members, values, Row * Column);
        }

        public double this[int row, int column]
        {
            get { return values[row, column]; }
            set { values[row, column] = value; }
        }

        /// <summary>
        /// 矩阵相乘，就是A矩阵的行分别乘以B矩阵的列，其值
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Matrix operator *(Matrix a, Matrix b)
        {
            if (a.Column != b.Row)
                throw new Exception("矩阵维数不匹配。");
            Matrix result = new Matrix(a.Row, b.Column);
            for (int i = 0; i < a.Row; i++)
                for (int j = 0; j < b.Column; j++)
                    for (int k = 0; k < a.Column; k++)
                        result[i, j] += a[i, k] * b[k, j];
            return result;
        }
    }
}
