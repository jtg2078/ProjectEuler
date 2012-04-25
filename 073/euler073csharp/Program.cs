using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler073csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The number of reduced fraction lie between 1/3 and 1/2 for d<=12000: ");

            TestBenchSetup();
            TestBenchLoader(brute_force_with_euclidean_algorithm_to_find_gcd);
            TestBenchLoader(brute_force_with_seive_to_minimize_gcd_checking);
            TestBenchLoader(using_stern_brocot_tree_to_count_fraction_in_between);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// just brute foce every number for each denominator and using euclidean aglorithm test if the fraction is in
        /// reduced form
        /// </summary>
        static int brute_force_with_euclidean_algorithm_to_find_gcd()
        {
            var low_a = 1;
            var low_b = 3;
            var high_a = 1;
            var high_b = 2;
            var max = 12000;
            int p, q, low, high;
            var result = 0;
            for (q = 5; q <= max; q++)
            {
                low = (low_a * q) / low_b + 1;
                high = (high_a * q - 1) / high_b;
                for (p = low; p <= high; p++)
                {
                    if (find_GCD(p, q) == 1)
                        result++;
                }
            }
            return result;
        }

        /// <summary>
        /// modified version of the first method with seive to reduce number of fraction to test for co-prime
        /// not much improvement, and the map size is too big to fit in cache, lawl
        /// </summary>
        static int brute_force_with_seive_to_minimize_gcd_checking()
        {
            var low_a = 1;
            var low_b = 3;
            var high_a = 1;
            var high_b = 2;
            var max = 12000;
            int p, q, n, m, low, high, count;
            var map = new bool[max + 1, 2000];
            var result = 0;
            for (q = 5; q <= max; q++)
            {
                low = (low_a * q) / low_b + 1;
                high = (high_a * q - 1) / high_b;
                count = high - low + 1;
                for (p = low; p <= high; p++)
                {
                    if (map[q, p - low] == false)
                    {
                        for (n = q * 2, m = p * 2; n <= max; n += q, m += p)
                        {
                            map[n, m - (n / 3 + 1)] = true;
                        }

                        if (find_GCD(p, q) == 1)
                            result++;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// using the Stern–Brocot tree. see http://en.wikipedia.org/wiki/Stern-brocot_tree for details
        /// basically an infinite binary search tree(in this case, stops at limit(which is 12k)), in which 
        /// can be used to count number of nodes in bwetween two fractions(1/3, 1/2)
        /// </summary>
        static int using_stern_brocot_tree_to_count_fraction_in_between()
        {
            var limit = 12000;
            Func<int, int, int, int,int> countSB = null;
            countSB = (ln, ld, rn, rd) =>
                {
                    var mn = ln + rn;
                    var md = ld + rd;
                    if (md > limit)
                        return 0;
                    else
                    {
                        var count = 1;
                        count += countSB(ln, ld, mn, md);
                        count += countSB(mn, md, rn, rd);
                        return count;
                    }
                };
            return countSB(1, 3, 1, 2);
        }

        static int find_GCD(int x, int y)
        {
            while (x > 0)
            {
                y = y % x;
                y = y + x; // the next three lines is just swapping x and y
                x = y - x;
                y = y - x;
            }
            return y;
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
