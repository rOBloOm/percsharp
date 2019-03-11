using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Bloom.Percsharp.Domain.Test
{
    public class VectorTest
    {
        [Fact]
        public void VectorInitialization()
        {
            Vector vector = new Vector(1, 2);

            Assert.Equal(1, vector[0]);
            Assert.Equal(2, vector[1]);
        }

        [Fact]
        public void Addition()
        {
            Vector v1 = new Vector(1, 3);
            Vector v2 = new Vector(3, 5);

            Vector result = v1 + v2;

            Assert.Equal(4, result[0]);
            Assert.Equal(8, result[1]);
        }

        [Fact]
        public void Subtraction()
        {
            Vector v1 = new Vector(6, 9);
            Vector v2 = new Vector(3, 5);

            Vector result = v1 - v2;

            Assert.Equal(3, result[0]);
            Assert.Equal(4, result[1]);
        }

        [Fact]
        public void ScalarMultiplication()
        {
            Vector v1 = new Vector(5, 6);

            Vector result = v1 * 3;

            Assert.Equal(15, result[0]);
            Assert.Equal(18, result[1]);
        }

        [Fact]
        public void RotationPlus()
        {
            Vector v1 = new Vector(1, 0);

            Vector result = v1.Rotate(0.5 * Math.PI);

            Assert.Equal(0, result[0]);
            Assert.Equal(1, result[1]);
        }

        [Fact]
        public void RotationMinus()
        {
            Vector v1 = new Vector(1, 0);

            Vector result = v1.Rotate(-0.5 * Math.PI);

            Assert.Equal(0, result[0]);
            Assert.Equal(-1, result[1]);
        }

        [Fact]
        public void UnitVector()
        {
            Vector v1 = new Vector(10, 0);

            Vector result = v1.UnitVector();

            Assert.Equal(1, result[0]);
            Assert.Equal(0, result[1]);
        }
    }
}
