using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace euler105csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The sum of all special sum sets: ");

            TestBenchSetup();
            TestBenchLoader(using_binary_counter_mechanism_from_problem_103);

            Console.WriteLine("press any key to exit");
            Console.ReadKey();
        }

        static int using_binary_counter_mechanism_from_problem_103()
        {
            var mask = new int[12];
            var total = 0;
            foreach (var line in File.ReadAllLines(@"..\..\sets.txt"))
            {
                var nums = line.Split(new char[] { ',' }).Select(s => Convert.ToInt32(s)).ToList();
                total += check(nums, mask);
            }
            return total;
        }

        /// <summary>
        /// returns 0 if the nums is not a special sum set, 
        /// for any other number, it means that the nums is a special sum set, and the returned number is
        /// sum of all the number in the set
        /// </summary>
        static int check(List<int> nums, int[] mask)
        {
            // resets the mask(since the max is 12 as stated in problem's description
            mask[0] = 0; mask[1] = 0; mask[2] = 0; mask[3] = 0;
            mask[4] = 0; mask[5] = 0; mask[6] = 0; mask[7] = 0;
            mask[8] = 0; mask[9] = 0; mask[10] = 0; mask[11] = 0;

            var pos = nums.Count - 1;
            var map = new SortedDictionary<int, int>();
            var sum = 0;
            var counter = 0;
            // using binary counter mechanism to generate subsets
            while (pos >= 0)
            {
                if (mask[pos] == 0)
                {
                    mask[pos] = 1;
                    pos = nums.Count - 1;

                    sum = 0;
                    counter = 0;
                    for (int i = 0; i < nums.Count; i++)
                    {
                        if (mask[i] == 1)
                        {
                            counter++;
                            sum += nums[i];
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
