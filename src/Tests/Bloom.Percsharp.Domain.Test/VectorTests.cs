using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Bloom.Percsharp.Domain.Test
{
    public class VectorTests
    {
        [Fact]
        public void VectorInitialization()
        {
            Vector vector = new Vector(1, 2);

            Assert.Equal(1, vector[0]);
            Assert.Equal(2, vector[1]);
        }

        [Fact]
        public void VectorAddition()
        {
            Vector v1 = new Vector(1, 3);
            Vector v2 = new Vector(3, 5);

            Vector result = v1 + v2;

            Assert.Equal(4, result[0]);
            Assert.Equal(8, result[1]);
        }
    }
}
