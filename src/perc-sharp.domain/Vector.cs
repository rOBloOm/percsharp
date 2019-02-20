using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace percsharp.domain
{
    public class Vector
    {
        private decimal[] vector;

        public Vector(int size)
        {
            vector = new decimal[size];
            for (int i = 0; i < size; i++)
            {
                vector[i] = 0;
            }
        }

        public Vector(decimal[] data)
        {
            vector = data;
        }

        public decimal this[int i]
        {
            get { return this.vector[i]; }
            set { this.vector[i] = value; }
        }

        public int Size => this.vector.Length;

        public decimal Magnitude
        {
            get
            {
                double sum = 0;
                for(int i = 0; i < vector.Length; i++)
                {
                    sum += Math.Pow((double)vector[i], 2);
                }

                return (decimal)Math.Sqrt(sum);
            }
        }

        public Vector UnitVector()
        {
            return this * (1 / Magnitude);
        }

        #region Vector Initialization

        public static implicit operator Vector(decimal[] value)
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

        public decimal DotProduct(Vector other)
        {
            if (this.Size != other.Size)
                throw new Exception("Scalar product not supported when vectors are not the same size");

            decimal sum = 0;
            for (int i = 0; i < Size; i++)
            {
                sum += vector[i] * other[i];
            }

            return sum;
        }

        public static decimal operator *(Vector v1, Vector v2)
        {
            return v1.DotProduct(v2);
        }

        public static Vector operator *(Vector v1, decimal scalar)
        {
            Vector result = new Vector(v1.Size);
            for(int i = 0; i<v1.Size; i++)
            {
                result[i] = v1[i] * scalar;
            }

            return result;
        }

        public static Vector operator *(decimal scalar, Vector v1)
        {
            return v1 * scalar;
        }

        #endregion Vector Multiplication

        public override string ToString()
        {
            if (vector == null)
                return string.Empty;

            return string.Join(",", vector);
        }
    }
}
