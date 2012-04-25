using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler051csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The smallest prime that is part of an eight prime value family");

            TestBenchSetup();
            TestBenchLoader(test_each_6_digits_prime_to_find_first_match);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// base on the question, 5-d number with 2-replacement part produces 7 primes
        /// so its worth to try 6-d number with 3-replacement part to see if it produces 8 primes
        /// i guess it does!
        /// 1. generate a seive prime map up to 1 mil
        /// 2. for each 6-digit prime number, see if it has any repeating number of 0,1,2
        ///    (the reason for this is because we need at least 8 produced primes, so the number has to start with
        ///     at least one of 1, 2 or 3)
        /// 3. if 2. holds true, it would fill array (pos) with the index of repeating numbers,
        ///    replacing each number with 1~10 and see if the count goes to 8
        /// 4. return the 1st answer
        /// </summary>
        /// <returns></returns>
        static int test_each_6_digits_prime_to_find_first_match()
        {
            var bound = 1000000;
            var primes = GetPrimes(bound);
            var len = 6;
            var target = 8;
            var master = new int[len];
            var worker = new int[len];
            var pos = new int[len];
            for (int i = 100001; i < bound; i += 2)
            {
                if (primes[i] == false)
                {
                    num_to_int_array(i, master, len);
                    if (look_for_same_digits(0, master, len, pos)
                        || look_for_same_digits(1, master, len, pos)
                        || look_for_same_digits(2, master, len, pos))
                    {
                        copy_int_arrays(master, worker, len);
                        if (test_all_digits(worker, primes, len, pos, target))
                            return i;
                    }
                }
            }
            return 0;
        }

        static bool test_all_digits(int[] worker, bool[] primes, int len, int[] pos, int target)
        {
            var count1 = 0;
            var count2 = 0;
            int num, i, j;
            var limit = 10 - target;
            for (i = 0; i < 10; i++)
            {
                for (j = 0; j < 3; j++)
                {
                    worker[pos[j]] = i;
                }
                if (worker[0] == 0)
                    continue;

                num = int_array_to_num(worker, len);

                if (primes[num] == false)
                    count1++;
                else
                    count2++;

                if (count2 > limit)
                    break;
            }
            if (count1 == target)
                return true;
            return false;
        }

        static bool look_for_same_digits(int d, int[] array, int len, int[] pos)
        {
            var pos_count = 0;
            while (len != 0)
            {
                if (array[--len] == d)
                    pos[pos_count++] = len;
            }
            if (pos_count == 3)
                return true;
            else
                return false;
        }

        static void num_to_int_array(int n, int[] array, int len)
        {
            while (n != 0)
            {
                n = Math.DivRem(n, 10, out array[--len]);
            }
        }

        static int int_array_to_num(int[] array, int len)
        {
            var tens = 1;
            var n = 0;
            while (len != 0)
            {
                n += (array[--len] * tens);
                tens *= 10;
            }
            return n;
        }

        static void copy_int_arrays(int[] master, int[] clone, int len)
        {
            while (len != 0)
            {
                clone[--len] = master[len];
            }
        }

        static bool[] GetPrimes(int max)
        {
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
            return primes;
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
