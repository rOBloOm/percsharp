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
        private double initialBias;
        private Vector w;
        private double b;
        private double r = 0.1;


        public Perceptron(Vector initWeights, double initBias, double learningRate)
        {
            this.initialWeight = initWeights;
            this.initialBias = initBias;
            this.w = initWeights;
            this.b = initBias;

            this.r = learningRate;
        }

        public Vector InitialWeight => initialWeight;
        public double InitialBias => initialBias;

        public Vector W
        {
            get => w;
            set
            {
                w = value;
            }
        }

        public double Bias
        {
            get => b;
            set
            {
                b = value;
            }
        }
        
        public bool Classify(Vector input)
        {
            return input * w  + b >= 0;
        }
    }
}
