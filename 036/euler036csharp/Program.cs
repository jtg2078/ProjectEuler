using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler036csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The sum of all numbers which are equal to the sum of the factorial of their digits:");

            TestBenchSetup();
            TestBenchLoader(store_number_in_array_and_brute_force_every_number);
            TestBenchLoader(brute_force_every_number_using_formula_from_q_description);
            TestBenchLoader(generate_palindromes_using_algorithm_from_q_description_base_ten);
            TestBenchLoader(generate_palindromes_using_algorithm_from_q_description_base_two);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// bascially store the number in array so checking whether it is palindrome can be easily done
        /// but this method is only slightly faster than just checking each number
        /// i guess array operation is not cheap
        /// </summary>
        /// <returns></returns>
        static int store_number_in_array_and_brute_force_every_number()
        {
            var max = 999999;
            var bin_len = Convert.ToString(max, 2).Length; // convert to binary to get length
            var bin = new int[bin_len];
            var dec_len = max.ToString().Length;
            var dec = new int[dec_len];
            int i, carry, fwd, rwd;
            var num = 0;
            var bin_count = 0;
            var dec_count = 0;
            var result = 0;
            var isPal = true;
            while (++num <= max)
            {
                isPal = true;
                //calculate in binary
                i = bin_len;
                carry = 1;
                while (carry >= 1) //adding one
                {
                    i--;
                    bin[i] += carry;
                    carry = bin[i] > 1 ? 1 : 0;
                    bin[i] = bin[i] > 1 ? 0 : bin[i];
                    if (bin_len - i > bin_count)
                        bin_count = bin_len - i;
                }
                fwd = bin_len - bin_count;
                rwd = bin_len - 1;
                while (isPal && rwd >= fwd) // test for palindrome
                {
                    if (bin[fwd++] != bin[rwd--])
                        isPal = false;
                }
                // calculate in decimal
                i = dec_len;
                carry = 1;
                while (carry >= 1) // adding one
                {
                    i--;
                    dec[i] += carry;
                    carry = dec[i] > 9 ? 1 : 0;
                    dec[i] = dec[i] > 9 ? 0 : dec[i];
                    if (dec_len - i > dec_count)
                        dec_count = dec_len - i;
                }
                fwd = dec_len - dec_count;
                rwd = dec_len - 1;
                while (isPal && rwd >= fwd) // test for palindrome
                {
                    if (dec[fwd++] != dec[rwd--])
                        isPal = false;
                }
                if (isPal)
                    result += num;
            }
            return result;
        }

        /// <summary>
        /// loop through all the number and use the method described in the question's
        /// note to test whether or not it is palindrome
        /// </summary>
        /// <returns></returns>
        static int brute_force_every_number_using_formula_from_q_description()
        {
            var result = 0;
            for (int i = 1; i < 1000000; i += 2)
            {
                if (isPalindrome(i, 2) && isPalindrome(i, 10))
                    result += i;
            }
            return result;
        }

        /// <summary>
        /// basically generate palindromic number rather than test each number under 1mil to see
        /// if it is palindrome. so
        /// 1->11
        /// 2->22
        /// 12-> 1221
        /// until 1000-> 10000001 for even length palindrome
        /// 11->111
        /// 12->121
        /// until 1000-> 1000001 for odd length palindrome
        /// so using base 10 to generate all the palindrome, and then use isPalindrome method for base 2
        /// see http://mathworld.wolfram.com/PalindromicNumber.html
        /// for info on finding total # of palindromic under given range
        /// total Palindromic a(n) = 2*(10^(n/2)-1) when n=even, n is the power of 10(10^6, n=6)
        /// when n is odd:11*10^((n-1)/2)-2
        /// the possible combination of palindromic number under 1 mil:
        /// a,aa,aba,abba,abcba,abccba
        /// </summary>
        /// <returns></returns>
        static int generate_palindromes_using_algorithm_from_q_description_base_ten()
        {
            var limit = 1000000;
            var sum = 0;
            var i = 1;
            var p = makePalindrome(i, 10, true);
            while (p < limit)
            {
                if (isPalindrome(p, 2))
                    sum += p;
                p = makePalindrome(++i, 10, true);
            }
            i = 1;
            p = makePalindrome(i, 10, false);
            while (p < limit)
            {
                if (isPalindrome(p, 2))
                    sum += p;
                p = makePalindrome(++i, 10, false);
            }
            return sum;
        }

        /// <summary>
        /// reverse version of generate_palindromes_using_algorithm_from_q_description_base_ten,
        /// generate base 2 palindromic, this is slightly faster since we can use bit operator
        /// to do division and modulo, which is faster than normal divison and modulo
        /// </summary>
        /// <returns></returns>
        static int generate_palindromes_using_algorithm_from_q_description_base_two()
        {
            var limit = 1000000;
            var sum = 0;
            var i = 1;
            var p = makePalindromeBase2(i, true);
            while (p < limit)
            {
                if (isPalindrome(p, 10))
                    sum += p;
                p = makePalindromeBase2(++i, true);
            }
            i = 1;
            p = makePalindromeBase2(i, false);
            while (p < limit)
            {
                if (isPalindrome(p, 10))
                    sum += p;
                p = makePalindromeBase2(++i, false);
            }
            return sum;
        }

        /// <summary>
        /// basically for if the number is 1234
        /// the reverse is 4321, that mean 4*10*10*10 = 4000
        /// and this is what b*reverse is about (in this example, base is 10)
        /// r       k(k=n)
        /// 0+4     123
        /// 40+3    12
        /// 430+2   1
        /// 4320+1  0
        /// 4321
        /// </summary>
        /// <param name="n"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        static bool isPalindrome(int n, int b)
        {
            var reverse = 0;
            var k = n;
            while (k > 0)
            {
                reverse = b * reverse + k % b;
                k /= b;
            }
            return n == reverse;
        }

        /// <summary>
        /// basically create a new number with n + reverse of n
        /// e.g: n= 86, the output would be 8668
        /// the isOdd is for generting odd length, since the above
        /// method always generate even: 123 =>123321
        /// when set odd to true, 3 is removed
        /// 123=> 12(3-removed)+ 321 = 12321
        /// </summary>
        /// <param name="n"></param>
        /// <param name="b"></param>
        /// <param name="isOdd"></param>
        /// <returns></returns>
        static int makePalindrome(int n, int b, bool isOdd)
        {
            var res = n;
            if (isOdd)
                n /= b;
            while (n > 0)
            {
                res = b * res + n % b;
                n /= b;
            }
            return res;
        }

        /// <summary>
        /// well bit-shift is faster, and especially we need to do it in base 2
        ///   10111011  =  187
        ///   11011111  =  223
        ///AND
        ///   10011011  =  155
        /// (so n & 1 is effectively n % 2)
        /// shift left
        /// 00001111  =  15
        /// SHIFT LEFT
        /// 00011110  =  30
        /// (so n >>= 1 is effectively n= n/2)
        /// (so res << 1 is effectuveky n*2
        /// </summary>
        /// <param name="n"></param>
        /// <param name="isOdd"></param>
        /// <returns></returns>
        static int makePalindromeBase2(int n, bool isOdd)
        {
            var res = n;
            if (isOdd)
                n >>= 1;
            while (n > 0)
            {
                res = (res << 1) + (n & 1);
                n >>= 1; ;
            }
            return res;
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
