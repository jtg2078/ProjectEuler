using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler045csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The next triangle number that is also pentagonal and hexagonal?");

            TestBenchSetup();
            TestBenchLoader(incrementally_generate_each_type_of_number_and_compare);
            TestBenchLoader(skip_generate_triangle_number_with_overflow_check);
            TestBenchLoader(skip_generate_triangle_number_without_overflow_check);
            TestBenchLoader(only_generate_hexagonal_and_check_if_its_pentagonal);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// since Hexagonal number grow the fastest(t:0.5x p:1.5x h:2x)
        /// so for each H(n) as n++, let P(n) and T(n) catches, and there can be only 4 outcomes
        /// P(n) > H(n) || P(n) == H(n)
        /// T(n) > H(n) || T(n) == H(n)
        /// so if P(n) == H(n) and T(n) == H(n), then thats the answer
        /// </summary>
        /// <returns></returns>
        static long incrementally_generate_each_type_of_number_and_compare()
        {
            long tn = 285;
            long pn = 165;
            long hn = 144;
            long t = 0;
            long p = 0;
            long h = 0;
            while (true)
            {
                hn++;
                h = hn * (2 * hn - 1);

                while (t < h)
                {
                    t = tn * (tn + 1) / 2;
                    tn++;
                }

                while (p < h)
                {
                    p = pn * (3 * pn - 1) / 2;
                    pn++;
                }

                if (t == p && p == h)
                    return t;
            }
        }

        /// <summary>
        /// from the question's discussion post, learned the fact that
        /// Every hexagonal number is a triangular number(http://en.wikipedia.org/wiki/Hexagonal_number)
        /// so we just need to check pentagonal number
        /// this method use the .net built-in checked keyword for overflow checking
        /// it took me a while to realize that tn, pn, and hn has to be long as well
        /// (i was using var pn = 164, and then infinite loop)
        /// since hn * (2 * hn - 1) can overflow if hn is type int32 (int32 * int32) produce int32,
        /// even though when we get the answer, tn=55386, pn=31978 and hn=27694
        /// there is performance penality when using checked keywoard
        /// </summary>
        /// <returns></returns>
        static long skip_generate_triangle_number_with_overflow_check()
        {
            long pn = 165;
            long hn = 144;
            long p = 0;
            long h = 0;
            while (true)
            {
                h = checked(hn * (2 * hn - 1));
                hn++;

                while (p < h)
                {
                    p = checked(pn * (3 * pn - 1) / 2);
                    pn++;
                }

                if (p == h)
                    return p;
            }
        }

        /// <summary>
        /// same as method above, but without checked keyword
        /// </summary>
        /// <returns></returns>
        static long skip_generate_triangle_number_without_overflow_check()
        {
            long pn = 165;
            long hn = 144;
            long p = 0;
            long h = 0;
            while (true)
            {
                hn++;
                h = hn * (2 * hn - 1);

                while (p < h)
                {
                    p = pn * (3 * pn - 1) / 2;
                    pn++;
                }

                if (p == h)
                    return p;
            }
        }

        /// <summary>
        /// instead of playing "catch up", just check for every hexagonal number, if it is also a 
        /// Pentagonal number by using the formula(same as problem 44)
        /// </summary>
        /// <returns></returns>
        static long only_generate_hexagonal_and_check_if_its_pentagonal()
        {
            long hn = 144;
            long h = 0;
            double pn = 0;
            while (true)
            {
                hn++;
                h = hn * (2 * hn - 1);
                pn = (Math.Sqrt(24 * h + 1) + 1) / 6;
                
                if (pn == (int)pn)
                    return h;
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
