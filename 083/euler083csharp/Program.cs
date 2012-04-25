using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace euler083csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The minimal path sum, in matrix.txt: ");

            TestBenchSetup();
            TestBenchLoader(using_Dijkstra_s_algorithm_with_priority_queue);
            
            Console.WriteLine("press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// based on the pseudocode on wiki page(http://en.wikipedia.org/wiki/Dijkstra%27s_algorithm)
        /// </summary>
        static int using_Dijkstra_s_algorithm_with_priority_queue()
        {
            var len = 80;
            var node_map = new Node[len, len];
            var queue = new PriorityQueue(len * len);
            Node source = null;
            Node goal = null;
            var row = 0;
            var col = 0;
            foreach (var line in File.ReadAllLines("matrix.txt"))
            {
                col = 0;
                foreach (var num in line.Split(new char[] { ',' }))
                {
                    var node = new Node(Convert.ToInt32(num)) { row = row, col = col };
                    if (row == 0 && col == 0) source = node;
                    if (row == len - 1 && col == len - 1) goal = node;
                    // node map is mainly used to get neighbors
                    node_map[row, col] = node;
                    queue.Enqueue(node);
                    col++;
                }
                row++;
            }
            // set the source's distance to zero to kick start the process
            queue.Update(source, 0);
            // code for getting neighbor, using closure with the neighbor_list to make life a little easier
            var neighbor_list = new Node[4]; // 0:left 1:up 2:right 3:down
            Action<Node> prepare_neighbor_list = n =>
                {
                    neighbor_list[0] = n.col - 1 < 0 ? null : node_map[n.row, n.col - 1];
                    neighbor_list[1] = n.row - 1 < 0 ? null : node_map[n.row - 1, n.col];
                    neighbor_list[2] = n.col + 1 >= len ? null : node_map[n.row, n.col + 1];
                    neighbor_list[3] = n.row + 1 >= len ? null : node_map[n.row + 1, n.col];
                };
            var total = 0;
            while (queue.IsEmpty() == false)
            {
                var u = queue.DequeueMin();
                if (u.distance == int.MaxValue)
                    break; // all remaining vertices are inaccessible from source
                if (u == goal)
                {
                    while (u != null)
                    {
                        total += u.cost;
                        u = u.previous;
                    }
                    break;
                }
                // call this method before using neighbor_list array
                prepare_neighbor_list(u);
                foreach (var v in neighbor_list)
                {
                    if (v == null)
                        continue; // like when u is edge cell in the matrix

                    var alt = u.distance + u.cost + v.cost;
                    if (alt < v.distance)
                    {
                        v.previous = u;
                        queue.Update(v, alt);
                    }
                }
            }
            return total;
        }
        
        [DebuggerDisplay("d:{distance} c:{cost} i:{index}")]
        class Node
        {
            public int cost;
            public int distance = int.MaxValue; // so its like infinity 
            public int row;
            public int col;
            public int index; // so with index, searching the node in the array when update can be O(1)
            public Node previous = null;
            public Node(int cost) { this.cost = cost; }
        }
        /// <summary>
        /// a much better implementation of PriorityQueue this time :P.
        /// </summary>
        class PriorityQueue
        {
            Node[] array;
            int count;
            Func<int, int> Left = i => 2 * i;
            Func<int, int> Right = i => 2 * i + 1;

            public PriorityQueue(int size)
            {
                array = new Node[size + 1];
                count = 1; // rebase index to start with 1
            }

            public void Enqueue(Node new_node)
            {
                var i = count;
                array[i] = new_node;
                array[i].index = i;
                count++;
                while (i > 1 && array[i].distance < array[i / 2].distance)
                {
                    Swap(i / 2, i); // swap new_node's and its parent's position since new_node has smaller dist
                    i /= 2;
                }
            }
            public Node DequeueMin()
            {
                // save the minimum node to be returned later
                var node = array[1];
                node.index = -1;
                // swap the last node to root(array[1]) position, also updates count
                array[1] = array[--count];
                array[1].index = 1;
                array[count] = null;
                var i = 1;
                var min = 0;
                // bubble down the last_node til correct position
                while (i < count)
                {
                    min = i;
                    if (Left(i) < count && array[Left(i)].distance < array[min].distance)
                        min = Left(i);
                    if (Right(i) < count && array[Right(i)].distance < array[min].distance)
                        min = Right(i);

                    if (min != i)
                    {
                        Swap(i, min); // swap the node with smaller one of its children
                        i = min;
                    }
                    else
                        break;
                }
                return node;
            }
            public void Update(Node toUpdate, int newDist) // only works if node's distnace becomes smaller
            {
                toUpdate.distance = newDist;
                var i = toUpdate.index;
                while (i > 1 && array[i].distance < array[i / 2].distance)
                {
                    Swap(i / 2, i); // swap toUpdate node's and its parent's position since toUpdate node has smaller dist
                    i /= 2;
                }
            }
            void Swap(int index1, int index2)
            {
                var tmp = array[index1];
                array[index1] = array[index2];
                array[index1].index = index1;
                array[index2] = tmp;
                array[index2].index = index2;
            }
            public bool IsEmpty() { return count == 1; }
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
