using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace euler067csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The maximum total from top to bottom in triangle.txt: ");

            TestBenchSetup();
            TestBenchLoader(using_linq_and_algorithm_from_problem_18);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// basically using linq to read all lines of number in reverse order, parse each line into int[], and then
        /// use the same algorithm from problem 18(o(n), DP-style).
        /// the current vector store the row with updated number(max sum from children)
        /// the last holds the current's content because current will be wiped to hold new updated number.
        /// for the alogorithm: see following:
        /// 
        /// static int[][] map = new int[][]
        /// {
        ///     new int[] {3},          new int[] {3},           new int[] {3},         *new int[] {23},
        ///     new int[] {7,4},    ->  new int[] {7,4},     -> *new int[] {20,19},   -> new int[] {20,19},   
        ///     new int[] {2,4,6},     *new int[] {10,13,15},    new int[] {10,13,15},   new int[] {10,13,15},
        ///    *new int[] {8,5,9,3},    new int[] {8,5,9,3},     new int[] {8,5,9,3},    new int[] {8,5,9,3},
        /// };
        /// 
        /// Replace each cell in each row with higher sum of cell + right or left children, so the follow:
        /// 
        ///  2             (2+8)             10
        /// 8  5 becomes  8     5 becomes   8   5
        /// 
        /// eventually the 1st cell of the whole thing would contain the highest sum (see above)
        /// Since the the file is read in reverse, so "foreach" code can start from 1st row and work the way down, using
        /// current and last respectively. eventually current would contain 1st cell which is the max possible sum.
        /// </summary>
        static int using_linq_and_algorithm_from_problem_18()
        {
            var index = 0;
            var last = new List<int>();
            var current = new List<int>();

            File.ReadAllLines("triangle.txt")
                .Reverse()
                .Select(row => row.Split(' ').Select(ch => Convert.ToInt32(ch)).ToArray())
                .ToList()
                .ForEach(row =>
                    {
                        if (index > 0)
                        {
                            var left = 0;
                            var right = 0;
                            current.Clear();
                            for (int col = 0; col < row.Length; col++)
                            {
                                left = last[col];
                                right = last[col + 1];
                                current.Add(row[col] + (left > right ? left : right));
                            }
                            last.Clear();
                            for (int col = 0; col < current.Count; col++)
                                last.Add(current[col]);
                        }
                        else
                        {
                            last.Clear();
                            for (int col = 0; col < row.Length; col++)
                                last.Add(row[col]);
                        }
                        index++;
                    });

            return current.First();
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
