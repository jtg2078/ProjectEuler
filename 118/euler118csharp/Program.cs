using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler118csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The number of distinct sets: ");

            TestBenchSetup();
            TestBenchLoader(brute_force_generate_all_permutations_and_partition_each_to_test);

            Console.WriteLine("press any key to exit");
            Console.ReadKey();
        }

        static int brute_force_generate_all_permutations_and_partition_each_to_test()
        {
            var nums = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var j = 0;
            var l = 0;
            var tmp = 0;
            var len = nums.Length;
            var z = 0;
            var distinct = new HashSet<int>();
            var prime_keeper = new Dictionary<int, bool>();
            while (true)
            {
                //step 1. Find the largest index j such that a[j] < a[j + 1].
                for (j = len - 2; j >= 0; j--)
                {
                    if (nums[j] < nums[j + 1])
                        break;
                }

                if (j == -1)
                    break; // no more permutation, since no such index exist

                //step 2. Find the largest index l such that a[j] < a[l]
                for (l = len - 1; l >= 0; l--)
                {
                    if (nums[j] < nums[l])
                        break;
                }

                //step 3. Swap a[j] with a[l].
                tmp = nums[j];
                nums[j] = nums[l];
                nums[l] = tmp;

                //step 4. Reverse the sequence from a[j + 1] up to and including the final element a[n]
                z = len - 1;
                for (l = j + 1; l < len; l++)
                {
                    if (l >= z)
                        break;

                    tmp = nums[l];
                    nums[l] = nums[z];
                    nums[z] = tmp;
                    z--;
                }

                make_set_and_check(nums, 0, 1, 9, distinct, prime_keeper);
            }
            return distinct.Count;
        }

        /// <summary>
        /// this is the recursive method that paritions the number sequence and test/store each
        /// partitions if it satisfies the rules stated by the question.
        /// this method parition one slice at a time
        /// </summary>
        /// <param name="numbers">the digit sequence to be paritioned</param>
        /// <param name="start">the start index of remaining digit sequence to be partition</param>
        /// <param name="partitions_product">this number is used to be keep track of unique partition
        ///                                  by using the property of product of primes are unique</param>
        /// <param name="remain_len">the length of remaining digit sequence to be partition</param>
        /// <param name="distinct">used to store partitions_product as a way to count distinct partition</param>
        /// <param name="prime_cache">used to store prime test result of a number, since most numbers appear in
        ///                           this problem are highly repetitive, this is very useful</param>
        static void make_set_and_check(int[] numbers, int start, int partitions_product,
            int remain_len, HashSet<int> distinct, Dictionary<int, bool> prime_cache)
        {
            if (remain_len == 0)
                distinct.Add(partitions_product);

            for (int length = 1; length <= remain_len; length++)
            {
                // initial filter to filter out non-prime
                var last = numbers[start + length - 1];
                if (length > 1 && (last == 2 || last == 4 || last == 5 || last == 6 || last == 8))
                    continue;

                // turned the array digit into number
                var number = toNumber(numbers, start, length);

                // check if the number is primes and caching the result
                var isPrime = prime_cache.ContainsKey(number) ?
                    prime_cache[number] : (prime_cache[number] = Miller_Rabin_primality_test(number));

                // only continue further if everything is prime so far
                if (isPrime == true)
                    make_set_and_check(numbers,
                        start + length, partitions_product * number, remain_len - length, distinct, prime_cache);
            }
        }

        static int toNumber(int[] a, int start, int len)
        {
            var num = a[start];
            for (int i = 1; i < len; i++)
            {
                num *= 10;
                num += a[start + i];
            }
            return num;
        }

        static bool Miller_Rabin_primality_test(long n)
        {
            if (n < 2)
                return false;
            else if (n == 2 || n == 3 || n == 5 || n == 7 || n == 61)
                return true;
            else if (n % 2 == 0 || n % 3 == 0 || n % 5 == 0)
                return false;

            long s = 0;
            long d = n - 1;
            while (d % 2 == 0)
            {
                d /= 2;
                s++;
            }
            //  if n < 4,759,123,141, it is enough to test a = 2, 7, and 61;
            var a_list = new int[] { 2, 7, 61 };
            long x = 0;
            foreach (var a in a_list)
            {
                x = exponentiation_by_squaring_with_modulo(a, d, n);
                if (x == 1 || x == n - 1)
                {
                    continue;
                }
                for (long r = 1; r < s; r++)
                {
                    x = exponentiation_by_squaring_with_modulo(x, 2, n);
                    if (x == 1)
                        return false;
                    if (x == n - 1)
                        break;
                }
                if (x != n - 1)
                    return false;
            }

            return true;
        }

        static long exponentiation_by_squaring_with_modulo(decimal x, long n, long mask)
        {
            decimal r = 1;
            while (n > 0)
            {
                if ((n & 1) == 1) // using bitwise for checking odd
                {
                    r = (r * x) % mask;
                    n--;
                }
                x = (x * x) % mask;
                n >>= 1; // using bitshift for halving
            }
            return (long)r;
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

