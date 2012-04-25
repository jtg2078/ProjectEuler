using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler076csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The number of ways can one hundred be written as a sum of at least two positive integers: ");

            TestBenchSetup();
            TestBenchLoader(using_euler_generating_function_with_memoization);
            TestBenchLoader(using_euler_generating_function_with_memoization2);


            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// got the formula from http://mathworld.wolfram.com/PartitionFunctionP.html (#11)
        /// which generates the number partitions p(n)~, using cache to saved all previous calculated results,
        /// which boost the speed tremendously
        /// additional info: http://www.math.clemson.edu/~kevja/PAPERS/ComputingPartitions-MathComp.pdf
        /// </summary>
        static int using_euler_generating_function_with_memoization()
        {
            Func<int, int> p = null;
            Dictionary<int, int> cache = new Dictionary<int, int>();
            p = (n) =>
                {
                    if (n < 0)      // found out about the case when p(0) and p(negative) on 
                        return 0;   // http://en.wikipedia.org/wiki/Partition_%28number_theory%29#Partition_function
                    else if (n == 0)
                        return 1;
                    else
                    {
                        int a, b, p1, p2;
                        var result = 0;
                        for (int k = 1, sign = 1; k <= n; k++)
                        {
                            a = n - (k * (3 * k - 1) / 2);
                            b = n - (k * (3 * k + 1) / 2);
                            p1 = cache.ContainsKey(a) ? cache[a] : (cache[a] = p(a));
                            p2 = cache.ContainsKey(b) ? cache[b] : (cache[b] = p(b));
                            result += (sign * Math.Abs(p1 + p2));
                            sign = -sign;
                        }
                        return result;
                    }
                };
            return p(100) - 1; // since 100 itself doesnt count(2 or more numbers partitions)
        }

        static int using_euler_generating_function_with_memoization2()
        {
            Func<int, int> p = null;
            Dictionary<int, int> cache = new Dictionary<int, int>();
            p = (n) =>
            {
                int a, b, p1, p2;
                var result = 0;
                for (int k = 1, sign = 1; k <= n; k++)
                {
                    a = n - (k * (3 * k - 1) / 2);
                    b = n - (k * (3 * k + 1) / 2);
                    p1 = a <= 0 ? a < 0 ? 0 : 1 : cache.ContainsKey(a) ? cache[a] : (cache[a] = p(a));
                    p2 = b <= 0 ? b < 0 ? 0 : 1 : cache.ContainsKey(b) ? cache[b] : (cache[b] = p(b));
                    result += (sign * (p1 + p2));
                    sign = -sign;
                }
                return result;
            };
            return p(100) - 1; // since 100 itself doesnt count(2 or more numbers partitions)
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
