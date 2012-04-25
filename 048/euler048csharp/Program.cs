using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler048csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The last ten digits of the series, 1^(1) + 2^(2) + 3^(3) + ... + 1000^(1000)");

            TestBenchSetup();
            TestBenchLoader(only_calculate_last_10_digits_for_each_number_and_its_power);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// lets say the question is find the last 2 digits of 17^1 + 17^2 + ... +17^5
        /// if we just take the last 2 digits in each iteration: 
        /// n:1             (17)      1*17=17             1*17=(17)
        /// n:2            2(89)      17*17=289           17*17=2(89)                                  
        /// n:3           49(13)      289*17=4913         89*17=15(13)
        /// n:4          835(21)      4913*17=83521       13*17=2(21)
        /// n:5        14198(57)      83521*17=1418857    21*17=3(57)
        ///               =1(97)                               =1(97)
        /// this work because 289*17 = (200+89)*17 = 200*17 + 89*17 = 3400 + 1513
        /// </summary>
        static long only_calculate_last_10_digits_for_each_number_and_its_power()
        {
            long mask = 10000000000;
            long result = 0;
            long num, power, tmp;
            for (num = 1; num <= 1000; num++)
            {
                tmp = num;
                for (power = 1; power < num; power++)
                {
                    tmp = (tmp * num) % mask;
                }
                result = (result + tmp) % mask;
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
