using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler114csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The number of ways can a row measuring fifty units in length be filled: ");

            TestBenchSetup();
            TestBenchLoader(using_dynamic_programming_to_count);

            Console.WriteLine("press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// basically build the solution step by step from smallest row size to target row size
        /// using dynamic programming memoization technique to record number of ways of putting 
        /// red block(s) in a given row size so it could be used later in following steps:
        /// 
        /// 1. starting for smallest possible row size
        ///    2. iterate all red blocks that could be fit in the given row size
        ///       3. count the ways of positioning of red blocks and save the result in 
        ///          row[current row size]
        ///          
        /// eg. for a row with size 3, theres only 1 way of putting 1 type of red block(size 3)
        ///     row_size (4) =>  2 types of red block
        ///                        (size 3) => 2 ways of putting it(4-3+1=2)
        ///                        (size 4) => 1 way  of putting it(4-4+1=1)
        ///                        
        ///     row_size (8) =>  6 types of red block
        ///                        (size 3) => 6 ways (8-3+1=6)
        ///                           but when it is place at beginning of the row, theres 4 available
        ///                           spaces left after it(1 space for mandetory gap in between red blocks). 
        ///                           so look up ways for row_size (4), which is 3, so on..
        ///                        (size 4) => 5 ways (8-4+1=5)
        ///                           when it is placed at beginning, 3 available spaces left, so look up
        ///                           row_size(3)...
        ///                           when it is placed at block 1, 2 available spaces left, and no red blocks
        ///                           could fit..
        /// </summary>
        static long using_dynamic_programming_to_count()
        {
            var min_block = 3;
            var row_size = 50;
            var row = new long[row_size + 1];
            
            for (int r = min_block; r <= row_size; r++) // size of the row from 3...50
            {
                for (int b = min_block; b <= r; b++) // various size of red blocks from 3...current row size
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
            }
            return row[row_size] + 1; // the 1 is to count "no red blocks" scenario
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
