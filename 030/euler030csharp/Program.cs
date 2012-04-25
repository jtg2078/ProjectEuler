using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler030csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The sum of all the numbers that can be written as the sum of fifth powers of their digits:");

            TestBenchSetup();
            TestBenchLoader(loop_through_all_number_in_range_to_find_possible_match);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// the tricky part here is to find the range, the lower range we can just make it 2
        /// the upper is trickier. the number itself seems to advance a lot faster than sum of
        /// digits in their respective power.
        /// for number with 5 digits: min:10000  max:99999  min(digitpower):1 max(digitpower)=(9^5)*5=295245
        /// for number with 6 digits: min:100000 max:999999 min(digitpower):1 max(digitpower)=(9^5)*6=354294
        /// since the smallest number in 6 digits(100000) is far greater than sum of max digit in power 6
        /// so its impossible for number with 6 digits or greater that might have matching sum of digits in power
        /// the upper range must lies within number with 5 digits and since max digit power is (9^5)*5=295245
        /// thus it is the upper range
        /// </summary>
        /// <returns></returns>
        static int loop_through_all_number_in_range_to_find_possible_match()
        {
            var power_map = Enumerable.Range(0, 10).Select(l => (int)Math.Pow(l, 5)).ToArray();
            var max = power_map[9] * 5;
            var sum = 0;
            var num = 0;
            var rem = 0;
            var total = 0;
            for (int i = 2; i <= max; i++)
            {
                sum = 0;
                num = i;
                while (num > 0)
                {
                    rem = num % 10;
                    num = num / 10;
                    sum += power_map[rem];
                }
                if (i == sum)
                    total += sum;
            }
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
