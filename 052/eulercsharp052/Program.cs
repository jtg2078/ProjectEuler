using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace eulercsharp052
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The smallest positive integer, x, such that 2x, 3x, 4x, 5x, and 6x, contain the same digits:");

            TestBenchSetup();
            TestBenchLoader(test_each_number_between_100k_to_166666);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// starting with 6-digit number, the range would be 100k to 166666
        /// for each number, basically test each 6x,5x,4x,...,1x to see if they all have the same
        /// digits(essentially permutation of each other)
        /// luckily in 6-digit realm, such number does exist
        /// </summary>
        static int test_each_number_between_100k_to_166666()
        {
            int n, x;
            var same = true;
            for (n = 100000; n <= 166666; n++) // since more than 166666, 6x will produce extra digit
            {
                for (x = 6; x > 1; x--) //reverse == fail early
                {
                    same = isPerm(n, n * x);
                    if (same == false)
                        break;
                }
                if (same == true)
                    return n;
            }
            return 0;
        }

        /// <summary>
        /// basically the same code from problem 49
        /// </summary>
        static bool isPerm(int original, int candidate)
        {
            var map = new int[10];
            while (original > 0)
            {
                map[original % 10]++;
                original /= 10;
                map[candidate % 10]--;
                candidate /= 10;
            }
            return map.All(n => n == 0);
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
