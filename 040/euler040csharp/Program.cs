using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler040csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The value of the following expression:");

            TestBenchSetup();
            TestBenchLoader(increasing_numbers_and_check_as_it_goes);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// pretty straight-forward:
        /// there is no need to really create a concatenated number list, all we need is just the number and index
        /// basically loop through the number and save the index at same time, so when the number is single
        /// digits(1~9), increase index with length of the digit(1 atm), as number goes from 10,11,12...
        /// increase the index by 2 for each iterated number, check to see if the index match the check_point(d1,d10,d100...)
        /// however, since index may be multiple of 10s(since we are increasing the index by length of digits, which can be 3 for 
        /// number like 453, and thats where the whole "if (index >= check_point)..." code comes to play
        /// </summary>
        /// <returns></returns>
        static int increasing_numbers_and_check_as_it_goes()
        {
            var index = 1;
            var num_length = 1;
            var num = 1;
            var tens = 10;
            var result = 1;
            var check_point = 1;
            while (index <= 1000000)
            {
                if (index >= check_point)
                {
                    if (index == check_point)
                        result *= Convert.ToByte(num.ToString()[0]) - 48;
                    else
                        result *= Convert.ToByte((num - 1).ToString()[num_length - (index - check_point)]) - 48;

                    check_point *= 10; //since the q asks for d1 * d10 * d100 * d1000....etc
                }

                num++;
                index += num_length;
                if (num >= tens)
                {
                    tens *= 10;
                    num_length++;
                }
            }
            return result;
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
