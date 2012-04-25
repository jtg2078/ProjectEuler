using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler093csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The longest set of consecutive positive integers, 1 to n, can be obtained: ");

            TestBenchSetup();
            TestBenchLoader(plain_brute_force_test_all_possible_combinations);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// basically construct all the possible combinations of numbers, operators, and parentheses and calculate
        /// the generated formula:
        /// 1. generate all subset of 4 numbers out of 10 numbers
        /// 2. for each subset, generate all the permutation
        /// 3. for each permutation, generate all the operators combinations
        /// 4. compute the 5 placement of parentheses that reall matters and store the result
        /// 5. find and record the longest chain for the given subset of 4 number from 1.
        /// </summary>
        static string plain_brute_force_test_all_possible_combinations()
        {
            var s = new int[4];
            var ops = new char[] { '+', '-', '*', '/' };
            var len = s.Length;
            int j, l, z, tmp;
            var answer = string.Empty;
            var max = 0;
            // generates combination of 4 numbers from 0 to 9
            for (int a = 0; a < 7; a++)
            {
                for (int b = a + 1; b < 8; b++)
                {
                    for (int c = b + 1; c < 9; c++)
                    {
                        for (int d = c + 1; d < 10; d++)
                        {
                            s[0] = a;
                            s[1] = b;
                            s[2] = c;
                            s[3] = d;
                            var chain = new bool[1000]; // used to test for continutation of calculated results
                            // generates lexicographic permutation of set {a,b,c,d}
                            while (true)
                            {
                                //step 1. Find the largest index j such that a[j] < a[j + 1].
                                for (j = len - 2; j >= 0; j--)
                                {
                                    if (s[j] < s[j + 1]) break;
                                }
                                if (j == -1) break; // no more permutation, since no such index exist

                                //step 2. Find the largest index l such that a[j] < a[l]
                                for (l = len - 1; l >= 0; l--)
                                {
                                    if (s[j] < s[l]) break;
                                }

                                //step 3. Swap a[j] with a[l].
                                tmp = s[j];
                                s[j] = s[l];
                                s[l] = tmp;

                                //step 4. Reverse the sequence from a[j + 1] up to and including the final element a[n]
                                z = len - 1;
                                for (l = j + 1; l < len; l++)
                                {
                                    if (l >= z) break;

                                    tmp = s[l];
                                    s[l] = s[z];
                                    s[z] = tmp;
                                    z--;
                                }

                                // generates combinations of operators(+,-,*,/)
                                generate_operators(ops, s[0], s[1], s[2], s[3], chain);
                            }
                            // find out maximum continuous chain length from n = 1
                            var cur = 0;
                            for (j = 1; j < 1000; j++)
                            {
                                if (chain[j] == false)
                                    break;
                                cur++;
                            }
                            if (max < cur)
                            {
                                max = cur;
                                answer = string.Format("{0}{1}{2}{3}", a, b, c, d);
                            }
                        }
                    }
                }
            }
            return answer;
        }

        static void generate_operators(char[] ops, int a, int b, int c, int d, bool[] chain)
        {
            foreach (var op1 in ops)
            {
                foreach (var op2 in ops)
                {
                    foreach (var op3 in ops)
                    {
                        //  since there are only 5 parentheses combinations that really matters
                        validate_and_log_calculation(cal(cal(cal(a, b, op1), c, op2), d, op3), chain); // ((a + b) + c) + d
                        validate_and_log_calculation(cal(cal(a, cal(b, c, op2), op1), d, op3), chain); // (a + (b + c)) + d
                        validate_and_log_calculation(cal(a, cal(cal(b, c, op2), d, op3), op1), chain); // a + ((b + c) + d)
                        validate_and_log_calculation(cal(a, cal(b, cal(c, d, op3), op2), op1), chain); // a + (b + (c + d))
                        validate_and_log_calculation(cal(cal(a, b, op1), cal(c, d, op3), op2), chain); // (a + b) + (c + d)
                    }
                }
            }
        }

        static void validate_and_log_calculation(double value, bool[] chain)
        {
            if (value > 0 && value < 1000) // 1k should be well above the end of maximum chain 
            {
                var int_value = (int)value; // only integer result 
                if (value == int_value)
                    chain[int_value] = true;
            }
        }

        static double cal(double a, double b, char op)
        {
            double result = 0;
            switch (op)
            {
                case '+': result = a + b; break;
                case '-': result = a - b; break;
                case '*': result = a * b; break;
                case '/': result = b != 0 ? a / b : int.MinValue; break; // indicate that division by zero
            }
            return result;
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
        static void TestBenchLoader(Func<string> test_method)
        {
            stopwatch.Reset();
            stopwatch.Start();
            var result = string.Empty;
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
