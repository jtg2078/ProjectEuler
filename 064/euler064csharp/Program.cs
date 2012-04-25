using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler064csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The numbers of continued fractions for N ≤ 10000 have an odd period: ");

            TestBenchSetup();
            TestBenchLoader(using_iterative_algorithm_to_calculate_continued_fraction_expansion);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// using the formula from
        /// http://en.wikipedia.org/wiki/Methods_of_computing_square_roots#Continued_fraction_expansion
        /// which made the whole problem pretty simple. bool[] array is used to keep track of perfect square,
        /// since the range is not big(10k), so its fine~
        /// </summary>
        static int using_iterative_algorithm_to_calculate_continued_fraction_expansion()
        {
            var limit = 10000;
            var bound = (int)Math.Sqrt(limit);
            var map = new bool[limit + 1];
            var result = 0;

            for (int i = 2; i <= limit; i++)
            {
                if (i <= bound)
                    map[i * i] = true; // using this way to mark perfect squares

                if (map[i] == false) // perfect squares are skipped
                {
                    var a0 = (int)Math.Sqrt(i);
                    var m = 0;
                    var d = 1;
                    var a = a0;
                    var counter = -1; // since the 1st iteration is not part of number chains
                    var m1 = int.MinValue; // reset m1 and d1
                    var d1 = int.MinValue;
                    while (true)
                    {
                        counter++;
                        
                        if (counter == 1)
                        {
                            m1 = m; // so these two are marked as head of the period
                            d1 = d;
                        }

                        m = d * a - m;
                        d = (i - m * m) / d;
                        a = (a0 + m) / d;

                        if (m == m1 && d == d1) // so end of period is found
                            break;
                    }
                    if ((counter & 1) != 0)
                        result++;
                }
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