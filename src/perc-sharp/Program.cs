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

            trainer.Train();
            int trainingSessions = 1;
            Console.WriteLine(trainer.Errors);
            while (trainer.Errors >= 0)
            {
                trainer.Train();
                trainingSessions++;
                Console.WriteLine(trainer.Errors);
                Console.Read();
            }
            
            Console.Read();
        }
    }
}
