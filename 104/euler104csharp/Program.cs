using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler104csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The first Fibonacci number for which the first nine digits AND the last nine digits are 1-9 pandigital: ");

            TestBenchSetup();
            TestBenchLoader(using_Binet_formula_for_1st_9_digits_and_modulo_for_last_9_digits);

            Console.WriteLine("press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// by using Binet's Formula to approximate nth Fibonacci number, and combine this formula with logarithm
        /// to get first 9 digits from the fib number.
        /// as for the last 9 digits, the simple iteration and build up(combine with modulo) does the job well
        /// see (http://www.maths.surrey.ac.uk/hosted-sites/R.Knott/Fibonacci/fibFormula.html)
        /// awesomely awesome link about Fibonacci number
        /// </summary>
        static int using_Binet_formula_for_1st_9_digits_and_modulo_for_last_9_digits()
        {
            var f1 = 1;
            var f2 = 1;
            var fn = f1 + f2;
            var n = 2;
            var map = new int[10];
            var phi = (1 + Math.Sqrt(5)) / 2;
            Func<int, bool> check_perm = num =>
                {
                    map[1] = 0; map[2] = 0; map[3] = 0;
                    map[4] = 0; map[5] = 0; map[6] = 0;
                    map[7] = 0; map[8] = 0; map[9] = 0;

                    var rem = 0;
                    var pass = true;
                    while (num > 0)
                    {
                        num = Math.DivRem(num, 10, out rem);
                        if (rem == 0 || map[rem] != 0)
                        {
                            pass = false;
                            break;
                        }
                        map[rem] = 1;
                    }
                    return pass == true && map[1] == 1 && map[2] == 1 && map[3] == 1
                                        && map[4] == 1 && map[5] == 1 && map[6] == 1
                                        && map[7] == 1 && map[8] == 1 && map[9] == 1;
                };
            var found = false;
            while (found == false)
            {
                fn = (f1 + f2) % 1000000000;
                f1 = f2;
                f2 = fn;
                n++;

                if (check_perm(fn) == true) // only calculate the 1st 9 digits if last 9 digits are good
                {
                    var step1 = n * Math.Log10(phi) - Math.Log10(5) / 2; // Log(phi^nth / √5)
                    var step2 = Math.Pow(10, step1 - (int)step1) * 100000000; // only need the fraction part and turn it to int
                    var step3 = (int)step2; // only need the int part
                    found = check_perm(step3);
                }
            }
            return n;
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
