using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler049csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The arithmetic sequences, made of prime terms, whose four digits are permutations of each other");

            TestBenchSetup();
            TestBenchLoader(generate_and_test_each_arithmetic_sequence_for_each_prime_number);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// basically for each prime n within 1k and 10k, generate and test all possible 
        /// arithmetic sequence and see if they are all primes and are permutations of n
        /// return the first finding!
        /// </summary>
        /// <returns></returns>
        static long generate_and_test_each_arithmetic_sequence_for_each_prime_number()
        {
            var max = 10000;
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
            int n1, n2, n3;
            for (n1 = 1001; n1 < max; n1 += 2)
            {
                if (primes[n1] == false && n1 != 1487)
                {
                    bound = (max + n1) / 2;
                    for (n2 = n1 + 1000; n2 < bound; n2 += 2)
                    {
                        if (primes[n2] == false)
                        {
                            n3 = n2 + (n2 - n1);
                            if (n3 < max && primes[n3] == false && isPerm(n1, n2) && isPerm(n2, n3))
                                return (long)n1 * 100000000 + n2 * 10000 + n3;
                        }
                    }
                }
            }
            return 0;
        }

        static bool isPerm(int original, int candidate)
        {
            var map = new int[11];
            while (original > 0)
            {
                map[original % 10]++;
                original /= 10;
                map[candidate % 10]--;
                candidate /= 10;
            }
            return !map.Any(n => n != 0);
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
