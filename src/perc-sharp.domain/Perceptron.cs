using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace percsharp.domain
{
    public class Perceptron
    {
        private Vector w;

        public Perceptron(Vector initWeights)
        {
            w = initWeights;
        }

        public Vector W
        {
            get => w;
            set
            {
                w = value;
            }
        }

        public bool Classify(Vector input)
        {
            return input * w < 0;
        }

        public void Learn(Vector input, int error)
        {
            if(error < 0)
            {
                w += input;
            }
            else if (error >= 0)
            {
                w -= input;
            }
        }
    }
}
