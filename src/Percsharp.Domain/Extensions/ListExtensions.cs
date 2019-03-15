using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloom.Percsharp.Domain.Extensions
{
    /// <summary>
    /// Yes, this is shamelessly copied from Stackoverflow
    /// https://stackoverflow.com/questions/273313/randomize-a-listt
    /// </summary>
    public static class ListExtensions
    {
        private static readonly Random rng = new Random();
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
