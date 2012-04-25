using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler050csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The prime, below one-million, can be written as the sum of the most consecutive primes:");

            TestBenchSetup();
            TestBenchLoader(calculate_length_of_each_sequence_with_starting_point_beginning_from_first_prime);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// basically brute force and test each sequence starting from 2,3,5,6... to max/536
        /// and find the longest sequence
        /// </summary>
        /// <returns></returns>
        static int calculate_length_of_each_sequence_with_starting_point_beginning_from_first_prime()
        {
            var max = 1000000;
            var primes = primeList(max);
            var bound = max / 536; //if we start with 2, then the sequence count is 536, so the answer must be higher than that
            var i = 1;
            var longest = 0;
            int sum, index, length, counter;
            var result = 0;
            var prime = 0;
            var weight = 1;
            while ((i += weight) < bound)
            {
                if (i == 3)
                    weight++;
                if (primes[i] == false)
                {
                    sum = 0;
                    index = i;
                    length = 0;
                    counter = 0;
                    while ((sum += index) < max)
                    {
                        counter++;
                        if (primes[sum] == false)
                        {
                            length = counter;
                            prime = sum;
                        }
                        do
                        {
                            index += weight;
                        } while (index < max && primes[index]);
                    }
                    if (length > longest)
                    {
                        longest = length;
                        result = prime;
                    }
                }
            }
            return result;
        }

        static bool[] primeList(int max)
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
            return primes;
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
