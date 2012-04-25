using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler037csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The sum of the only eleven primes that are both truncatable from left to right and right to left:");

            TestBenchSetup();
            TestBenchLoader(generate_all_right_truncatable_primes_and_check_each_for_left_truncatable);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// according to http://en.wikipedia.org/wiki/Truncatable_prime, there are only 83 right-truncatable primes
        /// so this method first generate all the right-truncatable primes and check each primes is also left-
        /// truncatable primes(using isLeftPrime(int n) method)
        /// the algorithm that generates right-truncatable primes:
        ///     1. start with seed list (2,3,5,7), set the range(start=0, end=seed list.count)
        ///     2. for each number in the seed list from range(start to end), append a single digit number(1~9) to the end, 
        ///        add new number to the seed list if the new number is also prime.
        ///        for example, 2-> 21,22,23,...,29: add 23,39 to the seed list.
        ///     3. after step 2, all the new numbers generated based on the numbers in the seed list are added to the
        ///        seed list. update the start index and end index. the reason for this is because we dont need
        ///        to go through any number except the ones that are just been generated. so basically we only want to
        ///        generate new number based on the numbers in the seed list that havent been used to generate new numbers
        ///     4. go to step 2
        ///     5. if after the "new prime-number generation phase" and no new primes number are found. the that mean we 
        ///        reach the end.
        ///     6. the main reason for this algorithm is that. take the largest right-truncatable prime
        ///        73939133, each successive sub number has to be prime. e.g 7,72,739,7393,73939,739391,739391(3)
        ///        so base on a tested right-truncatable prime, any new prime number generated from it are also inherently
        ///        right-truncatable
        /// </summary>
        /// <returns></returns>
        static int generate_all_right_truncatable_primes_and_check_each_for_left_truncatable()
        {
            var seeds = new List<int>() { 2, 3, 5, 7 };
            int right, index;
            var start = 0;
            var end = 0;
            while (true)
            {
                end = seeds.Count;
                if (start == end)
                    break;
                for (index = start; index < end; index++)
                {
                    for (int i = 1; i < 10; i += 2) // eliminate even number
                    {
                        if (i == 5)
                            continue;
                        right = seeds[index] * 10 + i;
                        if (isPrime(right))
                            seeds.Add(right);
                    }
                }
                start = end;
            }
            var result = 0;
            foreach (var num in seeds)
            {
                if (num > 10 && isLeftPrime(num))
                    result += num;
            }
            return result;
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

        static bool isLeftPrime(int n)
        {
            var tens = n < 10 ? 10 : (int)Math.Pow(10, (int)Math.Log10(n));
            var rem = n;
            while (tens >=10)
            {
                rem = rem % tens; //doesnt need to check n, since n is already a prime in this context
                if (isPrime(rem) == false)
                    return false;
                tens /= 10;
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
