using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace euler026csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();

            Console.WriteLine("The longest recurring cycle in its decimal fraction part.");
            sw.Start();
            var sum1 = test1();
            sw.Stop();
            Console.WriteLine(string.Format("loop + HashSet + long division algorithm(tick:{0}): {1}", sw.ElapsedTicks, sum1));

            sw.Reset();

            sw.Start();
            var sum2 = test2();
            sw.Stop();
            Console.WriteLine(string.Format("optimized version of test1(tick:{0}): {1}", sw.ElapsedTicks, sum2));

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// using the remainder as indicator(long division algorithm)
        /// add the remainder to HashSet(only allow unique values),
        /// if there are duplicate(the Add will return false), then we know the chain has ended
        /// iterate through all the number and find out the longest chain and return the number
        /// </summary>
        /// <returns></returns>
        static int test1()
        {
            var num = 0;
            var master = 10;
            var set = new HashSet<int>();
            var longest_count = 0;
            var longest_num = 0;

            for (int i = 2; i < 1000; i++)
            {
                num = i > 100 ? 1000 : i > 10 ? 100 : 10;
                set.Clear();
                do
                {
                    num = num % i;
                    num *= master;
                } while (set.Add(num));

                if (set.Count >= longest_count)
                {
                    longest_num = i;
                    longest_count = set.Count;
                }
            }
            return longest_num;
        }

        /// <summary>
        /// the optimized version of test1(), with the given properties
        /// 1. Since the remainder has to be a unique number in a chain, so there are only
        ///    n-1 possible numbers for number n. E.g. for 1/7 the longest chain possible is 6
        /// 2. A fraction in lowest terms with a prime denominator other than 2 or 5 (i.e. coprime to 10) 
        ///    always produces a repeating decimal.
        ///    see. http://en.wikipedia.org/wiki/Recurring_decimal#Fractions_with_prime_denominators
        ///    with this, there are definitely numbers under 1000 that can have chains with length of n-1
        /// 3. Iterate the number in descending order, the first number with cycle length of n-1 would
        ///    most likely to be the number with longest cycle
        /// </summary>
        /// <returns></returns>
        static int test2()
        {
            var num = 0;
            var master = 10;
            var set = new HashSet<int>();

            for (int i = 999; i >= 2; i--)
            {
                num = i > 100 ? 1000 : i > 10 ? 100 : 10;
                set.Clear();
                do
                {
                    num = num % i;
                    num *= master;
                } while (set.Add(num));

                if (set.Count == i - 1)
                {
                    return i;
                }
            }
            return 0;
        }
    }
}
