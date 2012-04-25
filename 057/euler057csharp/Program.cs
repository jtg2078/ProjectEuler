using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler057csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The fractions contain a numerator with more digits than denominator: ");

            TestBenchSetup();
            TestBenchLoader(brute_froce_construct_each_fraction_number_and_compare);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        struct fraction
        {
            public int[] numerator;
            public int[] denominator;
        }

        /// <summary>
        /// basically brute force and find out the exact fraction for each iteration and compare the length
        /// since the the iteration goes like this, 
        /// 1 + 1/(2+1/2) = 1 + a (iteration 2) seed = 1/(2+1/2)
        /// 1 + 1/(2+1/(2+1/2)) = 1 + 1 / 2 + a (iteration 3) seed = 1/(2+1/(2+1/2)) = 1 + 1/(2+a)
        /// so on...
        /// </summary>
        static int brute_froce_construct_each_fraction_number_and_compare()
        {
            var counter = 0;
            var seed = add_fraction_and_flip(TWO_OVER_ONE, ONE_OVER_TWO);
            var result = new fraction();
            for (int i = 1; i < 1000; i++)
            {
                seed = add_fraction_and_flip(TWO_OVER_ONE, seed);
                result = add_fraction(ONE_OVER_ONE, seed);
                
                if (result.numerator.Length > result.denominator.Length)
                    counter++;

                //Console.WriteLine(string.Format("i:{0} {1}/{2}",
                //    i + 1,
                //    result.numerator.Reverse().Aggregate(new StringBuilder(), (s, n) => s.Append(n)).ToString(),
                //    result.denominator.Reverse().Aggregate(new StringBuilder(), (s, n) => s.Append(n)).ToString()));
            }
            return counter;
        }

        static readonly fraction ONE_OVER_ONE = new fraction() { numerator = new int[] { 1 }, denominator = new int[] { 1 } };
        static readonly fraction TWO_OVER_ONE = new fraction() { numerator = new int[] { 2 }, denominator = new int[] { 1 } };
        static readonly fraction ONE_OVER_TWO = new fraction() { numerator = new int[] { 1 }, denominator = new int[] { 2 } };

        static fraction add_fraction(fraction a, fraction b)
        {
            var result = new fraction();
            result.denominator = multiply(a.denominator, b.denominator);
            result.numerator = addition(multiply(a.numerator, b.denominator), multiply(b.numerator, a.denominator));
            return result;
        }

        static fraction add_fraction_and_flip(fraction a, fraction b)
        {
            var result = new fraction();
            result.denominator = addition(multiply(a.numerator, b.denominator), multiply(b.numerator, a.denominator));
            result.numerator = multiply(a.denominator, b.denominator);
            return result;
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
