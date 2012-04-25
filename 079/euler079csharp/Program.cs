using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace euler079csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The shortest possible secret passcode: ");

            TestBenchSetup();
            TestBenchLoader(using_topological_sorting);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// first did it by hand with a chart which list of all predecessors for each number
        /// and found out this problem can be solved with topological_sorting.
        /// </summary>
        static int using_topological_sorting()
        {
            int number, n1, n2, n3;
            var map = new Dictionary<int, Node>();
            // create a directed graph using use class and dictionary to prevent duplicate
            foreach (var num in File.ReadAllLines("keylog.txt").Select(n => Convert.ToInt32(n)))
            {
                number = num;
                number = Math.DivRem(number, 10, out n3);
                number = Math.DivRem(number, 10, out n2);
                number = Math.DivRem(number, 10, out n1);

                if (map.ContainsKey(n1) == false)
                    map.Add(n1, new Node(n1));

                if (map.ContainsKey(n2) == false)
                    map.Add(n2, new Node(n2));

                if (map.ContainsKey(n3) == false)
                    map.Add(n3, new Node(n3));

                map[n1].outgoing[n2] = map[n2];
                map[n1].outgoing[n3] = map[n3];
                map[n2].incoming[n1] = map[n1];
                map[n2].outgoing[n3] = map[n3];
                map[n3].incoming[n1] = map[n1];
                map[n3].incoming[n2] = map[n2];
            }
            // using Topological sorting(http://en.wikipedia.org/wiki/Topological_sort)
            var s = map.Where(n => n.Value.incoming.Count == 0).Select(n => n.Value).ToList();
            var l = new List<Node>();
            while (s.Count != 0)
            {
                var n = s[0];
                s.RemoveAt(0);
                l.Add(n);
                foreach (var m in n.outgoing.Values)
                {
                    m.incoming.Remove(n.value);
                    if (m.incoming.Count == 0)
                        s.Add(m);
                }
            }
            // transform now sorted list "l" to result as a number
            var result = l[0].value;
            for (int i = 1; i < l.Count; i++)
            {
                result *= 10;
                result += l[i].value;
            }
            return result;
        }

        class Node
        {
            public int value;
            public Dictionary<int, Node> incoming;
            public Dictionary<int, Node> outgoing;
            public Node(int num)
            {
                value = num;
                incoming = new Dictionary<int, Node>();
                outgoing = new Dictionary<int, Node>();
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
