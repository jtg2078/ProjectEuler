using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace euler099csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The line number that has the greatest numerical value: ");

            TestBenchSetup();
            TestBenchLoader(use_logarithm_and_formula_to_reduce_number_size);

            Console.WriteLine("press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// using the formula logb(x^p) = p * logb(x), which makes the number fits into a double
        /// </summary>
        static int use_logarithm_and_formula_to_reduce_number_size()
        {
            var line_num = 0;
            double max = 0;
            var max_line = 0;
            var base_exponent = new string[2];
            double estimate = 0;
            foreach (var line in File.ReadAllLines(@"..\..\base_exp.txt"))
            {
                line_num++;
                base_exponent = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                estimate = Convert.ToInt32(base_exponent[1]) * Math.Log10(Convert.ToInt32(base_exponent[0]));

                if (estimate > max)
                {
                    max = estimate;
                    max_line = line_num;
                }
            }
            return max_line;
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
