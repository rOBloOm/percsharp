using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloom.Percsharp.Domain
{
    public class DataGeneratorLinearSeparable
    {
        public DataGeneratorLinearSeparable(Vector initVector, double bias, int size, int dimension)
        {
            this.initVector = initVector;
            this.initBias = bias;
            this.size = size;
            this.dimension = dimension;
        }

        #region Properties

        private int size;
        public int Size => size;

        private int dimension;
        public int Dimension => dimension;

        private Vector initVector;
        public Vector InitVector => initVector;

        private double initBias;
        public double InitBias => initBias;

        private List<Vector> postitives;
        public List<Vector> Positives => postitives;

        private List<Vector> negatives;
        public List<Vector> Negatives => negatives;

        public Vector SeparationLineUpperEnd => (InitVector * 10).Rotate(0.5 * Math.PI).Add(new Vector(XDeviation, 0));

        public Vector SeparationLineLowerEnd => (InitVector * 10).Rotate(-0.5 * Math.PI).Add(new Vector(XDeviation, 0));

        public double XDeviation => InitBias / initVector[0]; 

        #endregion Properties

        public void run(int seed)
        {
            negatives = new List<Vector>();
            postitives = new List<Vector>();

            Random rnd = new Random(seed);

            for (int i = 0; i < Size; i++)
            {
                Vector data = new Vector(Dimension);

                for(int d = 0; d < Dimension; d++)
                {
                    data[d] = ((double)rnd.Next(-10, 10) /10);
                }

                if(data * InitVector - InitBias > 0)
                {
                    postitives.Add(data);
                }
                else
                {
                    negatives.Add(data);
                }
            }
        }
    }
}
