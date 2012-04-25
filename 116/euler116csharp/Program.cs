using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler116csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The # of ways can the black tiles in a row measuring fifty units in length be replaced: ");

            TestBenchSetup();
            TestBenchLoader(using_same_algorithm_as_problem_114_with_modification);

            Console.WriteLine("press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// again, reuse most of the code from problem 114. the dynamic programming is used to gradually build the
        /// solution. In this case, a func calculate_ways is created to find out # of ways a given block size
        /// can be placed with given row size. then the func is called three times for block size of 3, 4 and 5, and
        /// the results are accumulated.
        /// </summary>
        static long using_same_algorithm_as_problem_114_with_modification()
        {
            Func<int, int, long> calculate_ways = (block_size, row_size) =>
                {
                    var ways = new long[row_size + 1];
                    for (int row = block_size; row <= row_size; row++)
                    {
                        var bound = row - block_size + 1;
                        for (int pos = 1; pos <= bound; pos++)
                        {
                            var remain = row - (block_size + pos) + 1;
                            
                            if (remain >= block_size)
                                ways[row] += ways[remain];

                            ways[row]++;
                        }
                    }
                    return ways[row_size];
                };

            long total = 0;
            total += calculate_ways(2, 50); // red
            total += calculate_ways(3, 50); // green
            total += calculate_ways(4, 50); // blue
            return total;
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
