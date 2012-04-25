using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace euler005csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();

            Console.WriteLine("The smallest number that is evenly divisible by all of the numbers from 1 to 20");
            sw.Start();
            var num1 = BruteForceWay2();
            sw.Stop();
            Console.WriteLine(string.Format("Plain loop possibilities brute force way(tick:{0}): {1}", sw.ElapsedTicks, num1));

            sw.Reset();

            Console.WriteLine("The smallest number that is evenly divisible by all of the numbers from 1 to 20");
            sw.Start();
            var num2 = ArithmeticWay(20);
            sw.Stop();
            Console.WriteLine(string.Format("Smart Arithmetic Way(tick:{0}): {1}", sw.ElapsedTicks, num2));

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        static int BruteForceWay()
        {
            // find the product of all prime below 20
            int p = 2 * 3 * 5 * 7 * 11 * 13 * 17 * 19;

            // has to be divisible by 20
            p = p + p % 20;

            bool isFound = true;
            while (true)
            {
                isFound = true;
                for (int i = 19; i >= 11; i--)
                {
                    if (p % i != 0)
                    {
                        isFound = false;
                        break;
                    }
                }
                if (isFound)
                    return p;

                p += 20;
            }

            return 0;
        }

        /// <summary>
        /// well, since the answer has to be divisible by product of all prime below 20
        /// use that number as increment basis
        /// a lot less loop than BruteForceWay(), beats the ArithmeticWay as well(hm...)
        /// </summary>
        /// <returns></returns>
        static int BruteForceWay2()
        {
            // find the product of all prime below 20
            int p = 2 * 3 * 5 * 7 * 11 * 13 * 17 * 19;

            bool isFound = true;
            for (int i = p; i <= int.MaxValue; i += p)
            {
                isFound = true;
                for (int j = 20; j > 10; j--)
                {
                    if (i % j != 0)
                    {
                        isFound = false;
                        break;
                    }
                }
                if (isFound)
                    return i;
            }
            return 0;
        }

        /// <summary>
        /// Base on the logic below(from the question thread):
        /// This does not require programming at all. Compute the prime factorization of each 
        /// number from 1 to 20, and multiply the greatest power of each prime together:
        /// 20 = 2^2 * 5
        /// 19 = 19
        /// 18 = 2 * 3^2
        /// 17 = 17
        /// 16 = 2^4
        /// 15 = 3 * 5
        /// 14 = 2 * 7
        /// 13 = 13
        /// 11 = 11 
        /// so basically a map
        /// index is the number
        /// the content:
        /// -1 : not a prime number , skip
        /// anything other than -1 is prime
        /// the content's number represent the highest power of that prime number
        /// so if we times them together, then we would get the answer
        /// the tricky part is to find out highest power of each prime
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        static int ArithmeticWay(int x)
        {
            int len = x + 1;
            int[] map = new int[len];
            map[0] = -1;
            map[1] = -1;
            int sum = 1;
            int power = 1;
            for (int i = 2; i < len; i++)
            {
                if (map[i] != -1)
                {
                    int start = i * i;

                    if (start >= x)
                        map[i] = 1;
                    else
                    {
                        power = 1;
                        for (int j = start; j <= x; j += i)
                        {
                            if (j > x)
                                break;

                            map[j] = -1;
                            
                            if (j == Math.Pow(i, power + 1))
                            {
                                power++;
                            }
                        }
                        map[i] = power;
                    }
                    sum = sum * (int)Math.Pow(i, map[i]);
                }
                
            }
            return sum;
        }
    }
}
