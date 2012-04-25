using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler087csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The numbers below fifty million can be expressed as the sum: ");

            TestBenchSetup();
            TestBenchLoader(plan_brute_force_with_precalculated_primes);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// Just plain three nested loop each testing for double, triple and quad combinations,
        /// since the list for each power primes are sorted by nature, range check is implemented in each loop
        /// to skip unnecessary computation
        /// </summary>
        static int plan_brute_force_with_precalculated_primes()
        {
            // generate primes
            var limit = 50000000;
            var dlimit = (int)Math.Pow(limit, 0.5); // fins the limit for each prime power
            var tlimit = (int)Math.Pow(limit, 1.0 / 3.0);
            var qlimit = (int)Math.Pow(limit, 0.25);
            var max = dlimit + 1;
            var bound = (int)Math.Sqrt(max);
            var primes = new bool[max];
            primes[0] = true;
            primes[1] = true;
            int s, m, n;
            for (s = 2; s <= bound; s++)
            {
                if (primes[s] == false)
                {
                    for (m = s * s; m < max; m += s)
                    {
                        primes[m] = true;
                    }
                }
            }
            // calculate and save double, triple, and quad primes into respective lists
            var dlist = new List<int>();
            var tlist = new List<int>();
            var qlist = new List<int>();
            for (s = 2; s <= dlimit; s++)
            {
                if (primes[s] == false)
                {
                    if (s <= dlimit) dlist.Add(s * s);
                    if (s <= tlimit) tlist.Add(s * s * s);
                    if (s <= qlimit) qlist.Add(s * s * s * s);
                }
            }
            //  brute force loop to find all the possible combinations
            var counter = 0;
            var set = new HashSet<int>(); // used to keep out duplicate sum
            int q, qt, qtd;
            for (s = 0; s < qlist.Count; s++)
            {
                q = qlist[s];
                for (m = 0; m < tlist.Count; m++)
                {
                    qt = q + tlist[m];
                    if (qt >= limit) //  since the lists are in sorted order by nature
                        break;
                    for (n = 0; n < dlist.Count; n++)
                    {
                        qtd = qt + dlist[n];
                        if (qtd >= limit)
                            break;

                        if (set.Add(qtd) == true) // where the most of time is spent :(
                            counter++;
                    }
                }
            }
            return counter;
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
