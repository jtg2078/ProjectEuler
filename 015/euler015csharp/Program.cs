using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace euler015csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();

            Console.WriteLine("The number of routes in a 20×20 grid");
            sw.Start();
            var sum1 = Explorer(20);
            sw.Stop();
            Console.WriteLine(string.Format("Pascal's Triangle brute force way(tick:{0}): {1}", sw.ElapsedTicks, sum1));
            
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
        
        /// <summary>
        /// http://mathforum.org/advanced/robertd/manhattan.html
        /// </summary>
        /// <param name="bound"></param>
        /// <returns></returns>
        static long Explorer(int bound)
        {
            var map = new long[bound + 1, bound + 1];

            // 1. populate edge row and columns
            for (int i = 0; i <= bound; i++)
            {
                map[i, bound] = 1;
                map[bound, i] = 1;
            }

            // 2. starting 2nd last column to populate values in each cell(pascal triangle value)
            for (int col = bound - 1; col >= 0; col--)
            {
                for (int row = bound - 1; row >= 0; row--)
                {
                    map[row, col] = map[row + 1, col] + map[row, col + 1];
                }
            }

            return map[0, 0];
        }

        //static int ways;
        ////static long bound = 2;
        //static bool[,] map = new bool[21, 21];
        //static void Explorer1(int x, int y, int bound)
        //{
        //    if (map[x, y])
        //        return;

        //    if (x > bound || y > bound)
        //        return;

        //    if (x == bound && y == bound)
        //    {
        //        ways++;
        //        return;
        //    }

        //    map[x + 1, y] = true;
        //    Explorer1(x + 1, y,bound);
        //    map[x, y + 1] = true;
        //    Explorer1(x, y + 1,bound);
        //}

        //static ulong Explorer4(int bound)
        //{
        //    ulong p1 = 1;
            
        //    p1 = Enumerable.Range(bound+1, bound)
        //        .Aggregate(p1, (fact, num) => fact *= (ulong)num);

        //    ulong p2 = 1;

        //    p2 = Enumerable.Range(1, bound)
        //        .Aggregate(p2, (fact, num) => fact *= (ulong)num);
            

        //    return p1/p2;
        //}
    }
}
