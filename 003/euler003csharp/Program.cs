using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace euler003csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();

            Console.WriteLine("The largest prime factor of the number 600851475143 ");
            sw.Start();
            var sum1 = PrimeNumberInRange1(600851475143);
            sw.Stop();
            Console.WriteLine(string.Format("Sieve of Eratosthenes brute force way(tick:{0}): {1}", sw.ElapsedTicks, sum1));

            sw.Reset();

            Console.WriteLine("The largest prime factor of the number 600851475143 ");
            sw.Start();
            PrimeNumberInRange2(600851475143);
            sw.Stop();
            Console.WriteLine(string.Format("Sieve of Eratosthenes brute force way(using LINQ)(tick:{0}): {1}", sw.ElapsedTicks, sum1));

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        ///  so basically i use Sieve of Eratosthenes to construct list of prime
        ///  and the last prime is the biggest prime factor
        ///  takes about 16mil sec, maybe i should use profiler to find out the bottleneck
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        static int PrimeNumberInRange1(long n)
        {
            var upper = (int)Math.Sqrt(n);
            var upperl = (int)Math.Sqrt(upper);
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
                if (map[i] == false && n % i == 0)
                    return i;
            }

            return 0;
        }

        static void PrimeNumberInRange2(long max)
        {
            var upper = (int)Math.Sqrt(max);
            var upper2 = (int)Math.Sqrt(upper);
            var primes = Enumerable.Range(1, upper)
             .Where(i => i > 1)
             .Aggregate(Enumerable.Range(2, upper - 1).ToArray(), (sieve, i) =>
             {
                 if ((i > upper) || (sieve[i - 2] == 0)) return sieve;
                 for (int m = 2; m <= upper2 / i; m++)
                     sieve[i * m - 2] = 0;
                 return sieve;
             })
             .Where(n => n != 0);
        }
    }
}
