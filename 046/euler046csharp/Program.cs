using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler046csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The smallest odd composite that cannot be written:");

            TestBenchSetup();
            TestBenchLoader(brute_force_with_constructed_prime_sieve);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// basically brute force to test each odd number thats not prime under randomly
        /// chose limit(10k). First construct a prime sieve from 1 to 10k,
        /// and for each non-prime odd number, test if the conjecture holds by iterate 
        /// all the prime below it, calculate the difference, and see if a square exist
        /// return the first number that no square exist for all the primes blow it.
        /// </summary>
        /// <returns></returns>
        static int brute_force_with_constructed_prime_sieve()
        {
            var max = 10000; //randomly chose, it works :P
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
            var found = false;
            double test_sqrt = 0;
            for (s = 3; s < max; s += 2)
            {
                if (primes[s] == true)
                {
                    found = false;
                    for (m = s - 2; m > 3; m -= 2) //reverse == alot faster, think about it!
                    {
                        if (primes[m] == false)
                        {
                            test_sqrt = Math.Sqrt((s - m) / 2);
                            if (test_sqrt == (int)test_sqrt)
                            {
                                found = true;
                                break; //no need to test any further
                            }
                        }
                    }
                    if (found == false)
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
