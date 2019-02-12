using percsharp.domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace percsharp
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string[]> sonarDataStrings = res.Data.ReadSonarAll();            
            SonarDataTrainer trainer = new SonarDataTrainer(sonarDataStrings);

            int steps = 1;
            for(int i = 0; i < steps; i++)
            {
                trainer.Train();
                Console.WriteLine(trainer.Errors);
            }

            Console.Read();
        }
    }
}
