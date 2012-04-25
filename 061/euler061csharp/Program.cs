using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler061csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The sum of the only ordered set of six cyclic 4-digit numbers: ");

            TestBenchSetup();
            TestBenchLoader(recursive_brute_force_to_find_and_construct_chain);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// basically generate all the number for each type thats within range(1k to 9999) and save them in map
        /// (the Node class is used to make life easier)
        /// then the recursive way is used to find the answer
        /// </summary>
        /// <returns></returns>
        static int recursive_brute_force_to_find_and_construct_chain()
        {
            var map = new List<Node>[]
                {
                    generate_numbers(triangle, 3),
                    generate_numbers(square, 4),
                    generate_numbers(pentagonal, 5),
                    generate_numbers(hexagonal, 6),
                    generate_numbers(heptagonal, 7),
                    generate_numbers(octagonal, 8)
                };
            // closure ftw!
            var map_index = map.Length - 1;
            var result = 0;
            var init = 0;
            var set = new HashSet<List<Node>>(); //since there can be only one number from each type
            var found = false; // stops the search, since there is only 1 such set stated by the problem
            Action<Node, int> probe = null;
            probe = (n, i) =>
            {
                foreach (var nodes in map)
                {
                    if (set.Contains(nodes))
                        continue;
                    set.Add(nodes);
                    foreach (var node in nodes)
                    {
                        if (n.n1n2 == node.n3n4)
                        {
                            node.uplink = n;
                            if (i == 0 && node.n1n2 == init)
                            {
                                var walker = node; //johnnie
                                while (walker != null)
                                {
                                    result += walker.number;
                                    walker = walker.uplink;
                                }
                                found = true;
                            }
                            if (found == false)
                                probe(node, i - 1);
                        }
                    }
                    set.Remove(nodes);
                }
            };
            foreach (var nodes in map)
            {
                set.Add(nodes);
                foreach (var node in nodes)
                {
                    init = node.n3n4;
                    if (found == false)
                        probe(node, map_index - 1);
                }
                set.Remove(nodes);
            }
            return result;
        }

        static List<Node> generate_numbers(Func<int, int> formula, int s)
        {
            var nodes = new List<Node>();
            var num = 0;
            var n = 0;
            int s1, s2;
            while ((num = formula(++n)) <= 9999)
            {
                if (num >= 1000)
                {
                    s1 = Math.DivRem(num, 100, out s2);
                    nodes.Add(new Node() { type = s, number = num, nth = n, n1n2 = s1, n3n4 = s2, uplink = null });
                }
            }
            return nodes;
        }

        [DebuggerDisplay("side={type} num={number} n-th={nth}")]
        class Node
        {
            public int type;
            public int number;
            public int nth;
            public int n1n2;
            public int n3n4;
            public Node uplink;
        }

        static Func<int, int> triangle = (n) => n * (n + 1) / 2;
        static Func<int, int> square = (n) => n * n;
        static Func<int, int> pentagonal = (n) => n * (3 * n - 1) / 2;
        static Func<int, int> hexagonal = (n) => n * (2 * n - 1);
        static Func<int, int> heptagonal = (n) => n * (5 * n - 3) / 2;
        static Func<int, int> octagonal = (n) => n * (3 * n - 2);

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
