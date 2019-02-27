using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloom.Percsharp.Domain
{
    public class DataGeneratorLinearSeparable
    {
        private List<Vector> negatives;
        private List<Vector> postitives;

        private int size;
        private int dimension;

        private Vector initVector;
        private decimal deviation;

        public DataGeneratorLinearSeparable(Vector initVector, decimal deviation, int size, int dimension)
        {
            this.initVector = initVector;
            this.deviation = deviation;
            this.size = size;
            this.dimension = dimension;
        }

        public int Size => size;
        public int Dimension => dimension;

        public Vector InitVector => initVector;
        public decimal InitBias => deviation;

        public List<Vector> Positives => postitives;
        public List<Vector> Negatives => negatives;

        public void run()
        {
            negatives = new List<Vector>();
            postitives = new List<Vector>();

            Random rnd = new Random();

            for (int i = 0; i < size; i++)
            {
                Vector data = new Vector(dimension);

                for(int d = 0; d < dimension; d++)
                {
                    data[d] = ((decimal)rnd.Next(-10, 10) / 10);
                }

                if(data * initVector + deviation > 0)
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
