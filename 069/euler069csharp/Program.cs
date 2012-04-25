using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler069csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Thevalue of n ≤ 1,000,000 for which n/φ(n) is a maximum: ");

            TestBenchSetup();
            TestBenchLoader(using_sieve_to_find_prime_factors_and_calculate_eulers_function);
            TestBenchLoader(smart_totient_formula_observation);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
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
            var LIMIT = 1000000;
            var bound = LIMIT / 2;
            var primes = new int[LIMIT + 1];
            int s, m, t;
            for (s = 2; s <= bound; s++)
            {
                if (primes[s] == 0)
                {
                    for (m = s * 2; m <= LIMIT; m += s) // m = s * 2 since no point to test prime number(always 1 and itself)
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
            double max = 0;
            var n = 0;
            for (s = 2; s <= LIMIT; s++) // basically loop through the whole array and calculate max
            {
                t = primes[s];
                if (t != 0)
                {
                    cur = s / (double)t; 
                    if (max < cur)
                    {
                        max = cur;
                        n = s;
                    }
                }
            }
            return n;
        }

        /// <summary>
        /// from the problem's official answer.
        /// basically the number would be the the largest number within bound(1mil) that only has prime factors
        /// with power of 1, so baically 2*3*5*7*11*13*17 = 510510 (dang!)
        /// </summary>
        static int smart_totient_formula_observation()
        {
            var LIMIT = 1000000;
            var num = 4;
            var n = 2 * 3;
            var result = 0;
            while (true)
            {
                if (isPrime(num))
                {
                    n *= num;
                    if (n <= LIMIT)
                        result = n;
                    else
                        return result;
                }
                num++;
            }
        }

        static bool isPrime(int n)
        {
            if (n <= 1)
                return false;
            var limit = (int)Math.Sqrt(n) + 1;
            for (int i = 2; i < limit; i++)
            {
                if (n % i == 0)
                    return false;
            }
            return true;
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
