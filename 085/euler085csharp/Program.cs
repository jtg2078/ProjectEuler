using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler085csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The area of the grid with the nearest solution: ");

            TestBenchSetup();
            TestBenchLoader(brute_force_with_limited_range_using_formula);

            Console.WriteLine("press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// x(number of triangles) = 1/4 m n (m+1) (n+1) where m and n are rows and columns
        /// so roughly m^2 n^2 = 8mil that gives m*n around 2828ish so set range for m and n to be (1..to 100)
        /// and it works!
        /// http://www.mathhelpforum.com/math-help/other-topics/132725-number-squares-rectangles-grid.html
        /// </summary>
        static int brute_force_with_limited_range_using_formula()
        {
            var target = 2000000;
            var delta = target;
            var m1 = 0;
            var n1 = 0;
            for (int m = 100; m >= 1; m--)
            {
                for (int n = 1; n < m; n++)
                {
                    var sum = m * n * (m + 1) * (n + 1) / 4;
                    var diff = Math.Abs(sum - target);
                    if (delta > diff)
                    {
                        delta = diff;
                        m1 = m;
                        n1 = n;
                    }
                }
            }
            return m1 * n1;
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
