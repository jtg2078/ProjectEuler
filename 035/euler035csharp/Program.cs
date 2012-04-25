using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler035csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The number of circular primes below one million:");

            TestBenchSetup();
            TestBenchLoader(using_sieve_to_construct_primes_and_test_each_prime_candidates);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// basically using Sieve of Eratosthenes(from Q7) to construct a prime list under 1mil
        /// and for each prime number, test each circular number, using simple equation to rotate nubmers
        /// num = (num % d[size]) * 10 + num / d[size]
        /// </summary>
        static int using_sieve_to_construct_primes_and_test_each_prime_candidates()
        {
            var max = 1000000;
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

            var d = new int[] { 0, 1, 10, 100, 1000, 10000, 100000, 1000000 };
            var len = d.Length;
            int size, num;
            var pass = false;
            var count = 0;

            for (m = 0; m < max; m++)
            {
                if (primes[m] == false)
                {
                    size = (int)Math.Log10(m) + 1;
                    num = m;
                    pass = true;
                    for (s = 0; s < size; s++)
                    {
                        num = (num % d[size]) * 10 + num / d[size];
                        if (primes[num] == true)
                        {
                            pass = false;
                            break;
                        }
                    }
                    if (pass)
                        count++;
                }
            }
            return count;
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
