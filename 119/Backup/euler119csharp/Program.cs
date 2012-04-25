using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler119csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The a30th in the series: ");

            TestBenchSetup();
            TestBenchLoader(narrow_down_search_range_with_possible_candidates);

            Console.WriteLine("press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// Since numbers in this series is some number raised to some powers, and so is the answer im looking for.
        /// This narrows down search range substantially.
        /// 1. generate_map method is called which returns a nested dictionary contain numbers and their various
        ///    raised powered number. The method also return a candidates list which contained all the raised power
        ///    numbers
        /// 2. loop through the nested dictionary returned by generate_map and match against the candidates list
        ///    and then the answer can be obtain this way~
        /// </summary>
        static long narrow_down_search_range_with_possible_candidates()
        {
            var candidates = new List<long>();
            var map = generate_map(candidates);
            var counter = 10;
            long previous = 0;
            foreach (var num in candidates)
            {
                if (num < 614656)
                    continue;

                if (num == previous)
                    continue;
                else
                    previous = num;

                var sum = digit_sum(num);
                if (map.ContainsKey(sum) == true && map[sum].ContainsKey(num) == true)
                {
                    counter++;

                    if (counter > 30)
                        return num;
                }
            }
            return 0;
        }

        static int digit_sum(long num)
        {
            long sum = 0;
            while (num > 0)
            {
                sum += num % 10;
                num /= 10;
            }
            return (int)sum;
        }

        /// <summary>
        /// I've decided to use long.maxValue as the max range. Good thing the answer is well below the chosen max
        /// construct a nested dictionary structure to faster access
        /// key(digit sum)--->value(                                                                )
        ///                         key(raised power of digit sum)--->value(the power used to raise)
        /// </summary>
        /// <param name="candidates"></param>
        /// <returns></returns>
        static Dictionary<int, Dictionary<long, int>> generate_map(List<long> candidates)
        {
            var map = new Dictionary<int, Dictionary<long, int>>();
            var max = ((int)Math.Log10(long.MaxValue) - 1) * 9; // find out how many digits are there in long.maxValue
            // and 9 is the largest digit
            for (int num = 2; num <= max; num++)
            {
                var ceil = (int)Math.Log(long.MaxValue, num);
                long current = 1;
                map.Add(num, new Dictionary<long, int>());
                for (int p = 1; p < ceil; p++)
                {
                    current *= num;
                    map[num][current] = p;
                    candidates.Add(current);
                }
            }
            candidates.Sort(); // needed
            return map;
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

