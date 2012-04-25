using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler044csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The smallest pair of pentagonal numbers whose sum and difference is pentagonal:");

            //TestBenchSetup();
            TestBenchLoader(brute_force_with_precal_pentagonals_and_loop_every_combination);
            TestBenchLoader(parallel_version_with_on_fly_check_for_sum_and_diff);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// pre-calculate all the pentagonal numbers to 3000 (randomly picked) and save to hashset
        /// loop through all possible combinations and return 1st result as answer.
        /// as to why 1st result happens to be the answer? i am not sure
        /// Originally i thought this question has something to do with pentagonal number theorem
        /// (http://en.wikipedia.org/wiki/Pentagonal_number_theorem), but i guess not.
        /// </summary>
        /// <returns></returns>
        static int brute_force_with_precal_pentagonals_and_loop_every_combination()
        {
            Func<int, int> pn = (n) => n * (3 * n - 1) / 2;
            var numbers = new HashSet<int>();
            Enumerable.Range(1, 3000)
                .Select(l => pn(l))
                .ToList()
                .ForEach(l => numbers.Add(l));

            foreach (var j in numbers)
            {
                foreach (var k in numbers)
                {
                    if (numbers.Contains(j + k) && numbers.Contains(Math.Abs(j - k)))
                        return Math.Abs(j - k);
                }
            }

            return 0;
        }

        /// <summary>
        /// the parallelized version. the only differences are the following:
        /// 1. didnt use pre-calculated hashset, the hashset contain part make the whole thing even slower than linear way
        /// 2. using the forumla to check sum and diff (http://en.wikipedia.org/wiki/Pentagonal_number)
        /// 3. interesting read on how to check if sqrt of a number is natural or not
        ///    (http://stackoverflow.com/questions/295579), ah the John Carmack hack, read about it
        ///    on hacker news, still not sure who was the original author of that magical number
        /// 4. only slightly faster compared with linear way: 120ish vs 88ish
        /// </summary>
        /// <returns></returns>
        static int parallel_version_with_on_fly_check_for_sum_and_diff()
        {
            var limit = 3000;
            var size = limit / 7;
            var list = Enumerable.Range(1, limit)
                .Select(n => n * (3 * n - 1) / 2).ToList<int>();
            var work_items = Enumerable.Range(1, 7)
                .Select(l => new WorkItem
                    {
                        from = l == 1 ? 0 : (l - 1) * size + 1,
                        to = l == 7 ? limit - 1 : l * size,
                        signal = new AutoResetEvent(false),
                        result = int.MaxValue
                    }).ToList();
            foreach (var item in work_items)
            {
                ThreadPool.QueueUserWorkItem((o) =>
                {
                    int i, j;
                    double sum, diff;
                    var found = false;
                    var work_item = o as WorkItem;
                    for (i = work_item.from; i <= work_item.to; i++)
                    {
                        j = list[i];
                        foreach (var k in list)
                        {
                            sum = (Math.Sqrt(24 * (j + k) + 1) + 1) / 6;
                            diff = (Math.Sqrt(24 * Math.Abs(j - k) + 1) + 1) / 6;
                            if (sum - (int)sum < 0.00001 && diff - (int)diff < 0.00001)
                            {
                                work_item.result = Math.Abs(j - k);
                                found = true;
                                break;
                            }
                        }
                        if (found) break;
                    }
                    work_item.signal.Set();
                }, item);
            }
            var result = int.MaxValue;
            work_items.ForEach(w => { w.signal.WaitOne(); result = w.result < result ? w.result : result; });
            return result;
        }

        class WorkItem
        {
            public int from;
            public int to;
            public AutoResetEvent signal;
            public int result;
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
