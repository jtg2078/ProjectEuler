using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler111csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The sum of all S(10, d): ");

            TestBenchSetup();
            TestBenchLoader(brute_force_generate_all_possible_numbers_test_prime_with_Miller_Rabin);

            Console.WriteLine("press any key to exit");
            Console.ReadKey();
        }

        static long brute_force_generate_all_possible_numbers_test_prime_with_Miller_Rabin()
        {
            var d = 10;
            var bookkeeper = new ResultRecord((long)Math.Pow(10, d - 1));
            for (int a = 0; a <= 9; a++) // repeated digit
            {
                long number = 0;
                for (int b = 0; b < d; b++)
                {
                    number = number * 10 + a; // generate the initial number e.g. 1111111111, 2222222222
                }
                for (int c = 1; c <= d / 2; c++) // how many digits to alter
                {
                    recursive_number_generator(number, d, a, -1, c, bookkeeper);
                    if (bookkeeper.N[a] > 0)
                    {
                        bookkeeper.M[a] = d - c;
                        break;
                    }
                }
            }
            return bookkeeper.S.Sum();
        }


        /// <summary>
        /// basically a recursive method to generate a number with given parameter and test if such number
        /// is a prime number. For example: given M(4,1) 4 digit number, with 1 as most repeated digit - 
        ///     1. starts with 1111
        ///     2. alter one digit to (1....9 except 1) and in every position
        ///     3. test if the altered number is a prime
        ///         a. if there is any, then we know 3 as most repeated digit count
        ///         b. if none, go to 1, and alter 2 digits to (1....9 except 1) and in every position
        /// </summary>
        /// <param name="number">the constructed/constructing number</param>
        /// <param name="total_digits"># of digits in the number</param>
        /// <param name="repeat_digit">wat number is the repeated digit</param>
        /// <param name="previous_diff_index">the position of last altered digit(changed from repeated</param>
        /// <param name="remain_diff_count">how many more digits need to be altered</param>
        /// <param name="bookkeeper">the data structure used to keep track of results</param>
        static void recursive_number_generator(long number, int total_digits, int repeat_digit,
            int previous_diff_index, int remain_diff_count, ResultRecord bookkeeper)
        {
            if (remain_diff_count == 0)
            {
                if (number >= bookkeeper.MinNum && Miller_Rabin_primality_test(number) == true)
                {
                    bookkeeper.N[repeat_digit]++;
                    bookkeeper.S[repeat_digit] += number;
                }
            }
            else
            {
                for (int cur_diff_index = previous_diff_index + 1; cur_diff_index <= total_digits; cur_diff_index++)
                {
                    for (int cur_diff_digit = 0; cur_diff_digit <= 9; cur_diff_digit++)
                    {
                        if (cur_diff_digit == repeat_digit)
                            continue;

                        long tens = (long)Math.Pow(10, cur_diff_index - 1);
                        long new_number = number == 0 ?
                            cur_diff_digit * tens :
                            number - repeat_digit * tens + cur_diff_digit * tens;

                        recursive_number_generator(new_number, total_digits,
                            repeat_digit, cur_diff_index, remain_diff_count - 1, bookkeeper);
                    }
                }
            }
        }

        /// <summary>
        /// using the pseudocode of Miller–Rabin primality test from wiki to check if n is prime or not
        /// (see. http://en.wikipedia.org/wiki/Miller%E2%80%93Rabin_primality_test)
        /// </summary>
        static bool Miller_Rabin_primality_test(long n)
        {
            if (n < 2)
                return false;
            else if (n == 2 || n == 3)
                return true;
            else if (n % 2 == 0 || n % 3 == 0 || n % 5 == 0)
                return false;

            long s = 0;
            long d = n - 1;
            while (d % 2 == 0)
            {
                d /= 2;
                s++;
            }
            //  if n < 2,152,302,898,747, it is enough to test a = 2, 3, 5, 7, and 11;
            var a_list = new int[] { 2, 3, 5, 7, 11 };
            long x = 0;
            foreach (var a in a_list)
            {
                x = exponentiation_by_squaring_with_modulo(a, d, n);
                if (x == 1 || x == n - 1)
                {
                    continue;
                }
                for (long r = 1; r < s; r++)
                {
                    x = exponentiation_by_squaring_with_modulo(x, 2, n);
                    if (x == 1)
                        return false;
                    if (x == n - 1)
                        break;
                }
                if (x != n - 1)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Exponentiating by squaring is a general method for fast computation of large integer 
        /// powers of a number(from wiki). In this problem, the Miller Rabin primality test requires 
        /// computation of large powers of numbers with modulo operation. Thus this method. Decimal 
        /// data type is used and is big enough for dealing large number(10^20ish) for the scope of 
        /// this problem.
        /// see http://en.wikipedia.org/wiki/Exponentiation_by_squaring#Computation_by_powers_of_2
        /// </summary>
        /// <param name="x">base of the power</param>
        /// <param name="n">the power</param>
        /// <param name="mask">the number to be modulo with</param>
        static long exponentiation_by_squaring_with_modulo(decimal x, long n, long mask)
        {
            decimal r = 1;
            while (n > 0)
            {
                if ((n & 1) == 1) // using bitwise for checking odd
                {
                    r = (r * x) % mask;
                    n--;
                }
                x = (x * x) % mask;
                n >>= 1; // using bitshift for halving
            }
            return (long)r;
        }

        /// <summary>
        /// used to keep store results of M(10,d) N(10,d) and S(10,d)
        /// </summary>
        class ResultRecord
        {
            public int[] M;
            public int[] N;
            public long[] S;
            public long MinNum;
            public ResultRecord(long minNum)
            {
                M = new int[10];
                N = new int[10];
                S = new long[10];
                MinNum = minNum;
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
