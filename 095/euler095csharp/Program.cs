using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace euler095csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The smallest member of the longest amicable chain: ");

            //TestBenchSetup(); // since theres parallel code
            TestBenchLoader(calculate_divisors_sum_using_sieve_and_find_out_longest_chain_in_parallel);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        static int calculate_divisors_sum_using_sieve_and_find_out_longest_chain_in_parallel()
        {
            var max = 1000000;
            var sum_map = new int[max + 1];
            var bound = (int)Math.Sqrt(max);
            int n, d, q;
            // since the sieve method will not be able to encounter 1, so add 1 to every number's divisor sum first
            for (n = 1; n <= max; n++)
            {
                sum_map[n]++;
            }
            // sieve for calculating sum of divisors for each number 
            for (d = 2; d <= bound; d++)
            {
                n = d * d;
                sum_map[n] += d;
                for (n = n + d, q = d + 1; n <= max; n += d, q++)
                {
                    sum_map[n] += (d + q);
                }
            }
            // find out the chain length for each number in parallel and record the longest chain number and its min
            var THREAD_COUNT = Environment.ProcessorCount - 1;
            var wait_handles = Enumerable.Range(0, THREAD_COUNT).Select(w => new AutoResetEvent(false)).ToArray();
            var results = new int[THREAD_COUNT];
            for (int i = 0; i < THREAD_COUNT; i++)
            {
                ThreadPool.QueueUserWorkItem(o =>
                {
                    var nth = (int)o;
                    var longest = new HashSet<int>();
                    var longest_num = 0;
                    for (int num = nth; num <= max; num += THREAD_COUNT)
                    {
                        var accum = new HashSet<int>();
                        var cur = num;
                        while (accum.Add(cur) == true && cur <= max)
                        {
                            cur = sum_map[cur];
                        }
                        if (cur == num && accum.Count > longest.Count)
                        {
                            longest = accum;
                            longest_num = num;
                        }
                    }
                    results[nth] = longest.Min();
                    wait_handles[nth].Set();
                }, i);
            }
            for (int i = 0; i < THREAD_COUNT; i++)
            {
                wait_handles[i].WaitOne();
            }
            return results.Min();
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
