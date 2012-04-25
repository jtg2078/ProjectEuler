using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler091csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The numbers of right triangles can be formed: ");

            TestBenchSetup();
            TestBenchLoader(plain_brute_force_find_and_test_all_triangles);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// basically iterate through all the points and use Pythagorean theorem to test if formed triangle is a
        /// right triangle
        /// </summary>
        static int plain_brute_force_find_and_test_all_triangles()
        {
            int x1, y1, x2, y2;
            var x0 = 0;
            var y0 = 0;
            var limit = 50;
            var counter = 0;
            for (x1 = x0; x1 <= limit; x1++)
            {
                for (y1 = y0; y1 <= limit; y1++)
                {
                    for (x2 = x0; x2 <= limit; x2++)
                    {
                        for (y2 = y0; y2 <= limit; y2++)
                        {
                            var p0p1 = x1 * x1 + y1 * y1;
                            var p0p2 = x2 * x2 + y2 * y2;
                            var x_diff = x2 - x1;
                            var y_diff = y2 - y1;
                            var p1p2 = x_diff * x_diff + y_diff * y_diff;
                            var c = p1p2;
                            var b = p0p1;
                            var a = p0p2;
                            if (c < p0p1)
                            {
                                c = p0p1;
                                b = p1p2;
                                a = p0p2;
                            }
                            if (c < p0p2)
                            {
                                c = p0p2;
                                b = p1p2;
                                a = p0p1;
                            }
                            if (b != 0 && a != 0 && a + b == c)
                                counter++;
                        }
                    }
                }
            }
            return counter / 2; // remove duplicates
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
