using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler062csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The smallest cube for which exactly five permutations of its digits are cube:");

            TestBenchSetup();
            TestBenchLoader(using_dictionary_and_key_to_find_match);
            TestBenchLoader(using_linq_with_group_by_calculated_key);
            TestBenchLoader(brute_force_test_all_possible_permutation);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// basically using dictionary to do bookkeeping, iterate all the number within range
        /// and stop as soon as the answer is found.
        /// using calculate_key method such that all the permuation of a cube number would produce same key number
        /// as for range, arbitrarily choose 9999, and it worked lol
        /// </summary>
        static long using_dictionary_and_key_to_find_match()
        {
            long num, key;
            var map = new Dictionary<long, List<long>>();
            for (int i = 346; i < 9999; i++)
            {
                num = (long)Math.Pow(i, 3);
                key = calculate_key(num);

                if (map.ContainsKey(key) == true)
                {
                    map[key].Add(num);
                    if (map[key].Count == 5)
                        return map[key][0];
                }
                else
                    map.Add(key, new List<long>() { num });
            }
            return 0;
        }

        /// <summary>
        /// basically the linq version, since with the grouping and the count, it will
        /// go through all the numbers within range
        /// </summary>
        static long using_linq_with_group_by_calculated_key()
        {
            var start = 346;
            var end = 9999;
            var map = Enumerable.Range(start, end - start + 1)
                .Select(n => new
                {
                    num = (long)Math.Pow(n, 3),
                    key = calculate_key((long)Math.Pow(n, 3))
                })
                .GroupBy(n => n.key)
                .First(n => n.Count() == 5).First();
            return map.num;
        }

        /// <summary>
        /// basically pre-calcuated all the cube number within range
        /// for each number in range, iterate though all the cube number to see if the count match to question's condition
        /// </summary>
        static long brute_force_test_all_possible_permutation()
        {
            var start = 346;
            var end = 9999;
            var map = Enumerable.Range(start, end - start + 1)
                .Select(n => (long)Math.Pow(n, 3)).ToArray();

            int i, j;
            var len = map.Length;
            var req = 5;
            var ret = 0;
            for (i = 0; i < len; i++)
            {
                var count = 1;
                for (j = i + 1; j < len; j++)
                {
                    ret = isPerm(map[i], map[j]);
                    if (ret == 1 || count > req) // since the cube after that will all have more digits, no need to continue
                        break;
                    if (ret == 0)
                        count++;
                }
                if (count == req)
                    return map[i];
            }
            return 0;
        }

        /// <summary>
        /// returns 1 if the candidate is longer than original
        /// returns 0 if it is permutation of original
        /// returns -1 if it is not a permutation
        /// </summary>
        static int isPerm(long original, long candidate)
        {
            var map = new int[10];
            while (original > 0)
            {
                map[original % 10]++;
                original /= 10;
                map[candidate % 10]--;
                candidate /= 10;
            }

            if (candidate > 0)
                return 1;

            if (map.All(n => n == 0) == true)
                return 0;
            else
                return -1;
        }

        /// <summary>
        /// basically extract all the digit from original, sort the digits
        /// and then create a key number from reverse digits from the list(to avoid leading zeros)
        /// and all the number with same key are permutation of each other
        /// </summary>
        static long calculate_key(long original)
        {
            var list = new List<long>();
            long rem = 0;
            while (original > 0)
            {
                original = Math.DivRem(original, 10, out rem);
                list.Add(rem);
            }
            list.Sort();
            long key = 0;
            for (int i = list.Count - 1; i >= 0; i--)
            {
                key *= 10;
                key += list[i];
            }
            return key;
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
