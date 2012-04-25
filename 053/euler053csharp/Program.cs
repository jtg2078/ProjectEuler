using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler053csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The values of  ^(n)C_(r), for 1 ≤ n  ≤ 100, are greater than one-million:");

            TestBenchSetup();
            TestBenchLoader(turn_every_number_into_prime_factors_for_easy_calculation);
            TestBenchLoader(using_pascal_triangle_to_calculate_binomial_coefficients);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// test all the possible combination of n and r, then calling the factorial method to see if 
        /// the sum is greater than 1mil.
        /// n)C_(r) = 	
        ///    n!
        /// --------   (binomial coefficients)
        /// r!(n−r)!
        /// so end = the greater of r or (n-r) to be used to cancel out from n! so n*(n-1)*(n-2)....end
        /// f = the lesser of r or (n-r), and it is the divisor
        /// </summary>
        /// <returns></returns>
        static int turn_every_number_into_prime_factors_for_easy_calculation()
        {
            int n, r;
            var count = 0;
            var end = 0;
            var f = 0;
            var map = prime_factors_map();
            var max = 101;
            var factors = new int[max];
            for (n = 23; n <= 100; n++)
            {
                for (r = 1; r <= n; r++)
                {
                    end = n - r > r ? n - r : r;
                    f = n - r > r ? r : n - r;
                    if (factorial(n, end + 1, f, map, factors, max))
                        count++;
                }
            }
            return count;
        }

        /// <summary>
        /// turns out pascal triangle is awesome at solving this problem(from q's post)
        /// see detail: http://mathforum.org/dr/math/faq/faq.pascal.triangle.html
        /// </summary>
        static int using_pascal_triangle_to_calculate_binomial_coefficients()
        {
            var bound = 101;
            var pascal = new int[bound][];
            int i, col, row, value;
            var limit = 1000000;
            var count = 0;
            // 1. populate edges
            for (i = 0; i < bound; i++)
            {
                pascal[i] = new int[i + 1];
                pascal[i][0] = 1;
                pascal[i][i] = 1;
            }
            // 2. starting at 3rd row(1,x,1), and populate x value from x upper left and upper neighbor
            //    and if x value is greater than 1mil, add 1 to count. the max value is kept at 1 mil + 1
            for (row = 2; row < bound; row++)
            {
                for (col = 1; col < row; col++)
                {
                    value = pascal[row - 1][col - 1] + pascal[row - 1][col];
                    pascal[row][col] = value > limit ? limit + 1 : value;
                    if (value > limit)
                        count++;
                }
            }
            return count;
        }

        /// <summary>
        /// basically turning every number into prime factors, and then cancel out all the divisor's factors from dividend's
        /// factors, then calculate product of all the remaining factors and return false if it is less than 1 mil, true
        /// otherwise
        /// </summary>
        static bool factorial(int start, int end, int div, List<List<int>> map, int[] factors, int max)
        {
            var i = 0;
            var sum = 1;
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
                    if (sum > 1000000)
                        return true;
                }
            }
            return sum > 1000000;
        }

        /// <summary>
        /// generate all the prime factors for number from 0 to 100
        /// </summary>
        static List<List<int>> prime_factors_map()
        {
            var len = 101;
            var map = new List<List<int>>();
            map.Add(new List<int>() { 0 });
            map.Add(new List<int>() { 1 });
            var primes = generate_primes(100);
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

        /// <summary>
        /// generate all the primes from 1 to max
        /// </summary>
        static List<int> generate_primes(int max)
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
            var result = new List<int>();
            for (s = 2; s < max; s++)
            {
                if (primes[s] == false)
                    result.Add(s);
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
