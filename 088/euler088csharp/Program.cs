using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler088csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The sum of all the minimal product-sum numbers for 2≤k≤12000: ");

            //TestBenchSetup();
            //TestBenchLoader(brute_force_by_generate_and_test_multiplicative_partitions);
            brute_force_by_generate_and_test_multiplicative_partitions();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// since the number cannot be lower or same as k, and the num which is(2*k) would always satisfy k
        /// for example: k=3, n=2*k=6, which in turns 1*2*3=6=1+2+3. so the number to test can be limited to k+1 to 2k
        /// the next step is to generate and test all the multiplicative partitions to see if the product-sum condition satisfied
        /// using the method test_product_sum
        /// really slow, but orwill
        /// </summary>
        static int brute_force_by_generate_and_test_multiplicative_partitions()
        {
            var k_range = 12000;
            var k = 2;
            var len = k_range * 2 + 1;
            var map = new int[len];
            while (k <= k_range)
            {
                var bound = k * 2;
                for (int num = k + 1; num <= bound; num++)
                {
                    if (test_product_sum(num, 0, 0, num, k) == true)
                    {
                        map[num]++;
                        break;
                    }
                }
                k++;
            }
            var result = 0;
            for (int i = 0; i < len; i++)
            {
                if (map[i] != 0)
                    result += i;
            }
            return result;
        }

        /// <summary>
        /// basically generate all the multiplicative partitions and return true for first one found hat matched the problem's
        /// condition. return false if none to be found
        /// </summary>
        static bool test_product_sum(int num, int divisor_sum, int divisor_count, int number_to_test, int k)
        {
            var bound = num / 2;
            var rem = 0;
            var quotient = 0;
            for (int divisor = 2; divisor <= bound; divisor++)
            {
                quotient = Math.DivRem(num, divisor, out rem);
                if (rem == 0)
                {
                    var local_sum = divisor_sum + divisor;
                    var local_count = divisor_count + 1;

                    var total_sum = local_sum + quotient;
                    var total_count = local_count + 1;

                    if ((total_count == k && total_sum == number_to_test) || (number_to_test - total_sum == k - total_count))
                        return true;

                    if (test_product_sum(quotient, local_sum, local_count, number_to_test, k) == true)
                        return true;
                }
            }
            return false;
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
