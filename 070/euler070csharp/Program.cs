using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler070csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The value of n, 1 < n  < 10^(7), for which φ(n) is a permutation of n and the ratio n/φ(n) produces a minimum: ");

            TestBenchSetup();
            TestBenchLoader(using_sieve_to_find_prime_factors_and_calculate_eulers_function);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// (pretty much the same code from problem 69)
        /// since Sieve of Eratosthenes basically test each number by checking if it is multiple of the current
        /// "known" primes, essentially prime factorization(if each tested primes are saved). By combining this with the formula
        /// on computing euler's function(http://en.wikipedia.org/wiki/Totient_function). the primes array itself save each
        /// successive calculated totient result as more primes are being tested.
        /// e.g:
        /// f(36) = (2^2 * 3^2) = 36(1 - 1/2)(1 - 1/3) = ((36/2)*(36*(2-1))((36/3)*(36*(3-1)) =12;
        /// so when 2 becomes current prime factor to test:
        /// when it hits n, which is 36.
        /// 1. if primes[36] == 0, change it to 36
        /// 2. do ((primes[36]/2)*(primes[36]*(2-1)) now primes[36]=18
        /// 3. later on when 3 becomes current prime factor to test
        /// 4. do ((primes[36]/3)*(primes[36]*(3-1)) now primes[36]=12 (for n=36, this is it)
        /// </summary>
        static int using_sieve_to_find_prime_factors_and_calculate_eulers_function()
        {
            var LIMIT = 10000000;
            var bound = LIMIT / 2;
            var primes = new int[LIMIT];
            int s, m, t;
            for (s = 2; s < bound; s++)
            {
                if (primes[s] == 0)
                {
                    for (m = s * 2; m < LIMIT; m += s) // m = s * 2 since no point to test prime number(always 1 and itself)
                    {
                        t = primes[m];

                        if (t == 0)
                            t = m; // so this is 1st time, mark t to m(the number itself)

                        t /= s; // division first to avoid overflow
                        t *= (s - 1);
                        primes[m] = t;
                    }
                }
            }
            double cur = 0;
            double min = int.MaxValue;
            var n = 0;
            for (s = LIMIT - 1; s >= 2; s--) // since the problem asks for minimum, the number is likely to be close to LIMIT
            {
                t = primes[s];
                if (t != 0)
                {
                    cur = s / (double)t;
                    if (min > cur && isPerm(s, t))
                    {
                        min = cur;
                        n = s;
                    }
                }
            }
            return n;
        }

        /// <summary>
        /// basically just checking if two numbers are permutation of each other
        /// </summary>
        static bool isPerm(int original, int candidate)
        {
            var map = new int[10];
            while (original > 0)
            {
                map[original % 10]++;
                original /= 10;
                map[candidate % 10]--;
                candidate /= 10;
            }
            return map.All(n => n == 0);
        }

        static Stopwatch stopwatch = new Stopwatch();
        static void TestBenchSetup()
        {
            // Uses the second Core or Processor for the Test
            Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(2);
            // Prevents "Normal" processes from interrupting Threads
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            // Prevents "Normal" Threads from interrupting this thread
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
        }
        // see http://www.codeproject.com/KB/testing/stopwatch-measure-precise.aspx
        static void TestBenchLoader(Func<int> test_method)
        {
            stopwatch.Reset();
            stopwatch.Start();
            var result = 0;
            long avg_tick = 0;
            long avg_ms = 0;
            while (stopwatch.ElapsedMilliseconds < 1200)  // A Warmup of 1000-1500 ms 
            // stabilizes the CPU cache and pipeline.
            {
                result = test_method(); // Warmup
            }
            stopwatch.Stop();
            for (int repeat = 0; repeat < 20; ++repeat)
            {
                stopwatch.Reset();
                stopwatch.Start();
                result = test_method();
                stopwatch.Stop();
                avg_tick += stopwatch.ElapsedTicks;
                avg_ms += stopwatch.ElapsedMilliseconds;
            }
            avg_tick = avg_tick / 20;
            avg_ms = avg_ms / 20;
            Console.WriteLine(string.Format("{0} way(ticks:{1}, ms:{2}) Ans:{3}",
                test_method.Method.Name.Replace('_', ' '), avg_tick, avg_ms, result));
        }
    }
}
