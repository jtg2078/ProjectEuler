using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler038csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The largest 1 to 9 pandigital 9-digit number that can be formed:");

            TestBenchSetup();
            TestBenchLoader(test_each_number_start_with_nine_below_ten_thousands);
            TestBenchLoader(test_number_within_nine_to_ten_thousand);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// since the question stated that 9 * (n=1,2,3,4,5) = 918273645
        /// and obviously from how the question is phrased, the number(918273645) is not the largest number
        /// so the answer can only be greater than that. that means the number has to be 9
        /// e.g. since answer must consist of x * n(1,2,3) = (x*1)(x*2)(x*3) and x*1 must start with 9
        /// which makes x be 9, 9.. , 9... , 9....  and the number cant be greater than 5 digits, since
        /// (5-digit*1)(5-digit*2)=at least 10 digits, so this method test each number in range of 90~99,
        /// 900~999, 9000~9999 to find the answer
        /// </summary>
        /// <returns></returns>
        static int test_each_number_start_with_nine_below_ten_thousands()
        {
            var init = 9;
            var tens = 10;
            var n = 0;
            var cur = 0;
            var set = new HashSet<int>();
            var duplicate = false;
            var str = new StringBuilder();
            var tmp = 0;
            var result = new List<string>();
            while (tens <= 10000)
            {
                for (int i = init; i < tens; i++)
                {
                    n = 1;
                    duplicate = false;
                    set.Clear();
                    set.Add(0);
                    str.Remove(0, str.Length);
                    while (true)
                    {
                        cur = i * n;
                        tmp = cur;
                        while (tmp > 0)
                        {
                            if (set.Add(tmp % 10) == false)
                            {
                                duplicate = true;
                                break;
                            }
                            tmp /= 10;
                        }
                        if (duplicate)
                            break;
                        else
                            str.Append(cur);
                        n++;
                    }
                    if (str.Length == 9)
                        result.Add(str.ToString());
                        //Console.WriteLine(string.Format("int:{0},   product:{1}", i, str));
                }
                init *= 10;
                tens *= 10;
            }
            return Convert.ToInt32(result.Max());
        }

        /// <summary>
        /// this method is more refined than previous method, since the number has to start with digit 9
        /// which makes 9x, 9xx, 9xxx...etc, when n=2, the number times 2 would definitely add 1 more digit to it
        /// so for example, 9*2 = 18, 98*2= 196. so 3 digit number would become 9xx = (3-digit) (4-digit) (4-digit)
        /// either way, it wouldnt fit to be a 9-digit number.
        /// so the only possible range for the number is 4 digit with n=1,2, which makes (4-digit, n=1) (5-digit, n=2)
        /// this method test number range from 9999 down to 9000, the first number found must be the largest number, 
        /// and therefore the answer.
        /// </summary>
        /// <returns></returns>
        static int test_number_within_nine_to_ten_thousand()
        {
            int num, digits;
            var set = new HashSet<int>();
            var has_duplicate = false;
            for (int i = 9876; i >= 9123; i--)
            {
                num = i * 100000 + i * 2;
                has_duplicate = false;
                set.Clear();
                set.Add(0);
                digits = num;
                while (digits > 0)
                {
                    if (set.Add(digits % 10) == false)
                    {
                        has_duplicate = true;
                        break;
                    }
                    digits /= 10;
                }
                if (has_duplicate == false)
                    return num;
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
