using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler100csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The number of blue discs that the box would contain: ");

            TestBenchSetup();
            TestBenchLoader(using_recurring_quadratic_Diophantine_equation);

            Console.WriteLine("press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// (where x = blue)
        /// (where y = total disks)
        /// x/y * (x-1)/(y-1) = 1/2
        /// becomes
        /// 2*x^2 - y^2 - 2*x + y = 0
        /// use
        /// http://www.alpertron.com.ar/QUAD.HTM
        /// to get the following:
        /// Xn+1 = P Xn + Q Yn + K 
        /// Yn+1 = R Xn + S Yn + L 
        /// P = 3
        /// Q = 2
        /// K = -2
        /// R = 4
        /// S = 3
        /// L = -3
        /// </summary>
        static long using_recurring_quadratic_Diophantine_equation()
        {
            long x = 85; // from question's description
            long y = 120;
            long x_tmp, y_tmp;
            long limit = 1000000000000L;
            while (y < limit)
            {
                x_tmp = 3 * x + 2 * y - 2;
                y_tmp = 4 * x + 3 * y - 3;
                x = x_tmp;
                y = y_tmp;
            }
            return x;
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
        static void TestBenchLoader(Func<long> test_method)
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
