using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler065csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The sum of digits in the numerator of the 100^(th) convergent of the continued fraction for e: ");

            TestBenchSetup();
            TestBenchLoader(using_convergents_and_formula_to_calculate_numerator);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// base on the following links:
        /// http://en.wikipedia.org/wiki/Simple_continued_fraction
        /// http://mathworld.wolfram.com/ContinuedFraction.html
        /// by calculating the [a0:a1,a2,a3,a4......] the numerator and denominator can be calculated with the following
        /// forumla: numerator: pn = an*pn_1 + pn_2
        ///          denominator: qn = an*qn_1 + qn_2 (for this problem, just the numerator is needed)
        /// the key here is to generate an: since the problem gives some initial values of an
        /// e = [2; 1,(2),1,1,(4),1,1,(6),1,1,(8) ... , 1,2k,1, ...]. there is a pattern:, every 3rd term is incremented by 2 of
        /// the previous 3rd term(except for the first 2 term, a1 and a2), while the rest is always 1
        /// since numerator would get too big, so the int array is used.
        /// runs less than 1k ticks on my machine, the compiler or the .net runtime must be doing some voodoo optimizations.
        /// </summary>
        static int using_convergents_and_formula_to_calculate_numerator()
        {
            var every_third = 0;
            var a = 2;
            var p_2 = new int[] { 0 }; // p(n-1)
            var p_1 = new int[] { 1 }; // p(n-2)
            var p = addition(multiply(num_to_array(a), p_1), p_2);

            var counter = -1;
            while (counter++ < 98) // since counter starts at -1(for the ++ thing), and starts at a1
            {
                if ((counter + 2) % 3 == 0)
                {
                    every_third += 2;
                    a = every_third;
                }
                else
                    a = 1;
                
                p_2 = p_1;
                p_1 = p;
                p = addition(multiply(num_to_array(a), p_1), p_2);
            }
            return p.Sum();
        }

        static int[] num_to_array(int n)
        {
            // same alogorithm as itoa0
            var result = new List<int>();
            do
            {
                result.Add(n % 10);
            } while ((n /= 10) > 0);

            return result.ToArray();
        }

        /// <summary>
        /// Schönhage–Strassen algorithm
        /// http://en.wikipedia.org/wiki/Sch%C3%B6nhage%E2%80%93Strassen_algorithm
        /// </summary>
        /// <param name="a">previous number</param>
        /// <param name="b">next number to multiply</param>
        /// <returns>the product</returns>
        static int[] multiply(int[] a, int[] b)
        {
            var result = new List<int>();
            int shift, product, i, j;
            var index = 0;
            for (i = 0; i < a.Length; i++)
            {
                shift = index;
                for (j = 0; j < b.Length; j++)
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

        static int[] addition(int[] a, int[] b)
        {
            var len = a.Length > b.Length ? a.Length : b.Length;
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
