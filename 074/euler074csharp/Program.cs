using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace euler074csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The number of chains contain exactly sixty non-repeating terms: ");
            
            TestBenchLoader(parallel_version_of_brute_force_with_memoization);
            TestBenchSetup(); // moved to here so it wont hinder the multithreaded version 
            TestBenchLoader(brute_force_with_memoization_for_chain_length);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// bascially brute force every number to find out its chain length. with the lep of 
        /// memoization, a lot more calculation and drilling down the chains are avoided.
        /// see tracer method for detail
        /// </summary>
        static int brute_force_with_memoization_for_chain_length()
        {
            var result = 0;
            var cache = new Dictionary<int, int>();
            for (int i = 2; i < 1000000; i++)
            {
                if (tracer(i, i, 0, 0, cache) == 60)
                    result++;
            }
            return result;
        }

        /// <summary>
        /// he parallel version of the previous method(7 concurrent thread via threadpool, pick 7 cuz i have 8 cores)
        /// decided to let every concurent work unit to have its own cache, since i dont want to deal with locking the shared cache.
        /// On the down side the memoization would not be as effective.(locking vs cache hit + calculation. Didnt try any benchmark
        /// comparsion, so im not sure) (about 4 times faster than single threaded version)
        /// </summary>
        static int parallel_version_of_brute_force_with_memoization()
        {
            var work_items = Enumerable.Range(0, 7)
                .Select(w => new WorkItem()
                {
                    index = w,
                    result = 0,
                    signal = new AutoResetEvent(false)
                }).ToList();
            foreach (var item in work_items)
            {
                ThreadPool.QueueUserWorkItem((o) =>
                {
                    var cache = new Dictionary<int, int>();
                    var work_item = o as WorkItem;
                    var i = work_item.index;
                    while (i < 1000000)
                    {
                        if (tracer(i, i, 0, 0, cache) == 60)
                            work_item.result++;
                        i += 7;
                    }
                    work_item.signal.Set();
                }, item);
            }
            var total = 0;
            work_items.ForEach(w => { w.signal.WaitOne(); total += w.result; });
            return total;
        }

        /// <summary>
        /// basically the recursive method that drills down the chain and stops when repeated pattern is found.
        /// since the question stated that pattern's dept at most is 3. so r1, r2, and r3 is used to save previous
        /// 3 numbers in the chain. memoization is used(the cache variable)
        /// </summary>
        static int tracer(int num, int r1, int r2, int r3, Dictionary<int, int> cache)
        {
            if (cache.ContainsKey(num))
                return cache[num];
            else
            {
                var digit_sum = calculate_digits_sum(num);
                if (digit_sum == r1 || digit_sum == r2 || digit_sum == r3)
                    return 1;

                if (r1 == 0) // since sum of digits factorial can never be zero, this is a good way to start the initial accumulation
                    r1 = digit_sum;
                else if (r2 == 0)
                    r2 = digit_sum;
                else if (r3 == 0)
                    r3 = digit_sum;
                else
                {
                    r1 = r2;
                    r2 = r3;
                    r3 = digit_sum;
                }
                return (cache[num] = 1 + tracer(digit_sum, r1, r2, r3, cache));
            }
        }

        class WorkItem
        {
            public int index;
            public int result;
            public AutoResetEvent signal;
        }

        // precalculate each digit's factorial sum
        static int[] factorial_map =
            Enumerable.Range(0, 10).Select(n => n == 0 ? 1 : Enumerable.Range(1, n).Aggregate((a, b) => a *= b)).ToArray();

        /// <summary>
        /// break down num into digits and sum up each digit's factorial sum
        /// </summary>
        static int calculate_digits_sum(int num)
        {
            var rem = 0;
            var sum = 0;
            while (num > 0)
            {
                num = Math.DivRem(num, 10, out rem);
                sum += factorial_map[rem];
            }
            return sum;
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
