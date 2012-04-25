using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace euler081csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The minimal path sum, in matrix.txt: ");

            TestBenchSetup();
            TestBenchLoader(half_ass_a_star_and_Dijkstra_tarball);
            TestBenchLoader(using_DP_with_memoization);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        static int using_DP_with_memoization()
        {
            var len = 80;
            var map = new int[len, len];
            var cache = new int[len, len];
            var row = 0;
            foreach (var line in File.ReadAllLines("matrix.txt"))
            {
                var col = 0;
                foreach (var num in line.Split(new char[] { ',' }))
                {
                    cache[row, col] = -1;
                    map[row, col] = Convert.ToInt32(num);
                    col++;
                }
                row++;
            }
            Func<int, int, int> min_path = null;
            min_path = (r, c) =>
                {
                    if (cache[r, c] != -1)
                        return cache[r, c];
                    else if (r == len - 1 && c == len - 1)
                        return (cache[r, c] = map[r, c]);
                    else if (r == len - 1)
                        return (cache[r, c] = map[r, c] + min_path(r, c + 1));
                    else if (c == len - 1)
                        return (cache[r, c] = map[r, c] + min_path(r + 1, c));
                    else
                        return (cache[r, c] = map[r, c] + Math.Min(min_path(r, c + 1), min_path(r + 1, c)));
                };
            return min_path(0, 0);
        }

        /// <summary>
        /// was trying to implement A* search algorithm(http://en.wikipedia.org/wiki/A*_search_algorithm)
        /// but i cant think of a meaningful h(x) (heuristic_estimate_of_distance), so i guess its more of 
        /// Dijkstra's algorithm, but yet the code structure followed the pseudo code of A* search on wiki page.
        /// some crucial mechanism are not implemented (namely the tentative_g_score smaller than g_score[y])
        /// </summary>
        static int half_ass_a_star_and_Dijkstra_tarball()
        {
            var len = 80;
            var map = new int[len, len];
            var row = 0;
            foreach (var line in File.ReadAllLines("matrix.txt"))
            {
                var col = 0;
                line.Split(new char[] { ',' }).Select(c => Convert.ToInt32(c)).ToList().ForEach(n => map[row, col++] = n);
                row++;
            }
            var start = new Node(0, 0, map[0, 0]);
            var goal = new Node(len - 1, len - 1, map[len - 1, len - 1]);
            var openset = new PriorityQueue();

            start.g_score = 0;
            start.h_score = heuristic_estimate_of_distance(start, goal, map);
            start.f_score = start.h_score;
            openset.Push(start);

            while (openset.IsEmpty() == false)
            {
                var x = openset.PopMinimum();
                if (x.m_row == goal.m_row && x.m_col == goal.m_col)
                {
                    var cost = 0;
                    while (x != null)
                    {
                        cost += x.m_value;
                        x = x.came_from;
                    }
                    return cost;
                }

                foreach (var y in get_neighbors(x, map, len))
                {
                    if (openset.Exists(y) == false)
                    {
                        y.came_from = x;
                        y.g_score = x.g_score + x.m_value + y.m_value;
                        y.h_score = heuristic_estimate_of_distance(y, goal, map);
                        y.f_score = y.g_score + y.h_score;
                        openset.Push(y);
                    }
                }
            }
            return 0;
        }

        /// <summary>
        /// it has to be admissible heuristic(http://en.wikipedia.org/wiki/A*_search_algorithm)
        /// great! if i already know such way, wouldnt it be faster if i just use that instead. o.O
        /// </summary>
        static int heuristic_estimate_of_distance(Node x, Node y, int[,] map)
        {
            return x.m_value + y.m_value; // jetpack version, x can just jump to y
        }

        static List<Node> get_neighbors(Node node, int[,] map, int len)
        {
            var neighbors = new List<Node>();
            var row = node.m_row;
            var col = node.m_col;

            if (col + 1 < len)
                neighbors.Add(new Node(row, col + 1, map[row, col + 1]));
            if (row + 1 < len)
                neighbors.Add(new Node(row + 1, col, map[row + 1, col]));

            return neighbors;
        }

        [DebuggerDisplay("{m_value} ({m_row},{m_col}) f:{f_score}")]
        class Node : IComparable<Node>
        {
            public int m_value;
            public int m_row;
            public int m_col;
            public int g_score;
            public int h_score;
            public int f_score;
            public Node came_from;
            public Node() { g_score = 0; h_score = 0; f_score = 0; came_from = null; }
            public Node(int row, int col, int value) : this() { m_row = row; m_col = col; m_value = value; }
            public int CompareTo(Node node) { return f_score > node.f_score ? 1 : f_score < node.f_score ? -1 : 0; }
        }

        /// <summary>
        /// it is like a very ghetto priority queue implementation using minimum-at-top binary heap, which is used
        /// by the A* Search way
        /// </summary>
        class PriorityQueue
        {
            List<Node> m_store;
            int[,] m_nodeMap; //this is used for faster Exists look up
            public PriorityQueue()
            {
                m_store = new List<Node>() { null }; // filler, 1st element is not used
                m_nodeMap = new int[80, 80];
            }
            public void Push(Node item)
            {
                m_store.Add(item);
                m_nodeMap[item.m_row, item.m_col] = 1;
                var index = m_store.Count - 1;
                var parent_index = 0;
                while (index != 1)
                {
                    parent_index = index / 2;
                    var parent = m_store[parent_index];
                    if (item.CompareTo(parent) < 0)
                    {
                        m_store[parent_index] = item;
                        m_store[index] = parent;
                        index = parent_index;
                    }
                    else
                        break;
                }
            }
            public Node PopMinimum()
            {
                var removed = m_store[1];
                var len = m_store.Count - 1;
                var current = 1;
                var left = 1;
                var right = 1;
                var min = 1;
                m_store[1] = m_store[len];
                do
                {
                    left = 2 * current;
                    right = 2 * current + 1;
                    min = left <= len && m_store[left].CompareTo(m_store[current]) < 0 ? left : min;
                    min = right <= len && m_store[right].CompareTo(m_store[min]) < 0 ? right : min;

                    if (min != current)
                    {
                        var tmp = m_store[current];
                        m_store[current] = m_store[min];
                        m_store[min] = tmp;
                        current = min;
                    }
                    else
                        break;
                } while (true);
                m_nodeMap[removed.m_row, removed.m_col] = -1;
                m_store.RemoveAt(len);
                return removed;
            }
            public bool IsEmpty() { return m_store.Count == 1; }
            public bool Exists(Node node) { return m_nodeMap[node.m_row, node.m_col] == 1; }
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
