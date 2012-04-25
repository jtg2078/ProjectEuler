using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler032csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The sum of all numbers that can be written as pandigital products:");

            TestBenchSetup();
            TestBenchLoader(using_knuth_permutation_algorithm_to_generate_candidates);
            TestBenchLoader(brute_force_with_limited_range_for_candidates);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// from this we can know the there can be only two possible way of making a number that fit 
        /// this problem's description:
        ///     1. 1-d * 4-d = 4-d
        ///     2. 2-d * 3-d = 4-d
        /// </summary>
        static void bound_checking()
        {
            Console.WriteLine(string.Format("{0} * {1} = {2}", 9, 9, 9 * 9));
            Console.WriteLine(string.Format("{0} * {1} = {2}", 9, 99, 9 * 99));
            Console.WriteLine(string.Format("{0} * {1} = {2}", 9, 999, 9 * 999));
            Console.WriteLine(string.Format("{0} * {1} = {2}", 9, 9999, 9 * 9999));
            Console.WriteLine(string.Format("{0} * {1} = {2}", 9, 99999, 9 * 99999));
            Console.WriteLine();

            Console.WriteLine(string.Format("{0} * {1} = {2}", 99, 99, 99 * 99));
            Console.WriteLine(string.Format("{0} * {1} = {2}", 999, 99, 999 * 99));
            Console.WriteLine(string.Format("{0} * {1} = {2}", 111, 111, 111 * 111));
        }

        /// <summary>
        /// using the code problem 24, which generates lexicographis permutation of given
        /// range, a very efficient algorithm.
        /// since there are limited combination how a pandigital can be made (see bound_checking())
        /// the structure of the multiplicand/multiplier/product can only be the following:
        ///     1. (1 digit) * (4 digit) = (4 digit) (or 4-1)
        ///     2. (2 digit) * (3 digit) = (4 digit) (Or 3-2)
        /// so it is fairly easy to test for each number generate by the permutation algorithm
        /// </summary>
        /// <returns></returns>
        static int using_knuth_permutation_algorithm_to_generate_candidates()
        {
            var a = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var j = 0;
            var l = 0;
            var tmp = 0;
            var len = a.Length;
            var z = 0;
            var p = 0;
            var num = 0;
            var rem = 0;
            var cont = true;
            var index = 0;
            var result = new HashSet<int>();
            while (true)
            {
                //step 1. Find the largest index j such that a[j] < a[j + 1].
                for (j = len - 2; j >= 0; j--)
                {
                    if (a[j] < a[j + 1])
                        break;
                }

                if (j == -1)
                    break; // no more permutation, since no such index exist

                //step 2. Find the largest index l such that a[j] < a[l]
                for (l = len - 1; l >= 0; l--)
                {
                    if (a[j] < a[l])
                        break;
                }

                //step 3. Swap a[j] with a[l].
                tmp = a[j];
                a[j] = a[l];
                a[l] = tmp;

                //step 4. Reverse the sequence from a[j + 1] up to and including the final element a[n]
                z = len - 1;
                for (l = j + 1; l < len; l++)
                {
                    if (l >= z)
                        break;

                    tmp = a[l];
                    a[l] = a[z];
                    a[z] = tmp;
                    z--;
                }

                p = a[0] * (a[1] * 1000 + a[2] * 100 + a[3] * 10 + a[4]);
                if (p > 1000 && p < 10000)
                {
                    num = p;
                    index = 8;
                    cont = true;
                    while (num > 0)
                    {
                        rem = num % 10;
                        num = num / 10;
                        if (rem != a[index--])
                        {
                            cont = false;
                            break;
                        }
                    }
                    if (cont)
                    {
                        //Console.WriteLine(a.Aggregate(string.Empty, (s, n) => s += n));
                        result.Add(p);
                        continue;
                    }
                }
                p = (a[0] * 10 + a[1]) * (a[2] * 100 + a[3] * 10 + a[4]);
                if (p > 1000 && p < 10000)
                {
                    num = p;
                    index = 8;
                    cont = true;
                    while (num > 0)
                    {
                        rem = num % 10;
                        num = num / 10;
                        if (rem != a[index--])
                        {
                            cont = false;
                            break;
                        }
                    }
                    if (cont)
                    {
                        //Console.WriteLine(a.Aggregate(string.Empty, (s, n) => s += n));
                        result.Add(p);
                        continue;
                    }
                }

            }
            return result.Sum();
        }

        /// <summary>
        /// since both way of (1-4) and (2-3) generates a 4-digit product
        /// so set the range from 1000 to 9000 for z, and test (1-4) senarion where x(1-9) * y = z, which
        /// if z / x = y, in which x, y and z makes a 1-9 pandigital
        /// do the same for (2-3) scenario
        /// </summary>
        /// <returns></returns>
        static int brute_force_with_limited_range_for_candidates()
        {
            int a, b, c, i, num, rem;
            var n = new int[5, 10]; // uses to track duplicate digits
            var skip = false;
            var result = new HashSet<int>();
            for (c = 1000; c < 9999; c++)
            {
                num = c;
                skip = false;
                while (num > 0)
                {
                    rem = num % 10;
                    num = num / 10;
                    if (n[0, rem] != 0 || rem == 0)
                    {
                        skip = true;
                        break;
                    }
                    n[0, rem] = 1;
                }
                if (skip == false)
                {
                    //1,4
                    for (a = 1; a <= 9; a++)
                    {
                        rem = c % a;
                        num = c / a;
                        if (rem == 0 && num > 1000 && n[0, a] == 0)
                        {
                            n[1, a] = 1;
                            //-----------------------------
                            skip = false;
                            while (num > 0)
                            {
                                rem = num % 10;
                                num = num / 10;
                                if (n[0, rem] != 0 || n[1, rem] != 0 || n[2, rem] != 0 || rem == 0)
                                {
                                    skip = true;
                                    break;
                                }
                                n[2, rem] = 1;
                            }
                            if (skip == false && result.Add(c))
                            {

                                //Console.WriteLine(a + " * " + c / a + " = " + c);
                            }
                            for (i = 0; i < 10; i++)
                                n[2, i] = 0;
                            //-----------------------------
                            n[1, a] = 0;
                        }
                    }
                    //2,3
                    for (a = 10; a <= 99; a++)
                    {
                        skip = false;
                        num = a;
                        while (num > 0)
                        {
                            rem = num % 10;
                            num = num / 10;
                            if (n[0, rem] != 0 || n[1, rem] != 0 || rem == 0)
                            {
                                skip = true;
                                break;
                            }
                            n[1, rem] = 1;
                        }
                        if (skip == false)
                        {
                            rem = c % a;
                            num = c / a;
                            if (rem == 0 && num > 100 && num < 1000)
                            {
                                //-----------------------------
                                skip = false;
                                while (num > 0)
                                {
                                    rem = num % 10;
                                    num = num / 10;
                                    if (n[0, rem] != 0 || n[1, rem] != 0 || n[2, rem] != 0 || rem == 0)
                                    {
                                        skip = true;
                                        break;
                                    }
                                    n[2, rem] = 1;
                                }
                                if (skip == false && result.Add(c))
                                {
                                    //Console.WriteLine(a + " * " + c / a + " = " + c);
                                }
                                for (i = 0; i < 10; i++)
                                    n[2, i] = 0;
                                //-----------------------------
                            }
                        }
                        for (i = 0; i < 10; i++)
                            n[1, i] = 0;
                    }
                }
                //reset number(level p)
                for (i = 0; i < 10; i++)
                    n[0, i] = 0;
            }
            return result.Sum();
            //Console.WriteLine("Sum is: " + result.Aggregate((s, z) => s += z));
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
