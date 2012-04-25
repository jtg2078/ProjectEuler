using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace euler006csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();

            Console.WriteLine("The difference between the sum of the squares of the first one hundred natural numbers and the square of the sum");
            sw.Start();
            var num1 = BruteForceWay(100);
            sw.Stop();
            Console.WriteLine(string.Format("Plain loop addition brute force way(Linq aggregate)(tick:{0}): {1}", sw.ElapsedTicks, num1));

            sw.Reset();

            Console.WriteLine("The difference between the sum of the squares of the first one hundred natural numbers and the square of the sum");
            sw.Start();
            var num3 = BruteForceWay2(100);
            sw.Stop();
            Console.WriteLine(string.Format("Plain loop addition brute force way(for loop)(tick:{0}): {1}", sw.ElapsedTicks, num3));
            
            sw.Reset();

            Console.WriteLine("The difference between the sum of the squares of the first one hundred natural numbers and the square of the sum");
            sw.Start();
            var num2 = ArithmeticWay(100);
            sw.Stop();
            Console.WriteLine(string.Format("Smart Arithmetic Way(tick:{0}): {1}", sw.ElapsedTicks, num2));

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
        /// <summary>
        /// the summation formula is
        /// (n/2)(2a+(n-1)d)
        /// n = number of terms in the sequence
        /// d = common difference
        /// a = first term
        /// eg 1+2+3+4+...+100=5050
        /// (100/2)(2*1+(100-1)*1)
        /// </summary>
        /// <returns></returns>
        static int BruteForceWay(int x)
        {
            var sumSq = Enumerable.Range(1, x)
                .Aggregate((s, n) => s += n * n);

            var sqSum = x * (2 * 1 + (x - 1) * 1) / 2;
            sqSum = sqSum * sqSum;

            return sqSum - sumSq;
        }

        static int BruteForceWay2(int r)
        {
            int x = 0;
            int y = 0;
            int z = 0;

            for (x = 1; x <= r; x++)
            {
                y += x * x;
                z += x;
            }

            return z * z - y;
        }

        /// <summary>
        /// got the forumla from wikipedia, see: 
        /// http://en.wikipedia.org/wiki/List_of_mathematical_series
        /// 
        /// (1+2+3+...+n) = n(n+1)/2
        /// (1+2+3+...+n)^2 = n*n(n+1)(n+1)/4
        /// (1^2+2^2+3^2+...+n^2) = n(n+1)(2n+1)/6
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        static int ArithmeticWay(int x)
        {
            var sumSq = x * (x + 1) * (2 * x + 1) / 6;

            var sqSum = x * x * (x + 1) * (x + 1) / 4;

            return sqSum - sumSq;
        }
    }
}
