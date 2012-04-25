using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace euler042csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The number of triangle words:");

            TestBenchSetup();
            TestBenchLoader(using_linq_to_make_my_life_easier);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// not sure why this is sooo fast(2500ish ticks on my i7 920, with intel ssd)
        /// note to self: try to find out whats going on under the hood, rather than spending time on reddit
        /// </summary>
        /// <returns></returns>
        static int using_linq_to_make_my_life_easier()
        {
            var nums = new HashSet<int>();

            Enumerable.Range(1, 30) // see http://en.wikipedia.org/wiki/Longest_word_in_English
                .Select(l => l * (l + 1) / 2)
                .ToList()
                .ForEach(l => nums.Add(l));

            return File.ReadAllText("words.txt").Split(new char[] { ',' })
                .Select(l => l.Trim(new char[] { '"' }).ToCharArray().Aggregate(0, (s, n) => s += (Convert.ToInt32(n) - 64)))
                .Where(l => nums.Contains(l))
                .Count();
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
