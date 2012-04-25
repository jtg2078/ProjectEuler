using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler115csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The least value of n for which the fill-count function first exceeds one million: ");

            TestBenchSetup();
            TestBenchLoader(using_same_ways_as_problem_114_with_modification);
            
            Console.WriteLine("press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// basically the same code from problem 114 with little modifications. the outer most loop is set without
        /// limit(though the array that stored incremental results is only up to 500). as soon as result(ways) of
        /// current row length reaches 1mil or greater, the loop stopped and row length is returned as answer
        /// </summary>
        static int using_same_ways_as_problem_114_with_modification()
        {
            var target = 1000000;
            var min_block = 50;
            var row_size = 500; // arbitrarily picked, turns out be more than enough :P
            var row = new int[row_size + 1];

            for (int r = min_block; ; r++) // size of the row from min_block to no limit
            {
                for (int b = min_block; b <= r; b++) // various size of red blocks from min_block...current row size
                {
                    var bound = r - b + 1;
                    for (int p = 1; p <= bound; p++) // positions of red block in row, from row begin to end
                    {
                        var remain = r - (b + p); // remaining spaces left after the red block + 1 black block gap

                        if (remain >= min_block)
                            row[r] += row[remain];

                        row[r]++;
                    }
                }
                if (row[r] >= target)
                {
                    return r;
                }
            }
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
