using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler033csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The sum of all numbers that can be written as pandigital products:");

            TestBenchSetup();
            TestBenchLoader(using_lowest_terms_to_limit_search_range);
            TestBenchLoader(brute_froce_all_fraction_under_range);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// since the range is under 1, all curious fraction will have a equivalent fraction of
        /// 1-digit numerator and denominator. so basically all curious fraction is a multiple of 
        /// one of the lowest term single digit numerator and denominator fraction.
        /// start from each lowest term fraction and iterate and test all the multiple below 1 to find
        /// curious fraction
        /// </summary>
        static int using_lowest_terms_to_limit_search_range()
        {
            int result_a = 1;
            int result_b = 1;
            int len, start, i;
            foreach (var seed in generate_seeds())
            {
                len = 99 / seed.b;
                start = 10 % seed.a == 0 ? 10 / seed.a : 10 / seed.a + 1;
                for (i = start; i <= len; i++)
                {
                    find_curious_fraction2(seed.a * i, seed.b * i, ref result_a, ref result_b);
                }
            }
            return result_b / find_GCD(result_a, result_b);
        }

        /// <summary>
        /// iterate through all possible fraction(2-digit max) under 1
        /// </summary>
        static int brute_froce_all_fraction_under_range()
        {
            int result_a = 1;
            int result_b = 1;
            
            for (int b = 99; b >= 10; b--)
            {
                for (int a = b - 1; a >= 10; a--)
                {
                    find_curious_fraction2(a, b, ref result_a, ref result_b);
                }
            }
            return result_b / find_GCD(result_a, result_b);
        }

        /// <summary>
        /// create a struct to represent a fraction
        /// </summary>
        struct fraction
        {
            public int a; // numerator
            public int b; // denominator
        }

        /// <summary>
        /// use Euclidean algorithm to find GCD
        /// http://en.wikipedia.org/wiki/Euclidean_algorithm
        /// </summary>
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

        /// <summary>
        /// generate a list of fraction which contain lowest term single digit fraction for both
        /// numerator and denominator. e.g. 1/2, 1/3, 2/3, 1/4, 3/4, 1/5, 2/5.... 
        /// </summary>
        static IEnumerable<fraction> generate_seeds()
        {
            var seeds = new List<fraction>();
            for (int b = 2; b <= 9; b++)
            {
                for (int a = 1; a < b; a++)
                {
                    if (find_GCD(a, b) == 1 || a == 1)
                        seeds.Add(new fraction() { a = a, b = b });
                }
            }
            return seeds;
        }

        /// <summary>
        /// test to see if given fraction(a/b) is a "curious fraction"
        /// since there are 4 possible ways of finding a curious fraction
        ///     1. a1 a2 / b1 b2    =   a1 / b2 (a2 is equal to b1, and b1 is not zero)
        ///     2. a1 a2 / b1 b2    =   a2 / b1 (a1 is equal to b2, and b2 is not zero)
        ///     3. a1 a2 / b1 b2    =   a1 / b1 (a2 is equal to b2, and b2 is not zero)
        ///     4. a1 a2 / b1 b2    =   a2 / b2 (a1 is equal to b1, and b1 is not zero)
        /// </summary>
        static void find_curious_fraction2(int a, int b, ref int result_a, ref int result_b)
        {
            var a1 = a / 10;
            var a2 = a % 10;
            var b1 = b / 10;
            var b2 = b % 10;

            if (b2 == a1 && b2 != 0 && a * b1 == b * a2)
            {
                result_a *= a2;
                result_b *= b1;
            }
            else if (b1 == a2 && b1 != 0 && a * b2 == b * a1)
            {
                result_a *= a1;
                result_b *= b2;
            }
            else if (b2 == a2 && b2 != 0 && a * b1 == b * a1)
            {
                result_a *= a1;
                result_b *= b1;
            }
            else if (b1 == a1 && b1 != 0 && a * b2 == b * a2)
            {
                result_a *= a2;
                result_b *= b2;
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
