using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler090csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The numbers of distinct arrangements: ");

            TestBenchSetup();
            TestBenchLoader(using_Monte_Carlo_ish_simulation_to_generate_dices_and_count_distinct);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// basically using Fisher–Yates shuffle to "produce" dices and then use Trie to keep track of 
        /// distincts dices combinations in monte carlo-ish simulation runs until no more new "count"
        /// </summary>
        static int using_Monte_Carlo_ish_simulation_to_generate_dices_and_count_distinct()
        {
            var squares = new int[9, 2]
            {
                {0,1}, //0
                {0,4}, //1
                {0,9}, //2-
                {1,6}, //3-
                {2,5}, //4
                {3,6}, //5-
                {4,9}, //6-
                {6,4}, //7-
                {8,1}  //8
            };
            // dice related variables, seed1 and seed2 are used to be shuffle around to generate dices
            var seed1 = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var seed2 = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var d1 = new bool[10];
            var d2 = new bool[10];
            var d1d2 = new bool[20];
            var trie = new Trie();
            var rnd = new Random();
            var limit = 600000; // 600k sim runs seem to be good enough to stablely produce answer
            var count = 0;
            while (--limit > 0)
            {
                shuffle(seed1, rnd);
                shuffle(seed2, rnd);
                for (int i = 0; i < 10; i++)
                {
                    d1[i] = false;
                    d2[i] = false;
                }
                for (int i = 0; i < 6; i++)
                {
                    d1[seed1[i]] = true;
                    d2[seed2[i]] = true;
                }
                if (validate(squares, d1, d2) == true)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        d1d2[i] = d1[i];
                        d1d2[i + 10] = d2[i];
                    }
                    if (trie.Insert(d1d2) == false)
                    {
                        count++;
                    }
                }
            }
            // divide by 2 since order of dice1 and dice2 doesnt matter, took awhile to find out :(
            return count % 2 == 0 ? count / 2 : count / 2 + 1; 
        }

        /// <summary>
        /// bascially using the Fisher–Yates shuffle to "randomly" generate dice
        /// http://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
        /// </summary>
        static void shuffle(int[] dice, Random rnd)
        {
            for (int i = dice.Length; i > 1; i--)
            {
                var j = rnd.Next(i);
                var tmp = dice[j];
                dice[j] = dice[i - 1];
                dice[i - 1] = tmp;
            }
        }
        /// <summary>
        /// check to see if d1 and d2 can form all the square numbers and also satisfied the rule such that
        /// 6 and 9 can be treated interchangeably
        /// </summary>
        /// <param name="criteria">all of the square numbers below one-hundred</param>
        /// <param name="d1">1st dice</param>
        /// <param name="d2">2nd dice</param>
        /// <returns>true for pass</returns>
        static bool validate(int[,] criteria, bool[] d1, bool[] d2)
        {
            int a, b;
            for (int i = 0; i < 9; i++)
            {
                a = criteria[i, 0];
                b = criteria[i, 1];
                // ugly crap, but orwill
                if (i == 0 || i == 1 || i == 4 || i == 8)
                {
                    if (((d1[a] && d2[b]) || (d1[b] && d2[a])) == false)
                        return false;
                }
                else
                {
                    if (a == 6 || a == 9)
                    {
                        if ((((d1[6] || d1[9]) && d2[b]) || (d1[b] && (d2[6] || d2[9]))) == false)
                            return false;
                    }
                    else if (b == 6 || b == 9)
                    {
                        if (((d1[a] && (d2[6] || d2[9])) || ((d1[6] || d1[9]) && d2[a])) == false)
                            return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// to be used in Trie, with array[0 to 9] for dice1 and [10 to 19] for dice2
        /// </summary>
        class Node
        {
            public Node[] nodes;
            public Node()
            {
                nodes = new Node[20];
            }
            public bool Contain(int n)
            {
                return nodes[n] != null;
            }
            public Node GetChildren(int n)
            {
                return nodes[n];
            }
        }

        /// <summary>
        /// using Trie to save all the distinct 1st and 2nd dice combinations.
        /// </summary>
        class Trie
        {
            public Node root;
            public Trie()
            {
                root = new Node();
            }
            /// <summary>
            /// inserts element into trie
            /// </summary>
            /// <param name="d1d2">concatenation of dice1 and dice2</param>
            /// <returns> true if already existed, false otherwise</returns>
            public bool Insert(bool[] d1d2) //d1d2, concatenation of d1[0 to 9], d2[10 to 19]
            {
                bool existed = true;
                Func<int, Node, Node> insert = null;
                insert = (num, parent) =>
                    {
                        if (parent.Contain(num) == true)
                            return parent.GetChildren(num);
                        else
                        {
                            var newNode = new Node();
                            parent.nodes[num] = newNode;
                            existed = false;
                            return newNode;
                        }
                    };
                var node = root;
                for (int i = 0; i < d1d2.Length; i++)
                {
                    if (d1d2[i] == true)
                        node = insert(i, node);
                }
                return existed;
            }
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
