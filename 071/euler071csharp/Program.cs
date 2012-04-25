using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler071csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The the numerator of the fraction immediately to the left of 3/7: ");

            TestBenchSetup();
            TestBenchLoader(test_each_fraction_with_denominator_within_range);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// basically just test each fracion a/b such that x/d = 3/7, where
        /// d = 9,10,11...1mil
        /// x can be calculated by x = (3*b)/7.
        /// also, since this problem only ask for reduced form. so dont need to worry about where x/d is really 3/7
        /// </summary>
        static int test_each_fraction_with_denominator_within_range()
        {
            var ratio_n = 3;
            var ratio_d = 7;
            var limit = 1000000;
            long max_n = 2;
            long max_d = 5;
            var n = 0;
            for (int d = limit - 1; d > 8; d--) // ends before 8, since the question already stated 1 to 8
            {                                   // also, starting from highest, the higer the number, the closer to the ratio
                n = (ratio_n * d) / ratio_d;

                if (max_n * d < max_d * n && find_GCD(n, d) == 1) //no going keep trying n, where n-1, n-2, n-3...
                {
                    max_n = n;
                    max_d = d;
                }
            }
            return (int)max_n;
        }

        /// <summary>
        /// Euclid's algorithm for finding gcd
        /// </summary>
        static int find_GCD(int x, int y)
        {
            while (x > 0)
            {
                y = y % x;
                y = y + x; // the next three lines is just swapping x and y
                x = y - x;
                y = y - x;
            }
            return y;
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
