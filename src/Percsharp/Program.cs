using Bloom.Percsharp.Domain;
using System;

namespace Bloom.Percsharp
{
    class Program
    {
        static void Main(string[] args)
        {
            DataGeneratorLinearSeparable generator = new DataGeneratorLinearSeparable(new Vector(new double[] { 1, 0 }), 0, 100, 2);
            generator.run();


            Console.WriteLine("Positives");
            generator.Positives.ForEach(p => Console.WriteLine(p));
            Console.WriteLine("Negatives");
            generator.Negatives.ForEach(n => Console.WriteLine(n));

            Console.Read();
        }
    }
}
