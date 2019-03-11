using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloom.Percsharp.Domain
{
    public class Vector
    {
        private double[] vector;

        public Vector(int size)
        {
            vector = new double[size];
            for (int i = 0; i < size; i++)
            {
                vector[i] = 0;
            }
        }

        public Vector(double[] data)
        {
            vector = data;
        }

        public double this[int i]
        {
            get { return this.vector[i]; }
            set { this.vector[i] = value; }
        }

        public int Size => this.vector.Length;

        public double Magnitude
        {
            get
            {
                double sum = 0;
                for(int i = 0; i < vector.Length; i++)
                {
                    sum += Math.Pow((double)vector[i], 2);
                }

                return (double)Math.Sqrt(sum);
            }
        }

        public Vector UnitVector()
        {
            return Magnitude != 0 ? this * (1 / Magnitude) : new Vector(new double[] { 0, 0 });
        }

        #region Vector Initialization

        public static implicit operator Vector(double[] value)
        {
            if (value == null) return null;

            Vector result = new Vector(value.Length);
            for(int i = 0; i < value.Length; i++)
            {
                result[i] = value[i];
            }

            return result;
        }

        #endregion Vector Initialization

        #region Vector Addition

        public Vector Add(Vector other)
        {
            Vector result = new Vector(this.Size);

            if (this.Size != other.Size)
                throw new Exception("Vector addition not supported when vectors are not the same size");


            for(int i = 0; i < this.Size; i++)
            {
                result[i] = this[i] + other[i];
            }

            return result;
        }

        public static Vector operator +(Vector v1, Vector v2)
        {
            return v1.Add(v2);
        }

        #endregion Vector Addition

        #region Vector Subtraction

        public static Vector operator -(Vector v1, Vector v2)
        {
            return v1.Add(v2 * -1);
        }

        #endregion Vector Subtraction

        #region Vector Multiplication

        public double DotProduct(Vector other)
        {
            if (this.Size != other.Size)
                throw new Exception("Scalar product not supported when vectors are not the same size");

            double sum = 0;
            for (int i = 0; i < Size; i++)
            {
                sum += vector[i] * other[i];
            }

            return sum;
        }

        public static double operator *(Vector v1, Vector v2)
        {
            return v1.DotProduct(v2);
        }

        public static Vector operator *(Vector v1, double scalar)
        {
            Vector result = new Vector(v1.Size);
            for(int i = 0; i<v1.Size; i++)
            {
                result[i] = v1[i] * scalar;
            }

            return result;
        }

        public static Vector operator *(double scalar, Vector v1)
        {
            return v1 * scalar;
        }

        #endregion Vector Multiplication

        #region Vector Rotation

        public Vector Rotate(double rad)
        {
            double[] result = new double[2];
            result[0] = vector[0] * Math.Cos(rad) - vector[1] * Math.Sin(rad);
            result[1] = vector[0] * Math.Sin(rad) + vector[1] * Math.Cos(rad);
            return result;
        }

        #endregion Vector Rotation

        public override string ToString()
        {
            if (vector == null)
                return string.Empty;

            return string.Join(",", vector);
        }
    }
}
