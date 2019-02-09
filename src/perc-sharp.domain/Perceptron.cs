using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace percsharp.domain
{
    public class Perceptron
    {
        private Vector Weights;

        public Perceptron(int inputSize)
        {
            Weights = new Vector(inputSize);
        }

        public bool Classify(Vector input)
        {
            return true;
        }

        public void Learn(Vector input, int error)
        {
            if(error < 0)
            {
                Weights += input;
            }
            else if (error >= 0)
            {
                Weights -= input;
            }
        }
    }
}
