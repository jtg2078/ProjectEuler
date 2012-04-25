using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace euler082csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The minimal path sum, in matrix.txt: ");

            TestBenchSetup();
            TestBenchLoader(using_dynamic_programming);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// the idea is that when moving from one cell to another cell to the right(col+1), if that another cell
        /// is the start of the minimum path from col+1 to end of the matrix wall(it can be any cell alone the wall),
        /// then the path from the cell to end of the wall is the minimum path.
        /// if this is done interatively, then eventually the the minimum path from start of the matrix wall to end of
        /// matrix wall can be calculated. really similar to problem 18 and 81.
        /// </summary>
        static int using_dynamic_programming()
        {
            var len = 80;
            var cache = new int[len, len];
            var map = new int[len, len];
            var row = 0;
            var col = 0;
            foreach (var line in File.ReadAllLines("matrix.txt"))
            {
                col = 0;
                foreach (var num in line.Split(new char[] { ',' }))
                {
                    map[row, col++] = Convert.ToInt32(num);
                }
                row++;
            }
            var tmp = new int[len];
            var min = 0;
            for (col = len - 2; col >= 0; col--)
            {
                for (row = 0; row < len; row++)
                {
                    min = map[row, col + 1];  // since the travel path can be vertical, the following two for loops calculates
                    var accum = 0;            // vertical travel costs
                    for (int i = row - 1; i >= 0; i--)
                    {
                        accum += map[i, col];
                        min = Math.Min(min, accum + map[i, col + 1]);
                    }
                    accum = 0; // accumulate the vertical travel cost
                    for (int i = row + 1; i < len; i++)
                    {
                        accum += map[i, col];
                        min = Math.Min(min, accum + map[i, col + 1]);
                    }
                    tmp[row] = min + map[row, col]; // cant just update the cell with min value yet, since the original still needed
                }                                   // by others
                for (int i = 0; i < len; i++)
                {
                    map[i, col] = tmp[i];  // now the cell value can be safely updated with min cost
                }
            }
            min = int.MaxValue;
            for (int i = 0; i < len; i++) // so col[0..len-1] would contain the minimum cost of col[i] to some cell at end of the matrix 
            {
                if (min > map[i, 0])
                    min = map[i, 0];
            }
            return min;
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
