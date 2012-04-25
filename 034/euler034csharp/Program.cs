using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler034csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The sum of all numbers which are equal to the sum of the factorial of their digits:");

            //TestBenchSetup(); // comment out this call if testing the parallel way
            TestBenchLoader(test_each_number_under_calculated_upper_bound);
            TestBenchLoader(test_numbers_in_parallel);
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        static void test_upperbound()
        {
            var unit = Enumerable.Range(1, 9).Aggregate((s, i) => s *= i);
            var length = 1;
            int digit_max, digit_min, factorial_max;
            while (true)
            {
                var s_max = Enumerable.Range(1, length)
                    .Select(i => "9")
                    .Aggregate(string.Empty, (s, i) => s += i);
                var s_min = Enumerable.Range(1, length)
                    .Select(i => "1")
                    .Aggregate(string.Empty, (s, i) => s += i);
                digit_max = Convert.ToInt32(s_max);
                digit_min = Convert.ToInt32(s_min);
                factorial_max = unit * length;
                Console.WriteLine(string.Format("{0}-digit: min:{1} max:{2} factorial max={3}",
                    length, s_min.PadRight(10), s_max.PadRight(10), factorial_max));
                if (digit_min > factorial_max)
                    break;
                length++;
            }
            Console.WriteLine();
        }

        /// <summary>
        /// pretty similar to question 30, so the key here is to find a upperbound and test each number blow it
        /// from test_upperbound method, we know that for 8 digits number, the max(99999999) factorial is
        /// 9!*8 = 2903040, and minimal 8-digit number is 11111111, which is greater. so we know the upper bound
        /// is within 7-digit number, so we can just use 9!*7 = 2540160 as upperbound.
        /// </summary>
        /// <returns></returns>
        static int test_each_number_under_calculated_upper_bound()
        {
            var map = Enumerable.Range(0, 10)
                .Select(n => n == 0 ? 1 : Enumerable.Range(1, n).Aggregate((s, i) => s *= i)).ToArray();
            var limit = map[9] * 7;
            int sum, num;
            var result = 0;
            for (int i = limit; i >=3 ; i--)
            {
                sum = 0;
                num = i;
                do
                {
                    sum += map[num % 10];
                } while ((num /= 10) > 0);
                if (sum == i)
                    result += i;
            }
            return result;
        }

        class WorkItem
        {
            public int from;
            public int to;
            public AutoResetEvent signal;
            public int result;
        }

        /// <summary>
        /// the testing for each number for match can be easily parallelized.
        /// so this method divides all the number to be tested in 7 chunks, and test
        /// each chunk in parallel(through threadpool).
        /// the method is coded in such way that no lock is required, since by the time we need
        /// to access the result of each chunk, the thread is already finished.
        /// </summary>
        /// <returns></returns>
        static int test_numbers_in_parallel()
        {
            var map = Enumerable.Range(0, 10)
                .Select(n => n == 0 ? 1 : Enumerable.Range(1, n).Aggregate((s, i) => s *= i)).ToArray();
            var limit = map[9] * 7;
            var size = limit / 7;
            var result = 0;
            var work_items = Enumerable.Range(1, 7)
                .Select(l => new WorkItem
                    {
                        from = l == 1 ? 3 : (l - 1) * size + 1,
                        to = l * size,
                        signal = new AutoResetEvent(false),
                        result = 0
                    }).ToList();
            foreach (var item in work_items)
            {
                ThreadPool.QueueUserWorkItem((o) =>
                    {
                        int sum, num, i;
                        var work_item = o as WorkItem;
                        for (i = work_item.from; i <=work_item.to; i++)
                        {
                            sum = 0;
                            num = i;
                            do
                            {
                                sum += map[num % 10];
                            } while ((num /= 10) > 0);
                            if (sum == i)
                                work_item.result += i;
                        }
                        work_item.signal.Set();
                    }, item);
            }
            work_items.ForEach(w => { w.signal.WaitOne(); result += w.result; });
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
