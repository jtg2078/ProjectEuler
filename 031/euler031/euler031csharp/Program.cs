using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler031csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The different ways can £2 be made using any number of coins:");

            TestBenchSetup();
            TestBenchLoader(brute_force_loop_all_possibilities);
            TestBenchLoader(controlled_brute_force_loop_eliminated_dead_ends);
            TestBenchLoader(the_1337_dynamic_programming);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// iterate every possibilities, lawl
        /// </summary>
        /// <returns></returns>
        static int brute_force_loop_all_possibilities()
        {
            var counter = 0;
            for (int a = 0; a <= 1; a++)
            {
                for (int b = 0; b <= 2; b++)
                {
                    for (int c = 0; c <= 4; c++)
                    {
                        for (int d = 0; d <= 10; d++)
                        {
                            for (int e = 0; e <= 20; e++)
                            {
                                for (int f = 0; f <= 40; f++)
                                {
                                    for (int g = 0; g <= 100; g++)
                                    {
                                        for (int h = 0; h <= 200; h++)
                                        {
                                            if (a * 200 + b * 100 + c * 50 + d * 20 + e * 10 + f * 5 + g * 2 + h == 200)
                                                counter++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return counter;
        }

        /// <summary>
        /// by filtering out the impossible value(like when 1 200p coin is used, there is no point keep going)
        /// a alot of unnecessary loop is eliminated
        /// </summary>
        /// <returns></returns>
        static int controlled_brute_force_loop_eliminated_dead_ends()
        {
            var counter = 0;
            int a, b, c, d, e, f, g;
            for (a = 200; a >= 0; a -= 200)
            {
                for (b = a; b >= 0; b -= 100)
                {
                    for (c = b; c >= 0; c -= 50)
                    {
                        for (d = c; d >= 0; d -= 20)
                        {
                            for (e = d; e >= 0; e -= 10)
                            {
                                for (f = e; f >= 0; f -= 5)
                                {
                                    for (g = f; g >= 0; g -= 2)
                                    {
                                        counter++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return counter;
        }

        /// <summary>
        /// the dynamic programming way, lawl
        /// (still considered to be voodoo magic to me, but getting better)
        /// so lets say an array with map[0...201] where the index is the dollar value
        /// and map[index] = all the ways made using any number of coins
        /// if we construct this map from the beginning and starts with 1 coin
        /// we can gradually transform the map with new ways added when more and more coins are added
        /// which means that for n-th coins, we can use the result of (n-1)th coin and begin from there
        /// so this make it a dynamic programming friendly problem, by solving all the subset problems
        /// 1,2,,,n-th problem we would get the final answer, and all the subset result help creating the 
        /// answer for final result, so we dont do any unnecessary/redundant calculations
        /// start with 1st coin(1p), there is only 1 way for each value(since there is only 1 type of coin)
        /// so after 1st coin(p1), the map is filled with 1.
        /// now moves to next coin(2p), when value is 1(map[1]), 2p coin cant be used, so the way remain unchanged
        /// when value is 2, which is same as the coin, we know now there are 2 ways(one p2, or 2 p1)
        /// when value is 3, we can definitely use 1 p2, so the remaining value becomes 3-2=1, since the value(map[1]) 
        /// contains 1 way, we know the following:
        ///     1. by using the current latest coin(p2), the remaining value's way is stored in map[current value-p2's value]
        ///     2. if the current value is equal to current latest coin's value, then now we have one additional way
        ///     3. the current value contain all the ways of all previous walked-through coins(so far only p1)
        ///     4. so we can sum up 1,2 and 3 now we have all the way for the current value with all the current coins(p1, and p2)
        ///     5. put the result from (4.) in map[current value]
        /// repeat the process by going through all the value (1 to 200) as coins are added, evetually the map[200] would contain
        /// all the values.
        /// </summary>
        /// <returns></returns>
        static int the_1337_dynamic_programming()
        {
            var map = new int[201];
            var coins = new int[] { 1, 2, 5, 10, 20, 50, 100, 200 };
            var v = 0;
            var c = 0;
            var ways = 0;
            for (c = 0; c < 8; c++)
            {
                for (v = 1; v < 201; v++)
                {
                    if (v == coins[c])
                        map[v]++;

                    ways = v - coins[c];
                    if (ways >= 0)
                        map[v] = map[v] + map[ways];
                }
            }
            return map[200];
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
