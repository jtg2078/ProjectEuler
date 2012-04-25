using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace euler094csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The sum of the perimeters of all almost equilateral triangles: ");

            TestBenchLoader(multithreaded_brute_force_test_all_possible_bases);
            TestBenchSetup();
            TestBenchLoader(generate_pythagorean_triplets_to_construct_and_test_triangles);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// basically test all the base from 6 to 1billion/3 to see if a triangle can be formed that matched problem's criteria
        /// this way is painfully slow. even the multithreaded version is extremely slow
        /// </summary>
        static long multithreaded_brute_force_test_all_possible_bases()
        {
            var THREAD_COUNT = Environment.ProcessorCount - 1;
            var results = new long[THREAD_COUNT];
            var wait_handles = Enumerable.Range(0, THREAD_COUNT).Select(w => new AutoResetEvent(false)).ToArray();

            for (int i = 0; i < THREAD_COUNT; i++)
            {
                ThreadPool.QueueUserWorkItem(o =>
                    {
                        var nth = (int)o;
                        var off_set = nth * 2;
                        var steps = THREAD_COUNT * 2;
                        var max = 1000000000 / 3;
                        long a, b, h, hh, partial_result;
                        for (a = 6 + off_set, partial_result = 0; a < max; a += steps)
                        {
                            b = a + 1;
                            hh = b * b - a * a / 4;
                            h = (long)Math.Sqrt(hh);

                            if (h * h == hh)
                                partial_result += (b + b + a);

                            b = a - 1;
                            hh = b * b - a * a / 4;
                            h = (long)Math.Sqrt(hh);
                            if (h * h == hh)
                                partial_result += (b + b + a);
                        }
                        results[nth] = partial_result;
                        wait_handles[nth].Set();
                    }, i);
            }

            long result = 0;
            for (int i = 0; i < THREAD_COUNT; i++)
            {
                wait_handles[i].WaitOne();
                result += results[i];
            }
            return result;
        }

        /// <summary>
        /// basically using the Parent/child relationships method to generate pythagorean triplets, which is a
        /// right triangle, and two of the same triangle can be used to form a new triangle, which is to be tested to
        /// see if it is an "almost equilateral triangle".
        /// the key point is that, since the Parent/child relationships method would create 3 more triplets from a 
        /// parent triplet. By only using the parent triplets can be used to form "almost equilateral triangle" to generate next set
        /// of children triplets, it becomes a lot more manageable since the upper bound is 1 billion.
        /// as for why this works, i am not sure o.0
        /// </summary>
        static long generate_pythagorean_triplets_to_construct_and_test_triangles()
        {
            long total = 0;
            Action<int, int, int> trans = null;
            trans = (a, b, c) =>
            {
                var t_base = 0;
                var t_side = 0;
                var is_valid = false;
                // the following if-else(s) are used to find out which of a,b,c is the smallest(for base), 
                // and largest(for side)
                if (a <= b && a <= c)
                {
                    t_base = 2 * a;
                    if (b <= c)
                    {
                        // test the condition of "the third differs by no more than one unit"
                        is_valid = (c == t_base + 1 || c == t_base - 1); 
                        t_side = c;
                    }
                    else
                    {
                        is_valid = (b == t_base + 1 || b == t_base - 1);
                        t_side = b;
                    }
                }
                else if (b <= a && b <= c)
                {
                    t_base = 2 * b;
                    if (a <= c)
                    {
                        is_valid = (c == t_base + 1 || c == t_base - 1);
                        t_side = c;
                    }
                    else
                    {
                        is_valid = (a == t_base + 1 || a == t_base - 1);
                        t_side = a;
                    }
                }
                else
                {
                    t_base = 2 * c;
                    if (a <= b)
                    {
                        is_valid = (b == t_base + 1 || b == t_base - 1);
                        t_side = b;
                    }
                    else
                    {
                        is_valid = (a == t_base + 1 || a == t_base - 1);
                        t_side = a;
                    }
                }
                if (is_valid == true)
                {
                    var perimeter = t_side * 2 + t_base;
                    if (perimeter >= 1000000000) return;
                    total += perimeter;
                    trans(a - 2 * b + 2 * c, 2 * a - b + 2 * c, 2 * a - 2 * b + 3 * c);
                    trans(a + 2 * b + 2 * c, 2 * a + b + 2 * c, 2 * a + 2 * b + 3 * c);
                    trans(-a + 2 * b + 2 * c, -2 * a + b + 2 * c, -2 * a + 2 * b + 3 * c);
                }
            };
            // call the function to kick start the process, using the first Pythagorean Triple as initial seed
            trans(3, 4, 5);
            // return the result
            return total;
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
        static void TestBenchLoader(Func<long> test_method)
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
