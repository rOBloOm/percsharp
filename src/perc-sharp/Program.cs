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
            DataGeneratorLinearSeparable generator = new DataGeneratorLinearSeparable(new Vector(new decimal[] { 1, 0 }), 100, 2);
            generator.run();


            Console.WriteLine("Positives");
            generator.Positives.ForEach(p => Console.WriteLine(p));
            Console.WriteLine("Negatives");
            generator.Negatives.ForEach(n => Console.WriteLine(n));

            Console.Read();
        }
    }
}
