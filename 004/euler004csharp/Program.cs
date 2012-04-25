using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace euler004csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();

            Console.WriteLine("The largest palindrome made from the product of two 3-digit numbers");
            sw.Start();
            var num1 = BruteForceWay();
            sw.Stop();
            Console.WriteLine(string.Format("Loop brute force way(n^2)(tick:{0}): {1}", sw.ElapsedTicks, num1));

            sw.Reset();

            Console.WriteLine("The largest palindrome made from the product of two 3-digit numbers");
            sw.Start();
            var num2 = BruteForceWay3();
            sw.Stop();
            Console.WriteLine(string.Format("LINQ brute force way(n)(tick:{0}): {1}", sw.ElapsedTicks, num2));

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        static int BruteForceWay()
        {
            //Enumerable.Range(0, 999)
            //    .Select(l => (999 * (999 - l)).ToString())
            //    .First(l => l.Substring(0, 3) == l.Substring(3, 3)); 
            //return Enumerable.Range(0, 900)
            //        .Select(l => 999 * (999 - l))
            //        .First(l => l.ToString().Substring(0, 3) == l.ToString().Substring(3, 3));
            //return Convert.ToInt32( 
            //    Enumerable.Range(0, 89)
            //       .Select(l => (99 * (99 - l)).ToString())
            //       .First(l => l[0] == l[3] && l[1] == l[2]));
            int max = 0;
            for (int i = 999; i > 100; i--)
            {
                for (int j = 999; j > 100; j--)
                {
                    var p = i * j;
                    var l = p.ToString();

                    if (p > 100000 && l[0] == l[5] && l[1] == l[4] && l[2] == l[3])
                    {
                        if (p > max)
                            max = p;
                    }
                }
            }
            return max;

            //for (int i = 99; i > 10; i--)
            //{
            //    for (int j = 99; j > 10; j--)
            //    {
            //        var l = (i * j).ToString();

            //        if (l[0] == l[3] && l[1] == l[2])
            //            return (i * j);
            //    }
            //}

            //return Convert.ToInt32(
            //    Enumerable.Range(0, 899)
            //       .Select(l => (999 * (999 - l)).ToString())
            //       .First(l => l[0] == l[5] && l[1] == l[4] && l[2] == l[3]));
        }

        static int BruteForceWay2()
        {
            return Convert.ToInt32(
                Enumerable.Range(0, 990000)
                   .Select(l => (999999 - l).ToString())
                   .First(l => l[0] == l[5] && l[1] == l[4] && l[2] == l[3]));
        }

        static int BruteForceWay3()
        {
            int max = 0;
            for (int i = 998001; i > 10000; i--)
            {
                var l = i.ToString();
                if(l[0] == l[5] && l[1] == l[4] && l[2] == l[3])
                {
                    for (int j = 999; j > 100; j--)
                    {
                        if (i % j == 0 && i / j < 999)
                            return i;
                    }
                }
            }
            return max;
        }
    }
}
