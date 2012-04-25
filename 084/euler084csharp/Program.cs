using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace euler084csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The six-digit modal string: ");

            TestBenchSetup();
            TestBenchLoader(plain_simulation_with_one_mil_rolls);

            Console.WriteLine("press any key to exit");
            Console.ReadKey();
        }

        static string plain_simulation_with_one_mil_rolls()
        {
            var board = new double[40];
            var count = new int[40];

            // -1: nothing
            var chest = new int[] { 0, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
            // -88: railroad -99: utility -3: go back three
            var chance = new int[] { 0, 10, 11, 24, 39, 5, -88, -88, -99, -3, -1, -1, -1, -1, -1, -1 };

            var dice1 = 0;
            var dice2 = 0;
            var rnd = new Random();
            var current = 0;

            Shuffle(chest, rnd);
            Shuffle(chance, rnd);

            var moves = 1000000;
            var double_counter = 0;
            var dice_side = 4;

            for (int i = 0; i < moves; i++)
            {
                dice1 = rnd.Next(dice_side) + 1;
                dice2 = rnd.Next(dice_side) + 1;
                current += (dice1 + dice2);

                if (current > 39)
                    current -= 40;

                if (dice1 == dice2)
                    double_counter++;
                else
                    double_counter = 0;

                if (double_counter == 3 || current == 30)
                    current = 10;

                // chest
                if (current == 2 || current == 17 || current == 33)
                {
                    var card = chest[rnd.Next(16)];
                    if (card != -1)
                        current = card;
                }

                // chance
                if (current == 7 || current == 22 || current == 36)
                {
                    var card = chance[rnd.Next(16)];
                    if (card != -1)
                    {
                        switch (card)
                        {
                            case -88:
                                current = NextRailWay(current);
                                break;
                            case -99:
                                current = NextUtility(current);
                                break;
                            case -3:
                                current -= 3;
                                if (current == 33)
                                {
                                    card = chest[rnd.Next(16)];
                                    if (card != -1)
                                        current = card;
                                }
                                break;
                            default:
                                current = card;
                                break;
                        };
                    }
                }

                if (current == 10)
                    double_counter = 0;

                count[current]++;
            }

            return count.Select((p, i) => new { index = i, count = p }).OrderByDescending(p => p.count).Take(3)
                .Aggregate(new StringBuilder(), (s, n) => s.Append(n.index == 0 ? "00" : n.index.ToString())).ToString();
        }

        static void Shuffle(int[] deck, Random rnd)
        {
            for (int i = deck.Length; i > 1; i--)
            {
                var j = rnd.Next(i);
                var tmp = deck[j];
                deck[j] = deck[i - 1];
                deck[i - 1] = tmp;
            }
        }

        static int DrawCard(int[] deck)
        {
            var card = deck[0];
            for (int i = 1; i < deck.Length; i++)
            {
                deck[i - 1] = deck[i];
            }
            deck[deck.Length - 1] = card;
            return card;
        }

        static int NextRailWay(int current)
        {
            if (current == 7)
                return 15;
            else if (current == 22)
                return 25;
            else // current == 36
                return 5;
        }

        static int NextUtility(int current)
        {
            if (current == 22)
                return 28;
            else
                return 12;
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
        static void TestBenchLoader(Func<string> test_method)
        {
            stopwatch.Reset();
            stopwatch.Start();
            string result = string.Empty;
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
