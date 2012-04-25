using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace euler023csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();

            Console.WriteLine("The sum of all the positive integers which cannot be written as the sum of two abundant numbers");
            sw.Start();
            var sum1 = test1(28124);
            sw.Stop();
            Console.WriteLine(string.Format("construct all divisor map and find all combo way(ms:{0}): {1}", sw.ElapsedMilliseconds, sum1));

            sw.Reset();

            Console.WriteLine("The sum of all the positive integers which cannot be written as the sum of two abundant numbers");
            sw.Start();
            var sum2 = test2(28124);
            sw.Stop();
            Console.WriteLine(string.Format("construct all divisor map and with filtered combo way(ms:{0}): {1}", sw.ElapsedMilliseconds, sum2));

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
        static int loop_counter = 0;
        /// <summary>
        /// basically construct a map containing all divisor of each number up to ceiling
        /// find the abundant numbers and save to a list
        /// create a boolean map with number as index and bool as value(true = sum of 2 abundant numbers)
        /// using 2 nested to find out all possible combination of 2 abundant number within range(huge loop counts :(
        /// </summary>
        /// <param name="ceiling"></param>
        /// <returns></returns>
        static int test1(int ceiling)
        {
            loop_counter = 0;
            var map = new List<int>[ceiling + 1];
            var div = 0;
            int bound = (int)Math.Sqrt(ceiling);
            for (int i = 1; i <= bound; i++)
            {
                for (int j = i * i; j <= ceiling; j += i)
                {
                    if (map[j] == null)
                        map[j] = new List<int>();

                    map[j].Add(i);
                    div = j / i;

                    if (div < j && map[j].Any(l => l == div) == false)
                        map[j].Add(div);

                    loop_counter++;
                }
            }
            Console.WriteLine("loop counter for building divisor map: " + loop_counter);
            var abundant = new List<int>();
            for (int i = 2; i < ceiling; i++)
            {
                if (map[i].Sum() > i)
                    abundant.Add(i);
            }
            var map2 = new bool[ceiling];
            var len = abundant.Count;
            int num = 0;
            loop_counter = 0;
            for (int i = 0; i < len; i++)
            {
                for (int j = i; j < len - 1; j++)
                {
                    num = abundant[i] + abundant[j];
                    if (num < ceiling)
                        map2[num] = true;

                    loop_counter++;
                }
            }
            Console.WriteLine("loop counter for building abundant map: " + loop_counter);
            var sum = 0;
            for (int i = 0; i < ceiling; i++)
            {
                if (map2[i] == false)
                    sum += i;
            }
            return sum;
        }
        /// <summary>
        /// using the knowledge from http://tafakuri.net/?p=71
        /// 1.The upper bound for numbers that cannot be expressed as the sum of 
        ///   two numbers is actually 20161 not 28123 (as shown here).
        /// 2.All even numbers greater than 48 can be written as the sum of two 
        ///   abundant numbers (also shown in the article here).
        /// </summary>
        /// <param name="ceiling"></param>
        /// <returns></returns>
        static int test2(int ceiling)
        {
            var map = new List<int>[ceiling + 1];
            var div = 0;
            int bound = (int)Math.Sqrt(ceiling);
            for (int i = 1; i <= bound; i++)
            {
                for (int j = i * i; j <= ceiling; j += i)
                {
                    if (map[j] == null)
                        map[j] = new List<int>();

                    map[j].Add(i);
                    div = j / i;

                    if (div < j && map[j].Any(l => l == div) == false)
                        map[j].Add(div);
                }
            }
            
            var even = new List<int>();
            var odd = new List<int>();
            for (int i = 2; i < ceiling; i++)
            {
                if (map[i].Sum() > i)
                {
                    if(i % 2 == 0)
                        even.Add(i);
                    else
                        odd.Add(i);
                }
            }

            var map2 = new bool[ceiling];
            map2[24] = true;
            map2[30] = true;
            map2[32] = true;
            map2[36] = true;
            map2[38] = true;
            map2[40] = true;
            map2[42] = true;
            map2[44] = true;
            map2[48] = true;
            var num = 0;
            loop_counter = 0;
            for (int i = 0; i < odd.Count; i++)
            {
                for (int j = 0; j < even.Count; j++)
                {
                    num = odd[i] + even[j];
                    if (num < ceiling)
                        map2[num] = true;

                    loop_counter++;
                }
            }
            Console.WriteLine("loop counter for building abundant map: " + loop_counter);
            var sum = 0;
            for (int i = 0; i < ceiling; i++)
            {
                if (i > 48 && i % 2 == 0)
                    map2[i] = true;

                if (map2[i] == false)
                    sum += i;
            }
            return sum;
        }
    }
}
