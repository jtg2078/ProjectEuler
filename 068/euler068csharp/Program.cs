using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler068csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The the maximum 16-digit string for a magic 5-gon ring: ");

            TestBenchSetup();
            TestBenchLoader(brute_force_with_lexicographic_permutation);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// basically using the lexicographic permutation to generate all possible number combination and 
        /// applies each rule to see the if number set matched all the criteria.
        /// really slow(1 sec!). just the permutation itself is 3mil+ iterations, and rules checking take some time too
        /// a lot optimization could be made, or another way. but not today. im done!
        /// </summary>
        static ulong brute_force_with_lexicographic_permutation()
        {
            var a = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            Func<int> line1 = () => a[5] + a[0] + a[1]; // so inner 5 nodes are a[0] to a[4] (clockwise)
            Func<int> line2 = () => a[6] + a[1] + a[2]; //    outer 5 nodes are a[5] to a[9] (clockwise)
            Func<int> line3 = () => a[7] + a[2] + a[3];
            Func<int> line4 = () => a[8] + a[3] + a[4];
            Func<int> line5 = () => a[9] + a[4] + a[0];
            Func<bool> rule1 = () => a.Take(5).All(n => n != 10); // 10 cannot appear in centers (16-digits only)
            Func<bool> rule2 = () => a[5] < a[6] && a[5] < a[7] && a[5] < a[8] && a[5] < a[9]; // first must be smallest
            Func<bool> rule3 = () => line1() == line2() && line1() == line3() && line1() == line4() && line1() == line5();
            var j = 0;
            var l = 0;
            var tmp = 0;
            var len = a.Length;
            var z = 0;
            ulong cur = 0;
            ulong max = 0;
            while (true)
            {
                //step 1. Find the largest index j such that a[j] < a[j + 1].
                for (j = len - 2; j >= 0; j--)
                {
                    if (a[j] < a[j + 1])
                        break;
                }

                if (j == -1)
                    break; // no more permutation, since no such index exist

                //step 2. Find the largest index l such that a[j] < a[l]
                for (l = len - 1; l >= 0; l--)
                {
                    if (a[j] < a[l])
                        break;
                }

                //step 3. Swap a[j] with a[l].
                tmp = a[j];
                a[j] = a[l];
                a[l] = tmp;

                //step 4. Reverse the sequence from a[j + 1] up to and including the final element a[n]
                z = len - 1;
                for (l = j + 1; l < len; l++)
                {
                    if (l >= z)
                        break;

                    tmp = a[l];
                    a[l] = a[z];
                    a[z] = tmp;
                    z--;
                }
                if (rule1() && rule2() && rule3())
                {
                    var str = new StringBuilder();
                    str.Append(string.Format("{0}{1}{2}", a[5], a[0], a[1]));
                    str.Append(string.Format("{0}{1}{2}", a[6], a[1], a[2]));
                    str.Append(string.Format("{0}{1}{2}", a[7], a[2], a[3]));
                    str.Append(string.Format("{0}{1}{2}", a[8], a[3], a[4]));
                    str.Append(string.Format("{0}{1}{2}", a[9], a[4], a[0]));
                    cur = Convert.ToUInt64(str.ToString());
                    if (max < cur)
                        max = cur;
                }
            }
            return max;
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
        static void TestBenchLoader(Func<ulong> test_method)
        {
            stopwatch.Reset();
            stopwatch.Start();
            ulong result = 0;
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
