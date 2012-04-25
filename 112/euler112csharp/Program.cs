using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace euler112csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The least number for which the proportion of bouncy numbers is exactly 99%:");

            TestBenchSetup();
            TestBenchLoader(plain_brute_force_iterate_each_number_and_check);

            Console.WriteLine("press any key to exit");
            Console.ReadKey();
        }

        static int plain_brute_force_iterate_each_number_and_check()
        {
            var number = new int[10];
            var count = 1;
            var carry = true;
            var bouncy = 0;

            for (int n = 1; ; n++)
            {
                // increase the number(represented) in array base by 1
                var c = 0;
                do
                {
                    number[c]++;
                    if (number[c] == 10)
                    {
                        number[c] = 0;
                        carry = true;
                    }
                    else
                    {
                        carry = false;
                    }
                    c++;
                } while (carry == true);

                if (c > count)
                    count++;

                // checks if the number is decreasing number
                var is_dec = true;
                for (int i = 0; i < count - 1; i++)
                {
                    if (number[i] > number[i + 1])
                    {
                        is_dec = false;
                        break;
                    }
                }

                // checks if the number is increasing number
                var is_inc = true;
                for (int i = 0; i < count - 1; i++)
                {
                    if (number[i] < number[i + 1])
                    {
                        is_inc = false;
                        break;
                    }
                }

                if (is_dec == false && is_inc == false)
                    bouncy++;

                var ratio = bouncy * 100.0 / n;
                if (ratio >= 99)
                {
                    return n;
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
