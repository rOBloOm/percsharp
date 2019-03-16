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
            this.InitVector = initVector;
            this.InitBias = bias;
            this.Size = size;
            this.Dimension = dimension;
            this.Spread = 1;
        }

        #region Properties

        public int Size { get; private set; }

        public int Dimension { get; private set; }

        public Vector InitVector { get; private set; }

        public double InitBias { get; private set; }

        public List<Vector> Positives { get; private set; }
        public List<Vector> Negatives { get; private set; }
        public double Spread { get; set; }

        public Vector SeparationLineUpperEnd => (InitVector * 10).Rotate(0.5 * Math.PI).Add(new Vector(XDeviation, 0));

        public Vector SeparationLineLowerEnd => (InitVector * 10).Rotate(-0.5 * Math.PI).Add(new Vector(XDeviation, 0));

        public double XDeviation => InitBias / InitVector[0]; 

        #endregion Properties

        public void run(int seed)
        {
            Negatives = new List<Vector>();
            Positives = new List<Vector>();

            Random rnd = new Random(seed);

            for (int i = 0; i < Size; i++)
            {
                Vector data = new Vector(Dimension);

                for(int d = 0; d < Dimension; d++)
                {
                    data[d] = ((double)rnd.Next(-100, 100) /100) * Spread;
                    
                    if (d == 0)
                        data[d] += XDeviation;
                }

                //Introduce a very small additional deviation from zero to account for rounding error when classifing points on the separation line
                if (data * InitVector - InitBias > 0.001)
                {                    
                    Positives.Add(data);
                }
                else
                {
                    Console.WriteLine(data);
                    Negatives.Add(data);
                }
            }
        }
    }
}
