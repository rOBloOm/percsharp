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
        public double CurrentBias;        
        public double ResultingBias;

        public double CurrentXDeviation => CurrentBias == 0 || CurrentWeight[0] == 0 ? 0 : CurrentBias / CurrentWeight[0];
        public double ResultingXDeviation => ResultingBias == 0 || ResultingWeight[0] == 0 ? 0: ResultingBias / ResultingWeight[0];
    }
}
