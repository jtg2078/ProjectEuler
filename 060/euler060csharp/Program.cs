using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace euler060csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The sum of the ASCII values in the original text: ");

            TestBenchSetup();
            TestBenchLoader(test3);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        static readonly int max = 100000000;
        static readonly bool[] master_primes = generate_primes(max);
        static int test3()
        {
            var primes = master_primes;
            var size = (int)Math.Sqrt(max);
            var matrix = new int[size, size];
            int row, col;
            for (row = 2; row < size; row++)
            {
                for (col = 2; col < size; col++)
                {
                    if (primes[row] == false && primes[col] == false && matrix[row, col] == 0)
                    {
                        if (concat_both_way(row, col, primes))
                        {
                            matrix[row, col] = 1;
                            matrix[col, row] = 1;
                        }
                        else
                        {
                            matrix[row, col] = -1;
                            matrix[col, row] = -1;
                        }
                    }
                }
            }
            var count = 5;
            var accum = new int[count];
            for (row = 3; row < size; row++)
            {
                for (col = 3; col < size; col++)
                {
                    if (matrix[row, col] == 1)
                    {
                        accum[5 - count] = row;
                        if (probe(col, matrix, size, primes, count - 1, accum))
                            return accum.Sum();
                    }
                }
            }
            return 0;
        }

        static bool probe(int row, int[,] matrix, int size, bool[] primes, int count, int[] accum)
        {
            for (int col = 3; col < size; col++)
            {
                if (matrix[row, col] == 1 && isGood(accum, primes, accum.Length - count, col))
                {
                    accum[accum.Length - count] = col;
                    if (count == 1)
                        return true;

                    return probe(col, matrix, size, primes, count - 1, accum);
                }
            }
            return false;
        }

        static bool isGood(int[] accum, bool[] primes, int count, int num)
        {
            for (int i = 0; i < count; i++)
            {
                if (concat_both_way(accum[i], num, primes) == false)
                    return false;
            }
            return true;
        }

        static bool concat_both_way(int n1, int n2, bool[] primes)
        {
            var num = 0;

            // tail
            num = n1 * (int)Math.Pow(10, (int)Math.Log10(n2) + 1) + n2;
            if (primes[num] == true)
                return false;

            // head
            num = n2 * (int)Math.Pow(10, (int)Math.Log10(n1) + 1) + n1;
            if (primes[num] == true)
                return false;

            return true;
        }

        static bool[] generate_primes(int max)
        {
            var bound = (int)Math.Sqrt(max);
            var primes = new bool[max];
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