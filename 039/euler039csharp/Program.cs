using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler039csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The number(<=1000) with most solution:");

            TestBenchSetup();
            TestBenchLoader(brute_force_every_possible_triplet_under_one_k);
            TestBenchLoader(calculate_every_prime_triplet_and_test_each_s_multuples);
            TestBenchLoader(someone_solution_from_q_discussion_thread);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// since a^2 + b^2 = c^2 and (a+b+c less or equal to 1000)
        /// that makes range for c from 4 to 500, calculate all c^2 and store in power_map,
        /// and c_map for fast Contains(...) operation(hashset is one of the fastest c# built-in collection
        /// for set operation and methods like Contains), iterate through every combination for
        /// a and b and if the a^2+b^2(power_map comes in handy) is in c_map, then it is a pythagorean triples.
        /// the rest is history~
        /// </summary>
        /// <returns></returns>
        static int brute_force_every_possible_triplet_under_one_k()
        {
            var len = 501;
            var power_map = new int[len];
            var c_map = new HashSet<int>();
            for (int i = 3; i < len; i++)
            {
                power_map[i] = (int)Math.Pow(i, 2);
                c_map.Add(power_map[i]);
            }
            int a, b, c;
            var map = new int[1001];
            for (a = 4; a < 500; a++)
            {
                for (b = 3; b < a; b++)
                {
                    c = power_map[a] + power_map[b];
                    if (c_map.Contains(c))
                    {
                        c = (int)Math.Sqrt(c);
                        if (a + b + c <= 1000)
                            map[a + b + c]++;
                    }
                }
            }
            var max = 0;
            var result = 0;
            for (int i = 0; i < 1001; i++)
            {
                if (map[i] > max)
                {
                    max = map[i];
                    result = i;
                }
            }
            return result;
        }

        /// <summary>
        /// using method IV from http://en.wikipedia.org/wiki/Formulas_for_generating_Pythagorean_triples
        /// to generate every prime triplet which no side(a,b,c) can be greater than 500
        /// (For the resulting triple to be primitive, u and v must be co-prime and u must be odd)
        /// thus, u or v cannot exceed 23(for u:  u^2+2uv~500, for v: 2v^2+2uv~500)
        /// </summary>
        /// <returns></returns>
        static int calculate_every_prime_triplet_and_test_each_s_multuples()
        {
            var map = new int[1001];
            int u, v, g, h, i, a, b, c, p, m;
            for (u = 23; u > 0; u -= 2)
            {
                for (v = 23; v > 0; v--)
                {
                    if (find_GCD(u, v) != 1)
                        continue;

                    g = u * u;
                    h = 2 * (v * v);
                    i = 2 * u * v;
                    a = g + i;
                    b = h + i;
                    c = g + h + i;
                    p = a + b + c;
                    if (p <= 1000)
                    {
                        m = 1;
                        while (p * m <= 1000)
                        {
                            map[p * m]++;
                            m++;
                        }
                    }
                }
            }
            var max = 0;
            var result = 0;
            for (i = 0; i < 1001; i++)
            {
                if (map[i] > max)
                {
                    max = map[i];
                    result = i;
                }
            }
            return result;
        }

        /// <summary>
        /// Kvom's java solution(from this question's discussion thread)
        /// http://projecteuler.net/index.php?section=forum&id=39&page=4
        /// </summary>
        /// <returns></returns>
        static int someone_solution_from_q_discussion_thread()
        {
            int total = 0;
            int maxp = 0;
            for (int p = 12; p <= 1000; p += 2)
            {
                int count = 0;
                for (int a = 1; a < p / 2; a++)
                {
                    for (int b = p / 2; b > a; b--)
                    {
                        if ((a * b) % 12 != 0)
                            continue;
                        int c = p - a - b;
                        if ((c * c) != (a * a + b * b))
                            continue;
                        count++;
                    }
                }
                if (count > total)
                {
                    total = count;
                    maxp = p;
                }
            }
            return maxp;
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
