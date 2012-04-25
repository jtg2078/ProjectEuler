using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler113csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The count of numbers below a googol (10100) that are not bouncy:");

            TestBenchSetup();
            TestBenchLoader(using_binomial_coefficients_formula_to_calculate_ways_of_forming_numbers);

            Console.WriteLine("press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// since from previous question, we know that non-bouncing number is alot more rare than 
        /// bouncing number, so by counting ways of creating bouncing and non-bouncing is the way to go
        /// given the question's criteria of 10^100 range.
        /// for increasing number, there are at most 8 possible times to increase the remaining digits in
        /// the number (e.g 12345678999999...., cant go any higher than 9, so eight times if starts with 1)
        /// for decreasing number, there are at most 9 possible times to decrease the remaining digits in
        /// the number (e.g 98765432100000...., cant go any lower than 0, so nine times if starts with 9)
        /// so ways of creating non-bouncing number is to calculate ways of making them at each digit length
        /// by using binomial coefficients formula(nCr)
        /// most of the code for calcuating nCr were reused from problem 53 :)
        /// </summary>
        static long using_binomial_coefficients_formula_to_calculate_ways_of_forming_numbers()
        {
            var count = 0L;
            var max = 110;
            var map = prime_factors_map(max);
            var factors = new int[max];
            var n = 0;
            for (int r = 1; r <= 100; r++)
            {
                n = 8 + r; // for inc
                count += nCr(n, r, map, factors, max);
                n = 9 + r; // for dec
                count += nCr(n, r, map, factors, max);
                count -= 10; // remove duplicated
            }
            return count;
        }

        /// <summary>
        /// n)C_(r) = 	
        ///    n!
        /// --------   (binomial coefficients)
        /// r!(n−r)!
        /// </summary>
        static long nCr(int n,int r, List<List<int>> map, int[] factors, int max)
        {
            var start = n;
            var end = n - r > r ? n - r + 1 : r + 1; //the greater of r or (n-r) to be used to cancel out from n! so n*(n-1)*(n-2)....end
            var div = n - r > r ? r : n - r; //the lesser of r or (n-r), and it is the divisor

            var i = 0;
            var sum = 1L;
            for (i = 2; i < max; i++) //reset
            {
                factors[i] = 0;
            }
            for (i = end; i <= start; i++) //add all the prime factors from dividend
            {
                foreach (var factor in map[i])
                {
                    factors[factor]++;
                }
            }
            for (i = 2; i <= div; i++) // substract all the prime factors from dividend
            {
                foreach (var factor in map[i])
                {
                    factors[factor]--;
                }
            }
            for (i = 2; i < max; i++) // multiply all the remaining prime factors
            {
                while (factors[i] > 0)
                {
                    sum *= i;
                    factors[i]--;
                }
            }
            return sum;
        }

        /// <summary>
        /// generate all the prime factors for number from 0 to len
        /// </summary>
        static List<List<int>> prime_factors_map(int len)
        {
            var map = new List<List<int>>();
            map.Add(new List<int>() { 0 });
            map.Add(new List<int>() { 1 });
            var primes = new List<int>() { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43,
                47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97, 101, 103, 107, 109 };
            var num = 0;
            for (int i = 2; i < len; i++)
            {
                var list = new List<int>();
                num = i;
                foreach (var prime in primes)
                {
                    while (num % prime == 0)
                    {
                        list.Add(prime);
                        num /= prime;
                    }
                    if (num == 1)
                        break;
                }
                map.Add(list);
            }
            return map;
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
