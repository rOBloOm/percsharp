using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloom.Percsharp.Domain
{
    public class Perceptron
    {
        private Vector initialWeight;
        private decimal initialBias;
        private Vector w;
        private decimal b;
        private decimal r = 0.1M;


        public Perceptron(Vector initWeights, decimal initBias, decimal learningRate)
        {
            this.initialWeight = initWeights;
            this.initialBias = initBias;
            this.w = initWeights;
            this.b = initBias;

            this.r = learningRate;
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
        
        public bool Classify(Vector input)
        {
            return input * w  + b < 0;
        }
    }
}
