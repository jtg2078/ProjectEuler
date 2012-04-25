using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler097csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The last ten digits of this prime number: ");

            TestBenchSetup();
            TestBenchLoader(plain_squaring_by_two_and_using_mask);
            TestBenchLoader(exponentiation_by_squaring_and_using_mask);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        static long plain_squaring_by_two_and_using_mask()
        {
            long num = 2;
            long mask = 10000000000L;
            for (int p = 1; p < 7830457; p++)
            {
                num *= 2;
                num %= mask;
            }
            return (num * 28433L + 1) % mask;
        }

        /// <summary>
        /// simiar to problem 56
        /// using the exponentiation by squaring to compute the powers. vastly efficient compare with 1st method.
        /// because of the precision of last 10 digits is needed by the question, so not even long is enough to hold
        /// the number of digits required by n = n * n, which both n has to be 10 digits long, so array-based way of storing
        /// the number is used instead. still super fast. :)
        /// </summary>
        static long exponentiation_by_squaring_and_using_mask()
        {
            var power = 7830457;
            var r = new int[] { 1 };
            var n = new int[] { 2 };
            var mask = 10;
            while (power > 0)
            {
                if ((power & 1) == 1)
                    r = multiply_and_mask(r, n, mask);

                power >>= 1;
                n = multiply_and_mask(n, n, mask);
            }
            r = multiply_and_mask(r, num_to_array(28433), mask);
            r = addition(r, new int[] { 1 });
            long result = r[r.Length - 1];
            for (int i = r.Length - 2; i >= 0; i--)
            {
                result = result * 10 + r[i];
            }
            return result;
        }

        /// <summary>
        /// Schönhage–Strassen algorithm
        /// http://en.wikipedia.org/wiki/Sch%C3%B6nhage%E2%80%93Strassen_algorithm
        /// </summary>
        static int[] multiply_and_mask(int[] a, int[] b, int max_digit)
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

            return result.Take(Math.Min(result.Count, max_digit)).ToArray<int>();
        }

        static int[] addition(int[] a, int[] b)
        {
            var len = Math.Max(a.Length, b.Length);
            var result = new List<int>(len);
            int carry = 0;
            int x, y, sum;
            for (int i = 0; i < len; i++)
            {
                x = i < a.Length ? a[i] : 0;
                y = i < b.Length ? b[i] : 0;

                sum = x + y + carry;
                result.Add(sum % 10);
                carry = sum / 10;
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
