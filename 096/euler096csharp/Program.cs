using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace euler096csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The sum of the 3-digit numbers found in the top left corner of each solution grid: ");

            TestBenchSetup(); 
            TestBenchLoader(using_DLX_alogorithm_and_code_from_sfabriz);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
        /// <summary>
        /// read about knuth's DLX algorithm which can be used to solve sudoku problem long time ago
        /// found this awesome link http://www.osix.net/modules/article/?id=792 and code which implemented the algorithm
        /// in c#. the following is pretty much direct copy of the code from the site, which helped me understand how
        /// the algorithm works tremendously!
        /// http://en.wikipedia.org/wiki/Knuth%27s_Algorithm_X
        /// http://en.wikipedia.org/wiki/Dancing_Links
        /// http://www.stolaf.edu/people/hansonr/sudoku/exactcovermatrix.htm
        /// http://en.wikipedia.org/wiki/Exact_cover#Sudoku
        /// </summary>
        static int using_DLX_alogorithm_and_code_from_sfabriz()
        {
            var index = 0;
            var accum = new List<string>();
            var result = 0;
            foreach (var line in File.ReadAllLines("sudoku.txt"))
            {
                if (index % 10 == 0)
                {
                    if (accum.Count != 0)
                    {
                        result += solve(accum.ToArray());
                        accum.Clear();
                    }
                }
                else
                    accum.Add(line);
                index++;
            }
            if (accum.Count != 0)
                result += solve(accum.ToArray()); // for last puzzle grid
            return result;
        }
        static int solve(string[] grid)
        {
            // ---- define and initialize variables ----
            var dimension = 9;
            var block_dimension = 3;
            var header = new Header(0);
            var solution = new Stack<Node>();
            var total_solutions = 0;
            var row_count = dimension * dimension * dimension;
            var col_count = dimension * dimension * 4;
            // ---- helper functions ----
            Action<Header> CoverColumn = c =>
            {
                // header unplugging
                c.Left.Right = c.Right;
                c.Right.Left = c.Left;
                // nodes unplugging
                Node i = c.Down;
                Node j = null;
                while (i != c)
                {
                    j = i.Right;
                    while (j != i)
                    {
                        j.Up.Down = j.Down;
                        j.Down.Up = j.Up;
                        j.Header.Size--;
                        j = j.Right;
                    }
                    i = i.Down;
                }
            };
            Action<Header> UncoverColumn = c =>
            {
                // nodes plugging
                Node i = c.Up;
                Node j = null;
                while (i != c)
                {
                    j = i.Left;
                    while (j != i)
                    {
                        j.Up.Down = j;
                        j.Down.Up = j;
                        j.Header.Size++;
                        j = j.Left;
                    }
                    i = i.Up;
                }
                // header plugging
                c.Left.Right = c;
                c.Right.Left = c;
            };
            Func<Header> FindMin = () =>
            {
                int min = int.MaxValue;
                Header ret = null;
                Header j = (Header)header.Right;
                while (j != header)
                {
                    if (j.Size < min)
                    {
                        min = j.Size;
                        ret = j;
                    }
                    j = (Header)j.Right;
                }
                return ret;
            };
            // ---- sets up the skeleton matrix ----
            var rows = new Node[row_count];
            var columns = new Header[col_count];
            for (int i = 0; i < col_count; i++) // columns
            {
                columns[i] = new Header(i);
                columns[i].Left = header.Left;
                columns[i].Right = header;
                header.Left.Right = columns[i];
                header.Left = columns[i];
            }
            for (int i = 0; i < row_count; i++) // rows
            {
                rows[i] = new Node();
            }
            // ---- populate the matrix row by row with initialized node ----
            var b = 0;
            var pos = new int[4];
            var row = 0;
            for (int r = 0; r < dimension; r++)
            {
                for (int c = 0; c < dimension; c++)
                {
                    // set the block
                    b = (r / block_dimension) * block_dimension + (c / block_dimension);
                    for (int d = 0; d < dimension; d++)
                    {
                        pos[0] = r * dimension + c;
                        pos[1] = dimension * (dimension + r) + d;
                        pos[2] = dimension * (c + 2 * dimension) + d;
                        pos[3] = dimension * (b + 3 * dimension) + d;
                        row = r * dimension * dimension + c * dimension + d;
                        Node first = new Node(row);
                        first.Header = columns[pos[0]];
                        first.Header.Size++;
                        first.Down = columns[pos[0]];
                        first.Up = columns[pos[0]].Up;
                        columns[pos[0]].Up.Down = first;
                        columns[pos[0]].Up = first;
                        rows[row] = first;
                        for (int i = 1; i < pos.Length; i++)
                        {
                            Node t = new Node(row);
                            // left - right
                            t.Left = first.Left;
                            t.Right = first;
                            first.Left.Right = t;
                            first.Left = t;
                            // up - down
                            t.Down = columns[pos[i]];
                            t.Up = columns[pos[i]].Up;
                            columns[pos[i]].Up.Down = t;
                            columns[pos[i]].Up = t;
                            // head
                            t.Header = columns[pos[i]];
                            t.Header.Size++;
                        }
                    }
                }
            }
            // ---- setup the matrix with given numbers ----
            int[,] g = new int[dimension, dimension];
            for (int i = 0; i < dimension; i++)
            {
                for (int j = 0; j < dimension; j++)
                {
                    g[i, j] = int.Parse(grid[i][j].ToString());
                }
            }
            var index = 0;
            for (int r = 0; r < dimension; r++)
            {
                for (int c = 0; c < dimension; c++)
                {
                    if (g[r, c] != 0)
                    {
                        index = r * dimension * dimension + c * dimension + (g[r, c] - 1);
                        Node n = rows[index];
                        solution.Push(n);
                        do
                        {
                            CoverColumn(n.Header);
                            n = n.Right;
                        } while (n != rows[index]);
                    }
                }
            }
            // ---- solve the exact cover with recursive method
            var first_three_num = 0;
            Action SolveRecursively = null;
            SolveRecursively = () =>
                {
                    if (header.Right == header)
                    {
                        total_solutions++;
                        index = 0;
                        var s = new int[solution.Count];
                        foreach (var item in solution)
                        {
                            s[index++] = item.Row;
                        }
                        Array.Sort(s);
                        for (int i = 0; i < s.Length; i++)
                        {
                            s[i] = s[i] % dimension;
                        }
                        first_three_num = (s[0] + 1) * 100 + (s[1] + 1) * 10 + (s[2] + 1);
                        return;
                    }
                    Header c = FindMin();

                    if (c.Size == 0) return; // not a good solution

                    CoverColumn(c);
                    Node r = c.Down;
                    Node j = null;
                    while (r != c)
                    {
                        solution.Push(r);
                        j = r.Right;
                        while (j != r)
                        {
                            CoverColumn(j.Header);
                            j = j.Right;
                        }
                        SolveRecursively();
                        r = (Node)solution.Pop();
                        c = r.Header;
                        j = r.Left;
                        while (j != r)
                        {
                            UncoverColumn(j.Header);
                            j = j.Left;
                        }
                        r = r.Down;
                    }
                    UncoverColumn(c);
                };
            SolveRecursively(); // invoke the method
            // ---- done ----
            return first_three_num;
        }
        class Header : Node
        {
            public int Size;
            public int Name;
            public Header(int name)
            {
                this.Name = name;
                this.Size = 0;
                this.Header = this;
            }
        }
        class Node
        {
            public Node Right;
            public Node Up;
            public Node Left;
            public Node Down;
            public int Row;
            public Header Header;
            public Node() : this(-1, null) { }
            public Node(int row) : this(row, null) { }
            public Node(int row, Header header)
            {
                this.Right = this.Up = this.Left = this.Down = this;
                this.Row = row;
                this.Header = header;
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
