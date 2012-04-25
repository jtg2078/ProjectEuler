using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace euler107csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The number of subset pairs that needed to be tested for equality: ");

            TestBenchSetup();
            TestBenchLoader(using_Kruskals_algorithm_with_PriorityQueue_and_UnionFind);
            TestBenchLoader(using_Prims_algorithm_with_adjacency_matrix);

            Console.WriteLine("press any key to exit");
            Console.ReadKey();
        }

        static int using_Kruskals_algorithm_with_PriorityQueue_and_UnionFind()
        {
            var len = 40;
            var sum = 0;
            var vertex1 = 0;
            var vertex2 = 0;
            var total = 0;
            var edges_queue = new PriorityQueue<Edge>(len * len);
            var vertices = new List<Vertex>();
            var components = new List<UnionFindSet<Vertex>>();
            var uf = new UnionFind<Vertex>();
            for (int i = 0; i < len; i++)
            {
                var vertex = new Vertex(i);
                var node = new UnionFindSet<Vertex>(vertex);
                uf.MakeSet(node);
                components.Add(node);
                vertices.Add(vertex);
            }
            foreach (var line in File.ReadAllLines(@"..\..\network.txt"))
            {
                foreach (var num in line.Split(new char[] { ',' }))
                {
                    if (num != "-")
                    {
                        var edge = new Edge(vertices[vertex1], vertices[vertex2], Convert.ToInt32(num));
                        var node = new PriorityQueueNode<Edge>(edge.weight, edge);
                        edges_queue.Enqueue(node);
                        total += edge.weight;
                    }
                    vertex2++;
                }
                vertex1++;
                vertex2 = 0;
            }
            // based on the pseudo code from http://en.wikipedia.org/wiki/Kruskal%27s_algorithm
            while (true)
            {
                var min_edge = edges_queue.DequeueMin();

                if (min_edge == null)
                    return total / 2 - sum; // since the network.txt is read as adjacency matrix, has the property of being reflective

                if (uf.Find(components[min_edge.item.vertex1.ID]) != uf.Find(components[min_edge.item.vertex2.ID]))
                {
                    uf.Union(components[min_edge.item.vertex1.ID], components[min_edge.item.vertex2.ID]);
                    sum += min_edge.item.weight;
                }
            }
        }

        /// <summary>
        /// detail on how the algorithm works
        /// http://www.google.com/url?sa=t&source=web&cd=5&ved=0CCgQFjAE&url=http%3A%2F%2Fwww.kentfurthermaths.org%2Fms-powerpoint%2FD1%2FMatrixFormofPrimsAlgorithm.ppt&rct=j&q=prim%20matrix&ei=3nykTJCLN4ysvgOOi72lDQ&usg=AFQjCNEMwrRowpYK3oK9WKbvWt2h9wvdNg&cad=rja
        /// http://www.ams.org/samplings/feature-column/fcarc-trees
        /// http://www.ics.uci.edu/~eppstein/161/960206.html
        /// http://students.ceid.upatras.gr/~papagel/project/prim.htm
        /// http://www.youtube.com/watch?v=sl6W3_Q4HZo
        /// </summary>
        static int using_Prims_algorithm_with_adjacency_matrix()
        {
            var len = 40;
            var graph = new int[len, len];
            int vertex1 = 0, vertex2 = 0, total = 0, weight = 0;
            // populate graph matrix, which is to be used as adjacency matrix
            foreach (var line in File.ReadAllLines(@"..\..\network.txt"))
            {
                foreach (var num in line.Split(new char[] { ',' }))
                {
                    if (num != "-")
                    {
                        weight = Convert.ToInt32(num);
                        graph[vertex1, vertex2] = weight;
                        total += weight;
                    }
                    else
                    {
                        graph[vertex1, vertex2] = -1;
                    }
                    vertex2++;
                }
                vertex1++;
                vertex2 = 0;
            }
            var start_vertex = 0; // pick the starting vertex as the 1st vertex
            var arrowed = new int[len]; // used to keep track arrowed columns
            for (int i = 0; i < len; i++) // act as deleting the whole row
            {
                graph[start_vertex, i] = -1;
            }
            arrowed[start_vertex] = 1;
            var sum = 0;
            while (true)
            {
                var min_weight = int.MaxValue;
                var to_circle = -1;
                for (int i = 0; i < len; i++) // find the min weight among all the arrowed columns
                {
                    if (arrowed[i] == 1)
                    {
                        for (int j = 0; j < len; j++)
                        {
                            weight = graph[j, i];
                            if (weight != -1 && weight < min_weight)
                            {
                                min_weight = weight;
                                to_circle = j;
                            }
                        }
                    }
                }
                if (min_weight == int.MaxValue) // all the element in the matrix are marked, which means the end
                    return total / 2 - sum;
                sum += min_weight;
                for (int i = 0; i < len; i++) // act as deleting the whole row
                {
                    graph[to_circle, i] = -1;
                }
                arrowed[to_circle] = 1; // add the circled's row to arrowed
            }
        }

        class Vertex
        {
            public int ID;
            public Vertex(int id)
            {
                this.ID = id;
            }
        }

        class Edge
        {
            public int weight;
            public Vertex vertex1;
            public Vertex vertex2;
            public Edge(Vertex v1, Vertex v2, int weight)
            {
                this.weight = weight;
                this.vertex1 = v1;
                this.vertex2 = v2;
            }
        }

        public class UnionFindSet<T>
        {
            public UnionFindSet<T> Parent;
            public T Item;
            public int Rank;
            public UnionFindSet(T item)
            {
                Item = item;
            }
        }

        /// <summary>
        /// using wikipedia's implementation
        /// http://en.wikipedia.org/wiki/Disjoint-set_data_structure
        /// </summary>
        class UnionFind<T>
        {
            public void MakeSet(UnionFindSet<T> x)
            {
                x.Parent = x;
                x.Rank = 0;
            }
            public UnionFindSet<T> Find(UnionFindSet<T> x)
            {
                return x.Parent == x ? x : x.Parent = Find(x.Parent);
            }
            public void Union(UnionFindSet<T> x, UnionFindSet<T> y)
            {
                var xRoot = Find(x);
                var yRoot = Find(y);
                if (xRoot.Rank > yRoot.Rank)
                    yRoot.Parent = xRoot;
                else if (yRoot.Rank > xRoot.Rank)
                    xRoot.Parent = yRoot;
                else if (xRoot != yRoot)
                {
                    yRoot.Parent = xRoot;
                    xRoot.Rank++;
                }
            }
        }

        class PriorityQueueNode<T>
        {
            public int value;
            public T item;
            public int index; // so with index, searching the node in the array when update can be O(1)
            public PriorityQueueNode<T> previous = null;
            public PriorityQueueNode(int value, T item) { this.value = value; this.item = item; }
        }

        class PriorityQueue<T>
        {
            PriorityQueueNode<T>[] array;
            int count;
            Func<int, int> Left = i => 2 * i;
            Func<int, int> Right = i => 2 * i + 1;
            public PriorityQueue(int size)
            {
                array = new PriorityQueueNode<T>[size + 1];
                count = 1; // rebase index to start with 1
            }
            public void Enqueue(PriorityQueueNode<T> new_node)
            {
                var i = count;
                array[i] = new_node;
                array[i].index = i;
                count++;
                while (i > 1 && array[i].value < array[i / 2].value)
                {
                    Swap(i / 2, i); // swap new_node's and its parent's position since new_node has smaller dist
                    i /= 2;
                }
            }
            public PriorityQueueNode<T> DequeueMin()
            {
                // save the minimum node to be returned later
                var node = array[1];
                if (node == null) // empty queue
                    return null;
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
                    if (Left(i) < count && array[Left(i)].value < array[min].value)
                        min = Left(i);
                    if (Right(i) < count && array[Right(i)].value < array[min].value)
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
            public void Update(PriorityQueueNode<T> toUpdate, int newValue) // only works if node's value becomes smaller
            {
                toUpdate.value = newValue;
                var i = toUpdate.index;
                while (i > 1 && array[i].value < array[i / 2].value)
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
