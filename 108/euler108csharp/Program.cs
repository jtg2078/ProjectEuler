using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler108csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The least value of n for which the number of distinct solutions exceeds one-thousand: ");

            TestBenchSetup();
            TestBenchLoader(using_prime_factorization_with_google_search);

            Console.WriteLine("press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// according to http://research.att.com/~njas/sequences/A048691
        /// 1/x + 1/y = 1/n is same as tau(n^2), and tau = # of divisors of n
        /// since the question asks for smallest possible number, by using the prime factorization
        /// of calculating  number of divisors(http://en.wikipedia.org/wiki/Divisor)
        /// (2*3*5*7*11*13)^2 = 3*3*3*3*3*3 = 729 and (2*3*5*7*11*13*17)^2 = 3*3*3*3*3*3*3 = 2187
        /// so n must be somewhere in between
        /// </summary>
        static int using_prime_factorization_with_google_search()
        {
            var primes = new int[] { 2, 3, 5, 7, 11, 13, 17 };
            var start = 30030l * 30030l; // 2*3*5*7*11*13
            var end = 510510l * 510510l; // 2*3*5*7*11*13*17
            for (long n = start; n <= end; n += start)
            {
                var count = prime_factorization(n, primes);
                if (count > 2000) // since the order doesnt matter
                    return (int)Math.Sqrt(n);
                    
            }
            return 0;
        }

        static int prime_factorization(long n, int[] primes)
        {
            var count = 1;
            foreach (long p in primes)
            {
                var local = 0;
                while (n % p == 0)
                {
                    local++;
                    n /= p;
                }
                count *= (local + 1);
            }
            return n == 1 ? count : -1;
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
            long result = 0;
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
