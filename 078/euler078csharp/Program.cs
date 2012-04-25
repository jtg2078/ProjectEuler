using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler078csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The least value of n for which p(n) is divisible by one million: ");

            TestBenchSetup();
            TestBenchLoader(using_euler_generating_function);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// similar to solution to problem 76. The difference here p(n) is built from ground up(1,2,3....) until answer is found
        /// so for each p(n), all the info is already calculated(p(n-k)). So array is used instead of dictionary.
        /// in order to avoid overflow, each p(n) result is masked by remainder of 1mil(since the question asked for 1st number that is
        /// divisible by 1mil
        /// </summary>
        static int using_euler_generating_function()
        {
            var map = new int[100000]; // should be big enough
            map[0] = 1;
            int a, b, p1, p2, bound, n, result;
            n = 1;
            while (true)
            {
                bound = (int)Math.Sqrt(n); // since p(negative number(a or b)) is 0, so stops at x=~x^2
                result = 0;
                for (int k = 1, sign = 1; k <= bound; k++, sign = -sign)
                {
                    a = n - (k * (3 * k - 1) / 2);
                    b = n - (k * (3 * k + 1) / 2);
                    p1 = a <= 0 ? a < 0 ? 0 : 1 : map[a];
                    p2 = b <= 0 ? b < 0 ? 0 : 1 : map[b];
                    result += (sign * (p1 + p2));
                }
                map[n] = result % 1000000; // masking
                if (map[n] == 0)
                    return n;
                n++;
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
