using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler101csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The sum of FITs for the BOPs: ");

            TestBenchSetup();
            TestBenchLoader(using_Lagrange_polynomial_to_find_fits);

            Console.WriteLine("press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// using the Lagrange polynomial(http://en.wikipedia.org/wiki/Lagrange_polynomial) to find fits.(thx google!)
        /// the actual coding implementation is bascially compute and store all the numerator portion in (top), and 
        /// denominator portion in (bot), and at the end, combine with nth term from 10th degree poly function from
        /// the question.
        /// the 10th degree poly question can be simplify to such term by:
        /// 1 - n + n^2 - n^3 + n^4 - n^5 + n^6 - n^7 + n^8 - n^9 + n^10
        /// 1 - n(1 - n + n^2 - n^3 + n^4 - n^5 + n^6 - n^7 + n^8 - n^9)
        /// 1 - n(1 - n(1-n......1-n(1-n))))))))...
        /// </summary>
        static long using_Lagrange_polynomial_to_find_fits()
        {
            // calculate the tenth degree polynomial generating function from the question
            Func<int, long> tensPoly = n =>
                {
                    long seed = 1 - n;
                    for (int i = 0; i < 9; i++)
                    {
                        seed = 1 - n * seed;
                    }
                    return seed;
                };
            var f = Enumerable.Range(1, 10).Select(n => tensPoly(n)).ToArray();
            // Lagrange interpolating polynomial
            Func<int, long> lagrange = n =>
                {
                    long nth = 0;
                    for (int i = 0; i < n; i++)
                    {
                        var top = 1;
                        var bot = 1;
                        for (int j = 0; j < n; j++)
                        {
                            if (i != j)
                            {
                                top *= (n - j);
                                bot *= (i - j);
                            }
                        }
                        nth += (f[i] * top / bot);
                    }
                    return nth;
                };
            //  using Lagrange interpolating polynomial to find sum of FITs
            long result = 0;
            for (int i = 1; i < 11; i++)
            {
                result += lagrange(i);
            }
            return result;
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
