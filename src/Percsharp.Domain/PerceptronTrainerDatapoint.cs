using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloom.Percsharp.Domain
{
    public struct PerceptronTrainerDatapoint
    {
        public Vector Datapoint;
        public bool IsPositive;
    }
}
