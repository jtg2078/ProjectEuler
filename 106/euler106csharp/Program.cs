using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler106csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The number of subset pairs that needed to be tested for equality: ");

            TestBenchSetup();
            TestBenchLoader(using_check_condition_method_from_problem_105_with_modification);
            TestBenchLoader(awesome_mathematical_analysis_with_DyckPath);

            Console.WriteLine("press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// since the problem stated that condition ii is already satisifed, and the set contains
        /// n strictly increasing elements.
        /// to check condition i, only two set with equal number of elements would need to be checked
        /// however, since the set is already in sorted order, the set which the ordinal element is always bigger than 
        /// the other set can be omitted. The rest of the pairs are the one that need to be compare and checked
        /// </summary>
        static int using_check_condition_method_from_problem_105_with_modification()
        {
            var n = 12;
            var mask = new int[n];
            var map = new int[(int)Math.Pow(2, n), n];
            var pos = n - 1;
            var row = 0;
            var col = 0;

            while (pos >= 0)
            {
                if (mask[pos] == 0)
                {
                    mask[pos] = 1;
                    pos = n - 1;

                    for (int i = 0; i < n; i++)
                    {
                        if (mask[i] == 1)
                        {
                            map[row, i] = 1;
                        }
                    }
                    row++;
                }
                else
                {
                    mask[pos] = 0;
                    pos--;
                }
            }

            row = 0;
            col = 0;
            var cur = 0;
            var bound = (int)Math.Pow(2, n) - 1;
            var half = n / 2;
            var result = 0;
            var src_set = new int[n];
            var dest_set = new int[n];
            for (cur = 0; cur < bound; cur++)
            {
                var cur_count = 0;

                // reset the set that holds current start set
                src_set[0] = 0; src_set[1] = 0; src_set[2] = 0;
                src_set[3] = 0; src_set[4] = 0; src_set[5] = 0;

                // copy the current start set col index
                for (col = 0; col < n; col++)
                {
                    if (map[cur, col] == 1)
                    {
                        src_set[cur_count] = col;
                        cur_count++;
                    }
                }

                // no point to continue if current start set size is greater than half of the whole set
                if (cur_count > half)
                    continue;

                for (row = cur + 1; row < bound; row++)
                {
                    var count = 0;
                    var disjoint = true;
                    var possible = false;
                    // reset the set that holds current destination set
                    dest_set[0] = 0; dest_set[1] = 0; dest_set[2] = 0;
                    dest_set[3] = 0; dest_set[4] = 0; dest_set[5] = 0;

                    for (col = 0; col < n; col++)
                    {
                        if (map[row, col] == 1)
                        {
                            if (map[cur, col] == 1 || count > cur_count)
                            {
                                disjoint = false;
                                break;
                            }
                            dest_set[count] = col;
                            // if there are any element in dest set that are bigger than src set
                            // which means dest set is not strickly smaller than src set and comparsion is needed
                            if (dest_set[count] > src_set[count])
                            {
                                possible = true;
                            }
                            count++;
                        }
                    }
                    if (disjoint == true && possible == true && count > 1 && count == cur_count)
                    {
                        result++;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// saw this awesome way of solving this problem using dyck path
        /// </summary>
        static int awesome_mathematical_analysis_with_DyckPath()
        {
            Func<int, int, int> C = (left, right) =>
                {
                    if (right > left)
                        return 0;
                    var sum = 1;
                    for (int i = 1; i <= right; i++)
                    {
                        sum = sum * (left - right + i) / i;
                    }
                    return sum;
                };

            int n = 12, result = 0;
            for (int a = 1; a <= n / 2; ++a)
            {
                result += C(n, a * 2) * (C(a * 2, a) / 2 - C(2 * a, a) / (a + 1));
            }
            return result;
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
