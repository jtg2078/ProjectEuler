using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler029csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The total number of distinct terms are in the sequence:");

            TestBenchSetup();
            TestBenchLoader(construct_prime_factor_list_and_use_hashset_to_prevent_duplicate);
            TestBenchLoader(using_brain_power_to_find_pattern);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
      
        static int construct_prime_factor_list_and_use_hashset_to_prevent_duplicate()
        {
            var len = 100;
            var primes = new int[] { 2, 3, 5, 7 };
            var sum = 0;
            var distinct_set = new HashSet<string>();
            var prime_factors = new Dictionary<int, int>();
            var count = 0;
            for (int i = 2; i <= len; i++)
            {
                sum = i;
                prime_factors.Clear();
                foreach (var prime in primes)
                {
                    count = 0;
                    while (sum % prime == 0)
                    {
                        sum = sum / prime;
                        ++count;
                    }
                    if (count > 0)
                        prime_factors.Add(prime, count);
                }
                if (prime_factors.Count == 0 || sum != 1)
                    prime_factors.Add(sum, 1);

                for (int power = 2; power <= len; power++)
                {
                    var factorList = new StringBuilder();
                    foreach (var item in prime_factors)
                    {
                        factorList.Append(item.Key);
                        factorList.Append('^');
                        factorList.Append(item.Value * power);
                        factorList.Append(' ');
                    }
                    distinct_set.Add(factorList.ToString());
                }
            }
            return distinct_set.Count;
        }

        /// <summary>
        /// The non-brute-force way explained by jorgbrown(from this q's discussion post, the code is by qbtruk)
        /// 
        /// C'mon guys, this is a pencil-and-paper problem, not a coding problem. 
        /// Euler probably could have done this one in under 5 minutes. While asleep. 
        /// 
        /// The total number of powers is 9801; really, we're just trying to find duplicates. 
        /// Specifically, let's think about, how many duplicates are there when a^b is a dupe of a 
        /// smaller a raised to a larger power b? 
        /// 
        /// Suppose a is a perfect square of the smaller a, but not a square of a square. 
        /// Then we have a duplicate when b is 2, 3, 4... up to 50. That is, 49 duplicates. 
        /// 
        /// Suppose a is a perfect cube of a smaller a. When b is 2 through 33, we have duplicates 
        /// of smaller a raised to the power b*3. When b is 34, 36, 38, 40, 42, 44, 46, 48, 50, 52, 
        /// 54, 56, 58, 60, 62, 64, 66, we have duplicates of a smaller a raised to the power (b/2)*3. 
        /// Total is 32 plus 17, or again, 49 duplicates. 
        /// 
        /// Suppose a is the square of the square of a smaller a. When b is 2 through 49, we have duplicates
        /// of the square root of a raised to the power (b*2). When b is 51, 54, 57, 60, 63, 66, 69, 72, or 75, 
        /// we have dupes of a^(3/4) raised to the power (b*4/3). Total is 49 plus 9, or 58. 
        /// 
        /// Suppose a is the fifth power of a smaller a. We have dupes of fifth root of a raised to the power (b*5), 
        /// which covers b from 2 to 20. Then we have dupes of a^(2/5) raised to the power (b*5/2), 
        /// which covers b of 22, 24, 26, 28, 30, 32, 34, 36, 38, 40. Then we have dupes of a^(3/5) raised to the 
        /// power (b*5/3), which covers b of 21, 27, 33, 39, 42, 45, 48, 51, 54, 57, 60. Last, we have dupes 
        /// of a^(4/5) raised to the power (b*5/4), which covers b of 44, 52, 56, 64, 68, 72, 76, and 80. Total dupes: 48. 
        /// And the last power we have to worry about is 6. We have dupes of the square root of a raised 
        /// to power (b*2), which covers b from 2 to 50. Then we have dupes of the sixth root to the power (b*6/4), 
        /// which covers b of 52, 54, 56, 58, 60, 62, 64, 66. And last we have dupes of the sixth root to the power (b*6/5), 
        /// which covers b of 55, 65, 70, 75, and 80. Total dupes: 62. 
        ///     
        /// Now let's put it all together: 
        /// squares: 4, 9, 25, 36, 49, 100: These 6 squares have 49 dupes each, 6 * 49 = 294 
        /// cubes: 8, 27: These 3 cubes have 49 duplicates each: 2 * 49 = 98 
        /// 4th power: 16, 81. These 2 have 58 dupes each: 2 * 58 = 116 
        /// 5th power: 32. This has 48 dupes. 
        /// 6th power: 64: this has 62 dupes. 
        /// Total # dupes: 618. 9801-618 is 9183. 
        /// </summary>
        /// <returns></returns>
        static int using_brain_power_to_find_pattern()
        {
            int maxBase = 100;
            int maxPower = 100;

            int nonPowerCount = 85;
            int nonPowerDistinct = 99;

            int power2Count = 4;
            int power2Distinct = 50;

            int totalDistinct = nonPowerCount * nonPowerDistinct + power2Count * power2Distinct;

            bool[] pow2found = new bool[601];
            int maxPow2 = 6;

            bool[] pow3found = new bool[401];
            int maxPow3 = 4;

            for (int pow2 = 1; pow2 < maxPow2 + 1; pow2++)
                for (int power = 2; power < maxPower + 1; power++)
                    pow2found[power * pow2] = true;

            for (int pow3 = 1; pow3 < maxPow3 + 1; pow3++)
                for (int power = 2; power < maxPower + 1; power++)
                    pow3found[power * pow3] = true;

            for (int index = 0; index < pow2found.Length; index++)
                if (pow2found[index])
                    totalDistinct++;

            for (int index = 0; index < pow3found.Length; index++)
                if (pow3found[index])
                    totalDistinct++;

            return totalDistinct;
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
