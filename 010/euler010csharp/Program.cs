using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace euler010csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();

            Console.WriteLine("The sum of all the primes below two million");
            sw.Start();
            var sum1 = PrimeNumberInRange(2000000);
            sw.Stop();
            Console.WriteLine(string.Format("Sieve of Eratosthenes brute force way(tick:{0}): {1}", sw.ElapsedTicks, sum1));

            sw.Reset();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// so basically i use Sieve of Eratosthenes to construct list of prime
        /// heh project euler sure likes prime
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        static long PrimeNumberInRange(int n)
        {
            var upper = n;
            var upperl = (int)Math.Sqrt(upper);
            long sum = 0;
            
            bool[] map = new bool[upper];
            map[0] = true;
            map[1] = true;

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

            for (int i = upper - 1; i > 0; i--)
            {
                if (map[i] == false)
                    sum += i;
            }

            return sum;
        }
    }
}
