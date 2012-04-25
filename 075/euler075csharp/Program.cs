using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler075csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The values of L ≤ 1,500,000 can exactly one integer sided right angle triangle be formed: ");

            TestBenchSetup();
            TestBenchLoader(calculate_every_prime_triplet_and_test_each_s_multuples);
            TestBenchLoader(calculate_every_prime_triplet_using_parent_child_relationships);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// using method IV from http://en.wikipedia.org/wiki/Formulas_for_generating_Pythagorean_triples
        /// to generate every prime triplet which no side(a,b,c) can be greater than 1.5m /2 = 750000
        /// (For the resulting triple to be primitive, u and v must be co-prime and u must be odd)
        /// thus, u or v cannot exceed 865(for u:  u^2+2uv~750000, for v: 2v^2+2uv~750000)
        /// </summary>
        /// <returns></returns>
        static int calculate_every_prime_triplet_and_test_each_s_multuples()
        {
            var max = 1500000;
            var bound = (int)Math.Sqrt(max / 2 + 1) - 1;

            var map = new int[max + 1];
            int u, v, g, h, i, a, b, c, p, m;
            for (u = bound; u > 0; u -= 2)
            {
                for (v = bound; v > 0; v--)
                {
                    if (find_GCD(u, v) == 1)
                    {
                        g = u * u;
                        h = 2 * (v * v);
                        i = 2 * u * v;
                        a = g + i;
                        b = h + i;
                        c = g + h + i;
                        p = a + b + c;
                        m = p;
                        while (m <= max) // so here the multiples of the prime triplets are also included
                        {
                            map[m]++;
                            m += p;
                        }
                    }
                }
            }
            var result = 0;
            for (i = 1; i <= max; i++)
            {
                if (map[i] == 1)
                    result++;
            }
            return result;
        }

        /// <summary>
        /// using Parent/child relationships ways to generate/tranform each triplet(a,b,c) into three more triplets
        /// starts with 3,4,5. stops when a+b+c > limit(1.5m). since it doesnt need to check for GCD, its faster than
        /// first method(not sure if the compiler would turn this into loop rather than recursive calls)
        /// </summary>
        static int calculate_every_prime_triplet_using_parent_child_relationships()
        {
            var max = 1500000;
            var map = new int[max + 1];
            Action<int, int, int> trans = null;
            trans = (a, b, c) =>
                {
                    var p = a + b + c;
                    if (p <= max)
                    {
                        var m = p;
                        while (m <= max) // so here the multiples of the prime triplets are also included
                        {
                            map[m]++;
                            m += p;
                        }
                        trans(a - 2 * b + 2 * c, 2 * a - b + 2 * c, 2 * a - 2 * b + 3 * c);
                        trans(a + 2 * b + 2 * c, 2 * a + b + 2 * c, 2 * a + 2 * b + 3 * c);
                        trans(-a + 2 * b + 2 * c, -2 * a + b + 2 * c, -2 * a + 2 * b + 3 * c);
                    }
                };
            trans(3, 4, 5);
            var result = 0;
            for (int i = 1; i <= max; i++)
            {
                if (map[i] == 1)
                    result++;
            }
            return result;
        }

        /// <summary>
        /// use Euclidean algorithm to find GCD, returns 1 if x and y are coprime
        /// http://en.wikipedia.org/wiki/Euclidean_algorithm
        /// </summary>
        static int find_GCD(int x, int y)
        {
            while (x > 0)
            {
                y = y % x;
                y = y + x; // the next three lines is just swapping x and y
                x = y - x;
                y = y - x;
            }
            return y;
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
