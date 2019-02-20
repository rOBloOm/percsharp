using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace percsharp.domain
{
    public class Perceptron
    {
        private Vector initialWeight;
        private decimal initialBias;
        private Vector w;
        private decimal b;
        private decimal r = 0.1M;


        public Perceptron(Vector initWeights, decimal initBias)
        {
            this.initialWeight = initWeights;
            this.initialBias = initialBias;
            this.w = initWeights;
            this.b = initBias;
        }

        public Vector InitialWeight => initialWeight;
        public decimal InitialBias => initialBias;

        public Vector W
        {
            get => w;
            set
            {
                w = value;
            }
        }

        public decimal Bias
        {
            get => b;
            set
            {
                b = value;
            }
        }

        public decimal R
        {
            get => r;
            set
            {
                r = value;
            }
        }

        public bool Classify(Vector input)
        {
            return input * w  + b < 0;
        }
    }
}
