using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler027csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The product of the coefficients, a and b:");

            TestBenchSetup();
            TestBenchLoader(loop_all_possible_a_and_b_in_range);
            TestBenchLoader(optimized_with_reduced_range_for_a_and_b);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        static int loop_all_possible_a_and_b_in_range()
        {
            //generate list of primes up(using problem 7's code)
            var primes = PrimeNumberInTerm(116684);

            // variables
            var a = 0;
            var b = 0;
            var n = 0;
            var p = 0;
            var max_count = 0;
            var result_a = 0;
            var result_b = 0;

            for (a = -999; a < 999; a++)
            {
                for (b = -999; b < 999; b++)
                {
                    n = 0;
                    do
                    {
                        p = n * n + a * n + b;
                        n++;
                    } while (primes.Contains(p));

                    if (n > max_count)
                    {
                        max_count = n - 1;
                        result_a = a;
                        result_b = b;
                    }
                }
            }

            return result_a * result_b;
        }

        static HashSet<int> PrimeNumberInTerm(long maxCeiling)
        {
            var upper = maxCeiling;
            var upperl = (int)Math.Sqrt(upper);
            var map = new bool[upper];
            map[0] = true;
            map[1] = true;

            for (int l = 2; l < upperl; l++)
            {
                if (map[l] == false)
                {
                    for (long i = l * l; i < upper; i += l)
                    {
                        map[i] = true;
                    }
                }
            }

            var primes = new HashSet<int>();
            for (int i = 0; i < upper; i++)
            {
                if (map[i] == false)
                    primes.Add(i);
            }

            return primes;
        }

        /// <summary>
        /// few things:
        /// 1. for n = 0, (0)^2 + a * (0) + b has to be prime, so this make b:
        ///     - b has to be prime
        ///     - b has to be positive
        /// 2. for n = 1, (1)^2 + a + b has to be prime, so
        ///     - 1 + a + b has to be prime and >= 2
        ///     - so a + b >= 1
        ///     - since b has to be prime, and result has to be prime
        ///     - (b + 1) is even, so a has to be odd
        /// </summary>
        static int optimized_with_reduced_range_for_a_and_b()
        {
            // using seive to construct a prime map
            // however, deciding the upper has been troublesome
            var upper = 20000; // turns out 20000 works fine
            var upperl = (int)Math.Sqrt(upper);
            var map = new bool[upper];
            map[0] = true;
            map[1] = true;

            for (int l = 2; l < upperl; l++)
            {
                if (map[l] == false)
                {
                    for (long i = l * l; i < upper; i += l)
                    {
                        map[i] = true;
                    }
                }
            }
            
            var a = 0;
            var b = 0;
            var n = 0;
            var p = 0;
            var max_count = 0;
            var result_a = 0;
            var result_b = 0;
            for (b = 2; b < 1000; b++)
            {
                if (map[b] == false)
                {
                    for (a = -b; a < 1000; a += 2)
                    {
                        if (map[a + b + 1] == true)
                            continue;

                        n = 0;
                        do
                        {
                            p = n * n + a * n + b;
                            n++;
                        } while (p >= 2 && p < upper && map[p] == false);

                        if (n > max_count)
                        {
                            max_count = n - 1;
                            result_a = a;
                            result_b = b;
                        }
                    }
                }
            }
            return result_a * result_b;
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
