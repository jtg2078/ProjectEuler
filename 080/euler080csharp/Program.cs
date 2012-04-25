using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler080csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The total of the digital sums of the first one hundred decimal digits for all the irrational square roots: ");

            TestBenchSetup();
            TestBenchLoader(using_BigInteger_with_square_roots_by_subtraction);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// wow this paper pretty much saved the day(http://www.afjarvis.staff.shef.ac.uk/maths/jarvisspec02.pdf)
        /// And i dont want to deal with Big Integer problem so i use .net 4.0 instead.
        /// </summary>
        /// <returns></returns>
        static int using_BigInteger_with_square_roots_by_subtraction()
        {
            var limit = 100;
            var bound = (int)Math.Sqrt(limit);
            var map = new bool[limit + 1];
            var result = 0;
            BigInteger cut_off = BigInteger.Parse(
                Enumerable.Range(0, 101).Aggregate(new StringBuilder("1"), (s, n) => s.Append("0")).ToString());
            for (int i = 0; i < limit; i++)
            {
                if (i <= bound)
                    map[i * i] = true; // using this way to mark perfect squares

                if (map[i] == false) // Perfect squares are skipped, The square root of x is rational if and only if x 
                {                    // is a rational number that can be represented as a ratio of two perfect squares
                    BigInteger a = 5 * i;
                    BigInteger b = 5;
                    while (b < cut_off) // since not every iteration would produce extra digit,
                    {                   // so when b.length == 100, b might not be accurate enough, wait until length is 101
                        if (a >= b)
                        {
                            a = a - b;
                            b += 10;
                        }
                        else
                        {
                            a *= 100;
                            var final_d = b % 10;
                            b -= final_d;
                            b *= 10;
                            b += final_d;
                        }
                    }
                    result += b.ToString().Select(l => Convert.ToInt32(l) - 48).Take(100).Sum();
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
