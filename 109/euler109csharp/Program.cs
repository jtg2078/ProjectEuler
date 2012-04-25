using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler109csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The number of distinct ways can a player checkout with a score less than 100: ");

            TestBenchSetup();
            TestBenchLoader(generate_and_test_each_possible_ways_for_given_target_score);

            Console.WriteLine("press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// basically generate all the possible combinations of dart throws for each given score
        /// and count the distinct way
        /// </summary>
        static int generate_and_test_each_possible_ways_for_given_target_score()
        {
            // regions are stored as array of int where:
            //
            // array[0]=0 : not used   array[20]=20 : S20       ...                  array[70]=60 : T20
            // array[1]=1 : S1         array[21]=0 : not used   array[50]=50 : D25
            // array[2]=2 : S2         array[25]=25 : S25       array[51]=3 : T3
            // ...                     array[26]=2 : D1         ...
            var regions = new int[71];
            var regions_debug = new string[71];
            for (int i = 1; i <= 25; i++)
            {
                if (i <= 20 || i >= 25) // single, double
                {
                    regions[i] = i;
                    regions[25 + i] = i * 2;

                    // debug
                    regions_debug[i] = "S" + i;
                    regions_debug[25 + i] = "D" + i;

                    if (i <= 20) // triple
                    {
                        regions[50 + i] = i * 3;

                        //debug
                        regions_debug[50 + i] = "T" + i;
                    }
                }
            }

            var result = 0;
            var range = 100;
            for (int score = 1; score < range; score++)
            {
                result += compute_distinct_ways_for_given_score(score, regions, false, regions_debug);
            }
            return result;
        }

        static int compute_distinct_ways_for_given_score(int score, int[] regions, bool turn_on_debug, string[] regions_debug)
        {
            var distinct = 0;
            var ways = new HashSet<int>(); // for checking distinct
            for (int s3 = 26; s3 <= 50; s3++)
            {
                var s3_score = regions[s3];
                if (s3_score != 0 && s3_score < score)
                {
                    for (int s2 = 1; s2 <= 70; s2++)
                    {
                        var s2_score = regions[s2];
                        if (s2_score == 0)
                            continue;

                        var s2s3 = s2_score + s3_score;
                        if (s2s3 < score)
                        {
                            for (int s1 = 1; s1 <= 70; s1++)
                            {
                                var s1_score = regions[s1];
                                if (s1_score == 0)
                                    continue;

                                var s1s2s3 = s1_score + s2s3;
                                if (s1s2s3 == score)
                                {
                                    var way = 0;
                                    way = s1 <= s2 ? s1 * 100 + s2 : s2 * 100 + s1; // sort s1 and s2

                                    if (ways.Add(way) == true) // so this way is distinct
                                    {
                                        distinct++;
                                        if (turn_on_debug == true)
                                            Console.WriteLine(regions_debug[s1] + regions_debug[s2] + regions_debug[s3]);
                                    }
                                }
                            }
                        }
                        else if (s2s3 == score)
                        {
                            if (ways.Add(s2) == true)
                            {
                                distinct++;
                                if (turn_on_debug == true)
                                    Console.WriteLine(regions_debug[s2] + regions_debug[s3]);
                            }
                        }
                    }
                }
                else if (s3_score == score)
                {
                    distinct++;
                    if (turn_on_debug == true)
                        Console.WriteLine(regions_debug[s3]);
                }
            }
            return distinct;
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
