using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Reflection;

namespace euler056csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The the maximum digital sum: ");

            TestBenchSetup();
            TestBenchLoader(brute_force_and_test_each_number_with_limited_range);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// bascially testing each possible combination of number and power,
        /// but skipped alot of loops with range set to 90 for both (see q's post)
        /// </summary>
        static int brute_force_and_test_each_number_with_limited_range()
        {
            var max = 0;
            var sum = 0;
            int a, b;
            for (a = 90; a < 100; a++)
            {
                for (b = 90; b < 100; b++)
                {
                    sum = exponentiation_by_squaring(a, b);
                    if (sum > max)
                        max = sum;
                }
            }
            return max;
        }

        /// <summary>
        /// see http://en.wikipedia.org/wiki/Exponentiation_by_squaring#Computation_by_powers_of_2
        /// the number is stored in array, and using the support method static int[] multiply(int[] a, int[] b)
        /// for multiplication.
        /// the return value is not the actual result, but the sum of all digits of the result
        /// </summary>
        static int exponentiation_by_squaring(int num, int power)
        {
            var r = new int[] { 1 };
            var n = num_to_array(num);
            while (power > 0)
            {
                if ((power & 1) == 1)
                    r = multiply(r, n);

                power >>= 1;
                n = multiply(n, n);
            }
            return r.Sum();
        }

        /// <summary>
        /// Schönhage–Strassen algorithm
        /// http://en.wikipedia.org/wiki/Sch%C3%B6nhage%E2%80%93Strassen_algorithm
        /// </summary>
        static int[] multiply(int[] a, int[] b)
        {
            var result = new List<int>();
            int shift, product, i, j;
            var index = 0;
            var a_len = a.Length;
            var b_len = b.Length;
            for (i = 0; i < a_len; i++)
            {
                shift = index;
                for (j = 0; j < b_len; j++)
                {
                    product = a[i] * b[j];
                    if (shift >= result.Count)
                        result.Add(0);
                    result[shift] += product;
                    shift++;
                }
                index++;
            }

            var carry = 0;
            for (i = 0; i < result.Count; i++)
            {
                result[i] += carry;
                carry = result[i] / 10;
                result[i] = result[i] % 10;
            }
            if (carry > 0)
                result.Add(carry);

            return result.ToArray<int>();
        }

        static int[] num_to_array(int n)
        {
            var result = new List<int>();
            do
            {
                result.Add(n % 10);
            } while ((n /= 10) > 0);

            return result.ToArray();
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
