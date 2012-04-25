using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler041csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The largest n-digit pandigital prime that exists?");

            TestBenchSetup();
            TestBenchLoader(brute_force_find_largest_prime_in_each_range_s_permutations);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// using code and algorithm from question 24 to generate permutation for each
        /// range of digits, from most to 4(which the question already state 2143 is a prime pandigital num)
        /// saw the rule of divisibility in the question's forum stated that if the sum of digits of a number 
        /// is divisible by 3, then the number is divisble by 3, which makes it a non-prime(cool, never heard of it)
        /// after some research:
        /// http://en.wikipedia.org/wiki/Divisibility_rule
        /// http://www.apronus.com/math/threediv.htm
        /// </summary>
        /// <returns></returns>
        static int brute_force_find_largest_prime_in_each_range_s_permutations()
        {
            var seeds = new int[][]{
                new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
                new int[] { 1, 2, 3, 4, 5, 6, 7, 8},
                new int[] { 1, 2, 3, 4, 5, 6, 7},
                new int[] { 1, 2, 3, 4, 5, 6},
                new int[] { 1, 2, 3, 4, 5},
                new int[] { 1, 2, 3, 4},
            };

            var result = 2143; //given by the question, if no bigger prime is found;
            foreach (var seed in seeds)
            {
                if (seed.Sum() % 3 == 0) //saw this rule from question's forum
                    continue;

                result = find_largest_prime_within_permutations(seed);
                if (result >= 2143)
                    return result;
            }
            return result;
        }

        /// <summary>
        /// basically the same code from question 24, which uses the algorithm from knuth's book
        /// http://en.wikipedia.org/wiki/Permutation#Systematic_generation_of_all_permutations
        /// </summary>
        /// <param name="a">contains seed values for permutation to be generated from</param>
        /// <returns></returns>
        static int find_largest_prime_within_permutations(int[] a)
        {
            var j = 0;
            var l = 0;
            var tmp = 0;
            var len = a.Length;
            var z = 0;
            var candidates = new List<int>();
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
                //reuse j for j=a[len-1]
                j = a[len - 1];
                if (j == 1 || j == 3 || j == 7 || j == 9)
                    candidates.Add(Convert.ToInt32
                        (a.Aggregate(new StringBuilder(len), (s, n) => s.Append(n)).ToString()));
            }
            for (j = candidates.Count - 1; j >= 0; j--)
            {
                if (isPrime(candidates[j]))
                    return candidates[j];
            }
            return 0;
        }

        static bool isPrime(int n)
        {
            if (n <= 1)
                return false;
            var limit = (int)Math.Sqrt(n) + 1;
            for (int i = 2; i < limit; i++)
            {
                if (n % i == 0)
                    return false;
            }
            return true;
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
