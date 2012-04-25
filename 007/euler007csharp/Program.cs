using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace euler007csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();

            Console.WriteLine("The 10001st prime number");
            sw.Start();
            var sum1 = PrimeNumberInTerm(116684, 3612);
            sw.Stop();
            Console.WriteLine(string.Format("Sieve of Eratosthenes brute force way(tick:{0}): {1}", sw.ElapsedTicks, sum1));

            sw.Reset();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        ///  so basically i use Sieve of Eratosthenes to construct list of prime
        ///  and using the Prime number theorem (http://en.wikipedia.org/wiki/Prime_Number_Theorem)
        ///  x/ln(x) = 10001, solving x should give me a good ceiling.
        ///  x came to be about 116684 (thank you, WolframAlpha)
        ///  takes about 5k tick
        ///  the 10001th prime is 104743
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        static int PrimeNumberInTerm(long maxCeiling, int term)
        {
            var upper = maxCeiling;
            var upperl = (int)Math.Sqrt(upper);
            bool[] map = new bool[upper];
            map[0] = true;
            map[1] = true;
            int count = 0;

            for (int l = 2; l < upperl; l++)
            {
                if (map[l] == false)
                {
                    for (long i = l * l; i < upper; i += l)
                    {
                        map[i] = true;
                    }
                }
            }

            for (int i = 0; i < upper; i++)
            {
                if (map[i] == false)
                    count++;

                if (count == term)
                    return i;
            }

            return count;
        }
    }
}
