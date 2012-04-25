using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;

namespace euler089csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The sum of all the minimal product-sum numbers for 2≤k≤12000: ");

            TestBenchSetup();
            TestBenchLoader(find_bloated_form_and_subtract_accordingly);
            TestBenchLoader(parse_to_int_then_convert_to_minimum_form);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// DCCCC(900) -> CM
        /// CCCC(400)  -> CD
        /// LXXXX(90)  -> XC
        /// XXXX(40)   -> XL
        /// VIIII(9)   -> IX
        /// IIII(4)    -> IV
        /// since the problem stated that "the file contain no more than four consecutive identical units",
        /// so dont need to worry about 5-ish numeral units
        /// </summary>
        static int find_bloated_form_and_subtract_accordingly()
        {
            var saved = 0;
            foreach (var line in File.ReadAllLines("roman.txt"))
            {
                if (line.Contains("DCCCC") == true) saved += 3;
                else if (line.Contains("CCCC") == true) saved += 2;

                if (line.Contains("LXXXX") == true) saved += 3;
                else if (line.Contains("XXXX") == true) saved += 2;

                if (line.Contains("VIIII") == true) saved += 3;
                else if (line.Contains("IIII") == true) saved += 2;
            }
            return saved;
        }

        /// <summary>
        /// 1. covert each line in roman.txt from roman numerals form to int value
        /// 2. construct a minimum form of roman numerals from the int value in 1.
        /// 3. save the length difference between 1 and 2 and save the result
        /// </summary>
        static int parse_to_int_then_convert_to_minimum_form()
        {
            var result = 0;
            foreach (var line in File.ReadAllLines("roman.txt"))
            {
                var num = parse_numerals(line);
                var reduced = generate_minimum_numerals(num);
                var count = reduced.Length - line.Length;
                result += (line.Length - reduced.Length);
            }
            return result;
        }

        /// <summary>
        /// 1. Only I, X, and C can be used as the leading numeral in part of a subtractive pair.
        /// 2. I can only be placed before V and X.
        /// 3. X can only be placed before L and C.
        /// 4. C can only be placed before D and M. (1900 = MCM 2400 = MMCD)
        /// </summary>
        static StringBuilder generate_minimum_numerals(int number)
        {
            int rem, n;
            var numerals = new StringBuilder();
            // thousands
            if (number >= 1000)
            {
                n = Math.DivRem(number, 1000, out rem);
                Enumerable.Range(0, n).Aggregate(numerals, (a, b) => numerals.Append('M'));
                number = rem;
            }
            // hundreds
            if (number >= 100)
            {
                n = Math.DivRem(number, 100, out rem);
                if (n == 9)
                {
                    numerals.Append('C');
                    numerals.Append('M');
                    n -= 9;
                }
                else if (n >= 5)
                {
                    numerals.Append('D');
                    n -= 5;
                }
                else if (n == 4)
                {
                    numerals.Append('C');
                    numerals.Append('D');
                    n -= 4;
                }
                Enumerable.Range(0, n).Aggregate(numerals, (a, b) => numerals.Append('C'));
                number = rem;
            }
            // tens
            if (number >= 10)
            {
                n = Math.DivRem(number, 10, out rem);
                if (n == 9)
                {
                    numerals.Append('X');
                    numerals.Append('C');
                    n -= 9;
                }
                else if (n >= 5)
                {
                    numerals.Append('L');
                    n -= 5;
                }
                else if (n == 4)
                {
                    numerals.Append('X');
                    numerals.Append('L');
                    n -= 4;
                }
                Enumerable.Range(0, n).Aggregate(numerals, (a, b) => numerals.Append('X'));
                number = rem;
            }
            //ones
            if (number >= 1)
            {
                n = number;
                if (n == 9)
                {
                    numerals.Append('I');
                    numerals.Append('X');
                    n -= 9;
                }
                else if (n >= 5)
                {
                    numerals.Append('V');
                    n -= 5;
                }
                else if (n == 4)
                {
                    numerals.Append('I');
                    numerals.Append('V');
                    n -= 4;
                }
                Enumerable.Range(0, n).Aggregate(numerals, (a, b) => numerals.Append('I'));
            }
            return numerals;
        }

        /// <summary>
        /// I = 1
        /// V = 5
        /// X = 10
        /// L = 50
        /// C = 100
        /// D = 500
        /// M = 1000
        /// </summary>
        static int parse_numerals(string numerals)
        {
            var queue = new Queue<int>();
            for (int i = 0; i < numerals.Length; i++)
            {
                switch (numerals[i])
                {
                    case 'I': queue.Enqueue(1); break;
                    case 'V': queue.Enqueue(5); break;
                    case 'X': queue.Enqueue(10); break;
                    case 'L': queue.Enqueue(50); break;
                    case 'C': queue.Enqueue(100); break;
                    case 'D': queue.Enqueue(500); break;
                    case 'M': queue.Enqueue(1000); break;
                }
            }
            var num = 0;
            while (queue.Count > 1)
            {
                var n1 = queue.Dequeue();
                var n2 = queue.Peek();
                if (n1 < n2)
                {
                    queue.Dequeue();
                    num += (n2 - n1);
                }
                else
                    num += n1;
            }
            if (queue.Count > 0)
                num += queue.Dequeue();
            return num;
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
