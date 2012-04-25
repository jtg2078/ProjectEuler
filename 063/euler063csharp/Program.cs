using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler063csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The numbers of n-digit positive integers exist which are also an nth power: ");

            TestBenchSetup();
            TestBenchLoader(using_list_to_store_number_to_avoid_overflow);
            TestBenchLoader(crazy_math_formula);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// basically the idea is that only number below two digits can have such number that equal to nth power
        /// since two digits number will always have more digits than its nth power eg. 10^1=10(2), 10^2=100(3)
        /// and since for single digit number, the most additional digit that can be add is 1. 
        /// eg. 9^1=3(no additional digits), 9^2=81(one additional digits), 9^3=729(one additional digits)
        /// so at any given time, if power becomes bigger than digits count, then digit count will not be able to catch
        /// up. Since the "product" will overflow, List of int and simple_multiply method is used for multiplication
        /// </summary>
        static int using_list_to_store_number_to_avoid_overflow()
        {
            int num, power, digits;
            var count = 1; // for 1^1
            var product = new List<int>();
            for (num = 2; num < 10; num++)
            {
                product.Clear();
                product.Add(1);
                for (power = 1; true; power++)
                {
                    simple_multiply(product, num);
                    digits = product.Count;

                    if (power > digits)
                        break;

                    if (power == digits)
                        count++;
                }
            }
            return count;
        }

        /// <summary>
        /// from the problem's discussion post (from Alvaro)
        /// You know that x^n has n digits when 
        ///     10^(n-1) <= x^n < 10^n 
        /// First of all, x < 10. So we only have to try x={1, 2, 3, ..., 9}. 
        /// The next thing to notice is that the left inequality is true for small values of n, 
        /// but the 10^(n-1) part grows faster than the x^n part. All you have to do is find out when they meet. 
        /// This can be done like this
        /// 10^(n-1)=x^n => 0.1*10^n=x^n => log(0.1)+n*log(10)=n*log(x)
        /// => log(10)=n*(log(10)-log(x)) => n=log(10)/(log(10)-log(x))
        /// That value of n is already not good, so we have to round down to find out the largest integer 
        /// for which the inequality holds true. 
        /// </summary>
        static int crazy_math_formula()
        {
            int num;
            var result = 0;
            var count = 0;
            for (num = 1; num < 10; num++)
            {
                count = (int)Math.Floor(Math.Log(10) / (Math.Log(10) - Math.Log(num)));
                result += count;
            }
            return result;
        }

        static List<int> tmpList = new List<int>();
        static void simple_multiply(List<int> a, int b)
        {
            int i;
            tmpList.Clear();
            var len = a.Count;
            for (i = 0; i < len; i++)
            {
                tmpList.Add(a[i] * b);
            }
            var carry = 0;
            len = tmpList.Count;
            for (i = 0; i < len; i++)
            {
                tmpList[i] += carry;
                carry = tmpList[i] / 10;
                tmpList[i] = tmpList[i] % 10;
                a[i] = tmpList[i];
            }
            if (carry > 0)
                a.Add(carry);
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
