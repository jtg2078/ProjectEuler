using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler092csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The numbers of starting numbers below ten million arrived at 89: ");

            TestBenchSetup();
            TestBenchLoader(brute_force_with_memoization);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// basically use memoization to store prevous calculated result
        /// and the long nested loop speeds up the process by not breaking down the number and generate first pass
        /// number iteratively.
        /// </summary>
        static int brute_force_with_memoization()
        {
            var map = new Dictionary<int, bool>();
            var squares = Enumerable.Range(0, 10).Select(l => l * l).ToArray();
            Func<int, bool> expand_chain = null;
            expand_chain = num =>
            {
                if (num == 89)
                    return true;
                else if (num <= 1)
                    return false;
                else
                    return map.ContainsKey(num) == true ? map[num] : (map[num] = expand_chain(form_new_number(num)));
            };
            var count = 0; // since the question asks for 10mil, which means 7 nested loops
            for (int a = 0; a < 10; a++)
            {
                var a0 = squares[a];
                for (int b = 0; b < 10; b++)
                {
                    var b0 = a0 + squares[b];
                    for (int c = 0; c < 10; c++)
                    {
                        var c0 = b0 + squares[c];
                        for (int d = 0; d < 10; d++)
                        {
                            var d0 = c0 + squares[d];
                            for (int e = 0; e < 10; e++)
                            {
                                var e0 = d0 + squares[e];
                                for (int f = 0; f < 10; f++)
                                {
                                    var f0 = e0 + squares[f];
                                    for (int g = 0; g < 10; g++)
                                    {
                                        if (expand_chain(f0 + squares[g]) == true)
                                            count++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return count;
        }

        static int form_new_number(int num)
        {
            var result = 0;
            do
            {
                var d = num % 10;
                result += d * d;
            } while ((num /= 10) > 0);
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
