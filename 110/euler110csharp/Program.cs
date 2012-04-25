using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler110csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The least value of n for which the number of distinct solutions exceeds 4mil: ");

            TestBenchSetup();
            TestBenchLoader(using_formula_and_refined_solution_from_problem_108);

            Console.WriteLine("press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// from problem 108, we know that by using all primes we can find out n that satisfied the given problem
        /// but it may not be the lowest possible number. so in this question, for n with distinct solutions the n would
        /// be (3^15 +1)/2 = 7,174,454 since (3^14 +1)/2 = 2,391,485. (for formula, see: http://oeis.org/A018892)
        /// the primes would be: 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, and their product would be
        /// 614,889,782,588,491,410.
        /// The idea is that we need to keep swaping largest primes(1 or more) with smaller numbers to make 
        /// product(which is n) smaller and yet still producing distinct solutions higher than 4,000,000. 
        /// </summary>
        static long using_formula_and_refined_solution_from_problem_108()
        {
            var primes = new int[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47 };
            long pp = primes.Aggregate(1L, (p, n) => p *= n); // product of all primes
            long smallest = pp; //  current smallest n sets to pp as default
            var to_swap = 1;
            var sofar = 1;
            // this loop is to swap out primes from highest to lowest
            for (int a = primes.Length - 1; a >= 0; a--)
            {
                pp /= primes[a];
                var smaller_found = false;
                // this loops looks for the smallest substitutable number to replace current largest_prime
                for (int b = 2; b < primes[a]; b++)
                {
                    var solutions = prime_factorization(b * sofar, primes, a);
                    if (solutions >= 8000000)
                    {
                        smaller_found = true;
                        to_swap = b;
                        break;
                    }
                }
                if (smaller_found == true)
                {
                    sofar *= to_swap;
                    smallest = pp * sofar;
                }
                else // so swapping this prime didnt yield anything, so no point to continue
                {
                    break;
                }
            }
            return smallest;
        }

        /// <summary>
        /// this method basically calculates prime factorization of n. (http://en.wikipedia.org/wiki/Divisor)
        /// And then save coefficient of each prime number which is later used to calclulate total divisor counts
        /// of number 2*3*5....(til num_primes) * prime factorization of n. For detail, see problem 108
        /// </summary>
        /// <param name="n">the number to be factorized</param>
        /// <param name="primes">list of primes</param>
        /// <param name="num_primes">upper bound of largest primes</param>
        /// <returns>divisor count</returns>
        static int prime_factorization(int n, int[] primes, int num_primes)
        {
            var list = new List<int>();
            foreach (var p in primes)
            {
                var local = 0;
                while (n % p == 0)
                {
                    local++;
                    n /= p;
                }
                list.Add(local);
            }
            var sofar = 1;
            for (int i = 0; i < num_primes; i++)
            {
                sofar = sofar * ((1 + list[i]) * 2 + 1);
            }
            return sofar;
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
