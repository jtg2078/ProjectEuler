using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler047csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The first four consecutive integers to have four distinct primes factors:");

            TestBenchSetup();
            TestBenchLoader(brute_force_test_each_number_for_match);
            TestBenchLoader(using_seive_to_calculate_prime_factor_count);
            TestBenchLoader(using_seive_to_calculate_prime_factor_count_with_unique_check);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// first construct a prime list up to 1k(using seive)
        /// and check each number after 647 to see if it matches with criteria given by the question
        /// 1. for each number, first find out all the prime factors, and add them to a hashset
        /// 2. if prime factors added are not exactly 4, reset the hashset
        /// 3  (start variable is used to make sure the numbers are continuous, if there is a gap between numbers, reset the hashset)
        /// 4. if prime factors added are exactly 4, check the hashset count, if it is 16 return the answer
        /// 5. if it is not 16, continues to next number
        /// </summary>
        /// <returns></returns>
        static int brute_force_test_each_number_for_match()
        {
            var max = 1000; //randomly chose, it works :P
            var bound = (int)Math.Sqrt(max);
            bool[] primes = new bool[max];
            primes[0] = true;
            primes[1] = true;
            int s, m;
            for (s = 2; s <= bound; s++)
            {
                if (primes[s] == false)
                {
                    for (m = s * s; m < max; m += s)
                    {
                        primes[m] = true;
                    }
                }
            }
            var factor = 4;
            var start = 0;
            var num = 0;
            var pwr = 1;
            var set = new HashSet<int>();
            var count = 1;
            for (s = 647; s < 200000; s++)
            {
                num = s;
                for (m = 2; m < max; m++)
                {
                    if (primes[m] == false)
                    {
                        pwr = 1;
                        while (num % m == 0)
                        {
                            pwr *= m;
                            num /= m;
                        }
                        if (pwr != 1)
                            set.Add(pwr);
                        if (num <= 1)
                            break;
                    }
                }
                if (set.Count == factor * count && (s == start + 1 || start == 0))
                {
                    if (count == factor)
                        return s - 3;

                    start = s;
                    count++;
                }
                else
                {
                    set.Clear();
                    count = 1;
                    start = 0;
                }
            }
            return 0;
        }

        /// <summary>
        /// basically using seive mechanism to find out each number's prime factor count, and return the first
        /// 4 sequential numbers that have exactly 4 primes factors.
        /// but this method doesnt check for uniqueness of each prime factors.
        /// still, it returns the correct answer(the answer happens to be the same as the first 4 sequential number
        /// with 4 prime factors)
        /// </summary>
        /// <returns></returns>
        static int using_seive_to_calculate_prime_factor_count()
        {
            var max = 1000;
            var bound = (int)Math.Sqrt(max);
            bool[] primes = new bool[max];
            primes[0] = true;
            primes[1] = true;
            int s, m;
            for (s = 2; s <= bound; s++) //construct a prime map up to 1k
            {
                if (primes[s] == false)
                {
                    for (m = s * s; m < max; m += s)
                    {
                        primes[m] = true;
                    }
                }
            }
            var limit = 200000; //tried 10k and 100k, nothing came up
            var map = new int[limit];
            for (s = 2; s < max; s++) //using seive to calculate each number's prime factor count
            {
                if (primes[s] == false)
                {
                    for (m = s * 2; m < limit; m += s)
                    {
                        map[m]++;
                    }
                }
            }
            for (s = 2; s < limit; s++) //find the first match and return the result
            {
                if (map[s + 3] == 4 && map[s + 2] == 4 && map[s + 1] == 4 && map[s] == 4)
                    return s;
            }
            return 0;
        }

        /// <summary>
        /// basically same version as previous method, but check for unique prime factors
        /// </summary>
        /// <returns></returns>
        static int using_seive_to_calculate_prime_factor_count_with_unique_check()
        {
            var max = 1000;
            var bound = (int)Math.Sqrt(max);
            bool[] primes = new bool[max];
            primes[0] = true;
            primes[1] = true;
            int s, m;
            for (s = 2; s <= bound; s++)
            {
                if (primes[s] == false)
                {
                    for (m = s * s; m < max; m += s)
                    {
                        primes[m] = true;
                    }
                }
            }
            var limit = 200000;
            List<int>[] map = new List<int>[limit];
            int num, factor;
            for (s = 2; s < max; s++)
            {
                if (primes[s] == false)
                {
                    for (m = s * 2; m < limit; m += s)
                    {
                        factor = 1;
                        num = m;
                        while (num % s == 0)
                        {
                            factor *= s;
                            num /= s;
                        }

                        if (map[m] == null)
                            map[m] = new List<int>();
                        map[m].Add(factor);
                    }
                }
            }
            var set = new HashSet<int>();
            for (s = 2; s < limit; s++)
            {
                if (map[s + 3] != null && map[s + 3].Count == 4 & map[s + 2] != null && map[s + 2].Count == 4
                    && map[s + 1] != null && map[s + 1].Count == 4 && map[s] != null && map[s].Count == 4)
                {
                    set.Clear();
                    map.Skip(s)
                        .Take(4)
                        .SelectMany(l => l) //flatten the nested array
                        .ToList()
                        .ForEach(l => set.Add(l));
                    if (set.Count == 16)
                        return s;
                }
            }
            return 0;
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
