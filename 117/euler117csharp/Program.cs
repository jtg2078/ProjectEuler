using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler117csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The # of ways can a row measuring fifty units in length be tiled: ");

            TestBenchSetup();
            TestBenchLoader(using_same_algorithm_as_problem_116_with_modification);

            Console.WriteLine("press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// mostly same as problem 116, the difference is that now block of different size can be mixed, so the problem is 
        /// very similar to problem 114
        /// </summary>
        static long using_same_algorithm_as_problem_116_with_modification()
        {
            var min_block = 2;
            var max_block = 4;
            var row_size = 50;
            var row = new long[row_size + 1];

            for (int r = min_block; r <= row_size; r++) // size of the row from min_block...row_size
            {
                for (int b = min_block; b <= max_block; b++) // various color of block (red/blue/green)
                {
                    var bound = r - b + 1;
                    for (int p = 1; p <= bound; p++) // positions of red block in row, from row begin to end
                    {
                        var remain = r - (b + p) + 1; // remaining spaces left after the colored block

                        if (remain >= min_block)
                            row[r] += row[remain];

                        row[r]++;
                    }
                }
            }
            return row[row_size] + 1; // the 1 is to count "no red/blue/green blocks" scenario
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
