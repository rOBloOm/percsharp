using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloom.Percsharp.Domain
{
    public class PerceptronTrainerStepPrediction
    {
        public bool Error;
        public bool isPositiveDatapoint;

        public Vector DataPoint;

        public Vector CurrentWeight;
        public Vector Correction;
        public Vector ResultingWeight;
    }
}
