using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler077csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The first value as the sum of primes in over 5k different ways: ");

            TestBenchSetup();
            TestBenchLoader(using_dynamic_programming);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// using the same concept from code in problem 31. Instead of coins, we use seive to calculate primes
        /// and within the loop, solve each of the optimum subset problem. lawl~ 
        /// </summary>
        static int using_dynamic_programming()
        {
            var max = 100; // arbitrary picked, it worked :)
            var primes = new bool[max];
            var map = new int[max];
            int s, m, n;
            for (s = 2; s < max; s++) // seive for finding primes
            {
                if (primes[s] == false)
                {
                    for (m = s * s; m < max; m += s)
                    {
                        primes[m] = true;
                    }
                    map[s]++;  // if the number is prime, then just itself is a way, so +1
                    for (n = s; n < max; n++)
                    {
                        map[n] += map[n - s]; // for each new additional prime(s), add the # ways from (n-s)
                    }
                }
            }
            for (s = 2; s < max; s++)
            {
                if (map[s] > 5000)
                    return s;
            }
            return 0;
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
