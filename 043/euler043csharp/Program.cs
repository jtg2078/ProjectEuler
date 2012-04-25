using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler043csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The sum of all 0 to 9 pandigital numbers with this property");

            TestBenchSetup();
            TestBenchLoader(precalculate_possible_d8d9d10_to_limit_permu_range);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// basically first calculate all the 3-digit number that is multiple of 17 and contain all unique number
        /// then generate permutation for each number and check for all conditions specified by the question
        /// </summary>
        /// <returns></returns>
        static long precalculate_possible_d8d9d10_to_limit_permu_range()
        {
            var num = 17;
            var upper = 999;
            var cur = num * 6;
            char[] tmp;
            var seeds = new List<string>();
            while (cur <= upper)
            {
                cur += num;
                tmp = cur.ToString().ToCharArray();

                if (tmp.Length == tmp.Distinct().Count())
                    seeds.Add(cur.ToString());
            }
            var permu = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            long result = 0;
            foreach (var item in seeds)
            {
                result += find_largest_prime_within_permutations(Enumerable.Range(0, 10)
                     .Where(l => item.Contains(l.ToString()) == false).ToArray(),
                     Convert.ToInt32(item[0]) - 48,
                     Convert.ToInt32(item[1]) - 48,
                     Convert.ToInt32(item[2]) - 48);
            }
            return result;
        }

        /// <summary>
        /// basically the same code from question 24, which uses the algorithm from knuth's book
        /// http://en.wikipedia.org/wiki/Permutation#Systematic_generation_of_all_permutations
        /// </summary>
        /// <param name="a">contains seed values for permutation to be generated from</param>
        /// <returns></returns>
        static long find_largest_prime_within_permutations(int[] a, int d8, int d9, int d10)
        {
            var j = 0;
            var l = 0;
            var tmp = 0;
            var len = a.Length;
            var z = 0;
            long result = 0;
            while (true)
            {
                //step 1. Find the largest index j such that a[j] < a[j + 1].
                for (j = len - 2; j >= 0; j--)
                {
                    if (a[j] < a[j + 1])
                        break;
                }
                if (j == -1)
                    break; // no more permutation, since no such index exist
                //step 2. Find the largest index l such that a[j] < a[l]
                for (l = len - 1; l >= 0; l--)
                {
                    if (a[j] < a[l])
                        break;
                }
                //step 3. Swap a[j] with a[l].
                tmp = a[j];
                a[j] = a[l];
                a[l] = tmp;
                //step 4. Reverse the sequence from a[j + 1] up to and including the final element a[n]
                z = len - 1;
                for (l = j + 1; l < len; l++)
                {
                    if (l >= z)
                        break;
                    tmp = a[l];
                    a[l] = a[z];
                    a[z] = tmp;
                    z--;
                }
                if (a[5] == 5 && (a[6] * 100 + d8 * 10 + d9) % 13 == 0 && a[3] % 2 == 0 && (a[2] + a[3] + a[4]) % 3 == 0 
                    && (a[4] * 100 + a[5] * 10 + a[6]) % 7 == 0 && (a[5] * 100 + a[6] * 10 + d8) % 11 == 0)
                {
                    result += Convert.ToInt64(a
                        .Aggregate(new StringBuilder(len), (s, n) => s.Append(n)).ToString() + d8 + d9 + d10);
                }
            }
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
