using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler086csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The least value of M such that the number of solutions first exceeds one million: ");

            TestBenchSetup();
            TestBenchLoader(using_memoization_with_pythagorean_triple);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// basically for width(a), depth(b), and height(c)
        /// counter(a+1) = all the combo when a = a+1 + previously calculated counter(a), thats why previous and current is used
        /// Still, finding all the combo for a+1 is going to take a long time. so the generate_triplets() is used to limit
        /// the search range
        /// http://mathschallenge.net/index.php?section=problems&show=true&titleid=spider_fly_distance&full=true#solution
        /// </summary>
        static int using_memoization_with_pythagorean_triple()
        {
            var map = generate_triplets(5000);
            var a = 0;
            var b = 0;
            var c = 0;
            var bound = 0;
            var counter = 0;
            var previous = 0;
            var current = 0;
            while (counter < 1000000)
            {
                a++;
                current = 0;
                if (map.ContainsKey(a) == true)
                {
                    bound = a * 2;
                    foreach (var num in map[a])
                    {
                        if (num <= bound) // counts how many ways that (b+c) = num can be formed
                        {
                            if (num > a)
                            {
                                b = a;
                                c = num - a;
                            }
                            else
                            {
                                b = num - 1;
                                c = 1;
                            }
                            while (b >= c)
                            {
                                b--;
                                c++;
                                current++;
                            }
                        }
                    }
                }
                counter = current + previous;
                previous = counter;
            }
            return a;
        }

        static Dictionary<int, List<int>> generate_triplets(int max)
        {
            var map = new Dictionary<int, List<int>>();
            Action<int, int, int> trans = null;
            trans = (a, b, c) =>
            {
                if (a <= max)
                {
                    var m_a = a;
                    var m_b = b;
                    while (m_a <= max) // so here the multiples of the prime triplets are also included
                    {
                        if (map.ContainsKey(m_a) == false) // so depth can be either a or b
                            map[m_a] = new List<int>();
                        if (map.ContainsKey(m_b) == false) // so width can be either a or b
                            map[m_b] = new List<int>();
                        map[m_a].Add(m_b);
                        map[m_b].Add(m_a);
                        m_a += a;
                        m_b += b;
                    }
                    trans(a - 2 * b + 2 * c, 2 * a - b + 2 * c, 2 * a - 2 * b + 3 * c);
                    trans(a + 2 * b + 2 * c, 2 * a + b + 2 * c, 2 * a + 2 * b + 3 * c);
                    trans(-a + 2 * b + 2 * c, -2 * a + b + 2 * c, -2 * a + 2 * b + 3 * c);
                }
            };
            trans(3, 4, 5);
            return map;
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
