using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler072csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The elements contained in the set of reduced proper fractions for d ≤ 1,000,000: ");

            TestBenchSetup();
            TestBenchLoader(using_totient_function_to_find_total_RF_for_each_number_as_denominator);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// since the number of reduced fraction for a given denominator is basically the Totient function of that number.
        /// (φ(4)=2, relative prime is 1, 3: reduced fraction with 4 as denominator: 1/4, 3/4, so total is 2 as well)
        /// so the code used in problem 69 and 70 can be reused to calculate Totient function for each base(1,2,3...1mil)
        /// </summary>
        static long using_totient_function_to_find_total_RF_for_each_number_as_denominator()
        {
            var LIMIT = 1000001;
            var bound = LIMIT / 2;
            var primes = new int[LIMIT];
            int s, m, t;
            for (s = 2; s < bound; s++)
            {
                if (primes[s] == 0)
                {
                    for (m = s * 2; m < LIMIT; m += s) // m = s * 2 since no point to test prime number(always 1 and itself)
                    {
                        t = primes[m];

                        if (t == 0)
                            t = m; // so this is 1st time, mark t to m(the number itself)

                        t /= s; // division first to avoid overflow
                        t *= (s - 1);
                        primes[m] = t;
                    }
                }
            }
            long sum = 0;
            for (s = 2; s < LIMIT; s++)
            {
                t = primes[s];
                sum += (t == 0 ? s - 1 : t); // since prime will have zero as value in the array, and basically # of reduced fraction
            }                                // for a prime number is every number blow it. eg: x/5: 1/5, 2/5, 3/5, 4/5 total:(5-1)
            return sum;
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
