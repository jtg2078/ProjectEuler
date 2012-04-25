using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler103csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The set string: ");

            TestBenchSetup();
            TestBenchLoader(slow_brute_force_with_combination_and_subsets_generation);

            Console.WriteLine("press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// bascially generates all the combination of picking 7 numbers out of 20-50
        /// (the 50 is coming from http://journals.cambridge.org/production/action/cjoGetFulltext?fulltextid=6993056)
        /// and then using check_OSS_conditions method to see if it satisfies the OSS set conditions.
        /// extremely slow. takes 22sec on my machine
        /// and the answer is same as the result produced by "near optimum set algorithm" provided by the question 
        /// </summary>
        /// <returns></returns>
        static long slow_brute_force_with_combination_and_subsets_generation()
        {
            var set = new int[7];
            var mask = new int[7];
            var lower = 20;
            var upper = 50; //  according to paper, the upperbound is less or equal to previous OSS's largest element
            var bucket = new int[50 + 49 + 48 + 47 + 46 + 45 + 44]; //329
            var map = new SortedDictionary<int, int>();
            int n1, n2, n3, n4, n5, n6, n7, sum;
            var min = int.MaxValue;
            long result = 0;
            for (n1 = lower; n1 < 45; n1++)
            {
                set[0] = n1;
                for (n2 = n1 + 1; n2 < 46; n2++)
                {
                    set[1] = n2;
                    for (n3 = n2 + 1; n3 < 47; n3++)
                    {
                        set[2] = n3;
                        for (n4 = n3 + 1; n4 < 48; n4++)
                        {
                            set[3] = n4;
                            for (n5 = n4 + 1; n5 < 49; n5++)
                            {
                                set[4] = n5;
                                for (n6 = n5 + 1; n6 < 50; n6++)
                                {
                                    set[5] = n6;
                                    for (n7 = n6 + 1; n7 < 51; n7++)
                                    {
                                        set[6] = n7;
                                        sum = check_OSS_conditions(set, mask);
                                        if (sum != 0 && sum < min)
                                        {
                                            min = sum;
                                            result = set[0];
                                            for (int i = 1; i < 7; i++)
                                            {
                                                result = result * 100 + set[i];
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// generates subsets by using binary counter (http://compprog.wordpress.com/2007/10/10/generating-subsets/)
        /// checks the condition of sums of subsets cannot be equal by using a sorted dictionary
        /// </summary>
        /// <param name="nums">contains the set to be check for optimum special sum set</param>
        /// <param name="mask">the mask which is used for generating subsets</param>
        /// <returns></returns>
        static int check_OSS_conditions(int[] nums, int[] mask)
        {
            for (int i = 0; i < 7; i++)
            {
                mask[i] = 0;
            }
            var pos = 6;
            var map = new SortedDictionary<int, int>();
            var sum = 0;
            var counter = 0;
            while (pos >= 0)
            {
                if (mask[pos] == 0)
                {
                    mask[pos] = 1;
                    pos = 6;

                    sum = 0;
                    counter = 0;
                    for (int z = 0; z < mask.Length; z++)
                    {
                        if (mask[z] == 1)
                        {
                            counter++;
                            sum += nums[z];
                        }
                    }
                    if (map.ContainsKey(sum) == true)
                        return 0;
                    else
                        map.Add(sum, counter);
                }
                else
                {
                    mask[pos] = 0;
                    pos--;
                }
            }
            var prev = 0;
            var cur = 0;
            foreach (var pair in map)
            {
                cur = pair.Value;
                if (cur < prev)
                    return 0;

                prev = cur;
            }
            return nums.Sum();
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
