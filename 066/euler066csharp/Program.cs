using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler066csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The value of D ≤ 1000 in minimal solutions of x for which the largest value of x is obtained: ");

            TestBenchSetup();
            TestBenchLoader(using_pells_equation_with_continued_fractions_and_convergents);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        static int using_pells_equation_with_continued_fractions_and_convergents()
        {
            var limit = 1000;
            var bound = (int)Math.Sqrt(limit);
            var map = new bool[limit + 1];
            var result = num_to_array(0);
            var tmp = num_to_array(0);
            int largest = 0;

            for (int i = 0; i < limit; i++)
            {
                if (i <= bound)
                    map[i * i] = true; // using this way to mark perfect squares

                if (map[i] == false) // perfect squares are skipped
                {
                    tmp = find_convergent(i);
                    if (compare(result, tmp) == -1)
                    {
                        result = tmp;
                        largest = i;
                    }
                }
            }
            return largest;
        }

        static int[] find_convergent(int i)
        {
            // for finding a0,a1,a2......
            var a0 = (int)Math.Sqrt(i);
            var m = 0;
            var d = 1;
            var a = a0;

            // for finding convergent
            var h_2 = num_to_array(0);
            var h_1 = num_to_array(1);
            var h = addition(multiply(num_to_array(a), h_1), h_2);
            var k_2 = num_to_array(1);
            var k_1 = num_to_array(0);
            var k = addition(multiply(num_to_array(a), k_1), k_2);
            var approx = num_to_array(0);
            var one = num_to_array(1);
            var i_in_array = num_to_array(i);
            while (true)
            {
                m = d * a - m;
                d = (i - m * m) / d;
                a = (a0 + m) / d;

                h_2 = h_1;
                h_1 = h;
                h = addition(multiply(num_to_array(a), h_1), h_2);

                k_2 = k_1;
                k_1 = k;
                k = addition(multiply(num_to_array(a), k_1), k_2);
                approx = subtraction(multiply(h, h), multiply(i_in_array, multiply(k, k)));

                if (compare(approx, one) == 0) // so minimal x is found
                    return h;
            }
        }

        static int[] num_to_array(int n)
        {
            var sign = n < 0 ? -1 : 1;
            n = Math.Abs(n);
            var result = new List<int>();
            do
            {
                result.Add(n % 10);
            } while ((n /= 10) > 0);
            result[result.Count - 1] = result[result.Count - 1] * sign;
            return result.ToArray();
        }

        // only work if a and b are positive, which is adequate for this problem
        static int[] multiply(int[] a, int[] b)
        {
            var result = new List<int>();
            int shift, product, i, j;
            var index = 0;
            if (a.Sum() == 0 || b.Sum() == 0)
                return new int[] { 0 };
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

        // only work if a and b are positive, which is adequate for this problem
        static int[] subtraction(int[] a, int[] b)
        {
            var ret = compare(a, b);
            var len = Math.Max(a.Length, b.Length);
            var result = new List<int>(len);
            int borrow = 0;
            int x, y, diff;

            if (ret == 0)
                return new int[] { 0 }; // a == b

            for (int i = 0; i < len; i++)
            {
                x = i < a.Length ? a[i] : 0;
                y = i < b.Length ? b[i] : 0;

                diff = x - y + borrow;
                if (ret == 1)
                    borrow = diff < 0 ? -1 : 0;
                else
                    diff = y - x + borrow;
                if (diff < 0)
                    diff += 10;
                result.Add(diff);
            }
            for (int i = result.Count - 1; i >= 0; i--)
            {
                if (result[i] == 0)
                    result.RemoveAt(i);
                else
                    break;
            }
            if (ret == -1)
                result[result.Count - 1] *= -1;
            return result.ToArray<int>();
        }

        static int compare(int[] a, int[] b)
        {
            var a_len = a.Length;
            var b_len = b.Length;
            var a_last = a[a_len - 1];
            var b_last = b[b_len - 1];
            if (a_len > b_len)
            {
                if (a_last > 0 && b_last > 0)
                    return 1;
                else if (a_last < 0 && b_last > 0)
                    return -1;
                else if (a_last < 0 && b_last < 0)
                    return -1;
            }
            else if (a_len < b_len)
            {
                if (a_last > 0 && b_last > 0)
                    return -1;
                else if (a_last > 0 && b_last < 0)
                    return 1;
                else if (a_last < 0 && b_last < 0)
                    return 1;
            }
            else
            {
                for (int i = a_len - 1; i >= 0; i--)
                {
                    int x = a[i];
                    int y = b[i];
                    if (x > y)
                        return 1;
                    else if (x < y)
                        return -1;
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
