using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace euler016csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();

            Console.WriteLine("The sum of the digits of the number 2^(1000)");
            sw.Start();
            var sum1 = test3(1000);
            sw.Stop();
            Console.WriteLine(string.Format("plain arithmetic brute force way(tick:{0}): {1}", sw.ElapsedTicks, sum1));

            sw.Reset();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// 2*1 = 2, 2*2 = 4 = (2*1)*2 = (2*1) + (2*1)
        /// so 2^(n+1) = (2^n) + (2^n)
        /// using the same way of adding big numbers from question 13
        /// so basically we add the number in the array(int[] map) to itself for each power iteration
        /// the size of array is 1000, assume that each addition would advance one decimal
        /// (which is a pretty crude estimate)
        /// but the code use the count variable to control number of loop for inner for
        /// so 2^3 = 8 = 2^2 + 2^2 = just need to loop 1 time
        /// 2^4 = 16 = 2^3 + 2^3 = [1][6]..... advance count by 1, so now on loop 2 times at least
        /// </summary>
        /// <param name="power"></param>
        /// <returns></returns>
        static int test3(int power)
        {
            var map = new int[power];
            map[0] = 2;
            var count = 1;
            var sum = 0;
            var rem = 0;
            var carry = 0;
            for (int i = 1; i < power; i++)
            {
                for (int j = 0; j < count; j++) // so for each digit
                {
                    sum = map[j] * 2 + carry;
                    rem = sum % 10;
                    map[j] = rem;
                    carry = (sum - rem) / 10;
                    if (j == count - 1 && carry > 0)
                    {
                        map[j + 1] += carry;
                        carry = 0;
                        count++;
                        break;
                    }
                }
            }
            return map.Sum();
        }
    }
}
