using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler055csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The number of Lychrel numbers below ten-thousand: ");

            TestBenchSetup();
            TestBenchLoader(using_decimal_number_type_for_calculation);
            TestBenchLoader(using_ulong_number_type_for_calculation);
            TestBenchLoader(using_int_array_type_for_calculation);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// since from the question's post, 28-digit seems to be a good upper bound for number(below 10k) with 50k
        /// so decimal value with 28 precision is enough to avoid overflow
        /// </summary>
        static int using_decimal_number_type_for_calculation()
        {
            int n, t;
            decimal num = 0;
            var result = 0;
            for (n = 10; n < 10000; n++)
            {
                num = n;
                for (t = 0; t < 50; t++)
                {
                    num += reverse_dec(num);
                    if (num == reverse_dec(num))
                        break;
                }
                if (t == 50)
                    result++;
            }
            return result;
        }

        static decimal reverse_dec(decimal n)
        {
            decimal result = 0;
            while (n > 0)
            {
                result = result * 10 + n % 10;
                n = decimal.Floor(n / 10);
            }
            return result;
        }

        /// <summary>
        /// using ulong, only 18-digits, and very likely to overflow(and it did), but still
        /// the answer came out to be correct
        /// </summary>
        static int using_ulong_number_type_for_calculation()
        {
            uint n, t;
            ulong num = 0;
            var result = 0;
            for (n = 10; n < 10000; n++)
            {
                num = n;
                for (t = 0; t < 50; t++)
                {
                    num += reverse_ulong(num);
                    if (num == reverse_ulong(num))
                        break;
                }
                if (t == 50)
                    result++;
            }
            return result;
        }

        /// <summary>
        /// reverse the number and return as ulong
        /// </summary>
        static ulong reverse_ulong(ulong n)
        {
            ulong result = 0;
            while (n > 0)
            {
                result = result * 10 + n % 10;
                n /= 10;
            }
            return result;
        }

        /// <summary>
        /// using array to store number, it also makes comparing and test palindrome easier
        /// </summary>
        static int using_int_array_type_for_calculation()
        {
            int n, t;
            var result = 0;
            int[] num;
            for (n = 10; n < 10000; n++)
            {
                num = num_to_array(n);
                for (t = 0; t < 50; t++)
                {
                    num = addition(num, num.Reverse().ToArray());
                    if (num.SequenceEqual(num.Reverse()))
                        break;
                }
                if (t == 50)
                    result++;
            }
            return result;
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
