using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace euler025csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();

            Console.WriteLine("The first term in the Fibonacci sequence to contain 1000 digits");
            sw.Start();
            var sum1 = test1(1000);
            sw.Stop();
            Console.WriteLine(string.Format("plain loop arithmetic way(ms:{0}): {1}", sw.ElapsedMilliseconds, sum1));

            sw.Reset();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// the while loop way of calculation Fibonacci sequence:
        /// (since the number can get way too big, so int[] is used to
        /// hold numbers for this problem)
        /// 
        /// var previous = 1;
        /// var cur = 1;
        /// var next = 0;
        /// while (true)
        /// {
        ///     next = previous + cur;
        ///     previous = cur;
        ///     cur = next;
        ///     Console.WriteLine(next);
        /// }
        /// </summary>
        /// <param name="ceiling"></param>
        /// <returns></returns>
        static int test1(int ceiling)
        {
            var previous = new int[1000];
            var next = new int[1000];
            var cur = new int[1000];
            previous[0] = 1; //f(1)
            cur[0] = 1; //f(2)
            var term = 2;
            var len = 1;
            var sum = 0;
            var rem = 0;
            var carry = 0;
            var i = 0;

            while (len < ceiling)
            {
                // next = previous + cur;
                for (i = 0; i < len; i++)
                {
                    sum = previous[i] + cur[i] + carry;
                    rem = sum > 9 ? sum - 10 : sum;
                    carry = sum > 9 ? 1 : 0;

                    next[i] = rem;
                    if (i == len - 1 && carry > 0)
                    {
                        next[i + 1] += carry;
                        carry = 0;
                        len++;
                        break;
                    }
                }
                
                //previous = cur;
                //cur = next;
                for (i = 0; i < len; i++)
                {
                    previous[i] = cur[i];
                    cur[i] = next[i];
                }
                term++;
            }
            return term;
        }
    }
}
