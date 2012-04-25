using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace euler021csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();

            Console.WriteLine("The sum of all the amicable numbers under 10000");
            sw.Start();
            var sum1 = test(10000);
            sw.Stop();
            Console.WriteLine(string.Format("construct all divisor map and then match way(tick:{0}): {1}", sw.ElapsedTicks, sum1));

            sw.Reset();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// basically find out all the divisor for each number from 1 to 10000 by using 
        /// sieve of eratosthenes way of constructing a dictionary of number as key and value is list of divisors
        /// next step is iterate the dictionary and find out all the amicable pairs and store them in a list
        /// sum of list is the answer!
        /// </summary>
        /// <param name="ceiling"></param>
        /// <returns></returns>
        static int test(int ceiling)
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
            var sum = 0;
            var pairs = new List<int>();
            for (int i = 2; i < ceiling; i++)
            {
                sum = map[i].Sum();
                if (sum < ceiling && sum != i && map[sum].Sum() == i)
                {
                    if (pairs.Contains(sum) == false)
                        pairs.Add(sum);
                    if (pairs.Contains(i) == false)
                        pairs.Add(i);
                }
            }
            return pairs.Sum();
        }
    }
}
