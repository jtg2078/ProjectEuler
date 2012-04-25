using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace euler012csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();

            Console.WriteLine("The value of the first triangle number to have over five hundred divisors?");
            sw.Start();
            var sum1 = FindFactorCountTriangleNum1(500);
            sw.Stop();
            Console.WriteLine(string.Format("linq brute force way(tick:{0} milsec:{1}): {2}"
                , sw.ElapsedTicks, sw.ElapsedMilliseconds, sum1));

            sw.Reset();

            sw.Start();
            var sum2 = FindFactorCountTriangleNum2(500);
            sw.Stop();
            Console.WriteLine(string.Format("for loop brute force way(tick:{0} milsec:{1}): {2}"
                , sw.ElapsedTicks,sw.ElapsedMilliseconds, sum2));

            sw.Reset();

            sw.Start();
            var sum3 = FactorCount();
            sw.Stop();
            Console.WriteLine(string.Format("smart GDC arithmetic way(tick:{0} milsec:{1}): {2}"
                , sw.ElapsedTicks, sw.ElapsedMilliseconds, sum3));

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        static int FindFactorCountTriangleNum1(int term)
        {
            var sum = 0;
            var count = 0;
            for (int i = 1; i < int.MaxValue; i++)
            {
                sum += i;

                count = Enumerable.Range(1, (int)Math.Sqrt(sum))
                    .Where(l => sum % l == 0)
                    .Select(l => sum / l != l ? 2 : 1)
                    .Sum();

                if (count > term)
                    return sum;
            }
            return sum;
        }

        static int FindFactorCountTriangleNum2(int term)
        {
            var sum = 0;
            var count = 0;
            var upper = 0;
            for (int i = 1; i < int.MaxValue; i++)
            {
                sum += i;
                count = 0;
                upper = (int)Math.Sqrt(sum);
                for (int j = 1; j <= upper; j++)
                {
                    if (sum % j == 0)
                    {
                        count++;
                        if (sum / j != j)
                            count++;
                    }
                }

                if (count > term)
                    return sum;
            }
            return sum;
        }

        /// <summary>
        /// found this on problem 12 forum, i still dont get it
        /// 
        /// Considering the number n*(n+1)/2
        /// and let d(x) denote the number of divisors of x
        /// First of all the g.c.d(n,n+1)=1
        /// So if 2/n g.c.d(n/2,n+1)=1
        /// and d(n*(n+1)/2)=d(n/2)*d(n+1)
        /// Otherwise d(n*(n+1)/2)=d((n+1)/2)*d(n)
        /// </summary>
        /// <returns></returns>
        static int FactorCount()
        {
            int limit = 500;
            int cnt = 0;
            for (int i = 1; cnt <= limit; i++)
            {
                if (i % 2 == 0) 
                    cnt = count(i / 2) * count(i + 1);
                else 
                    cnt = count(i) * count((i + 1) / 2);
                
                if (cnt > 500)
                    return  i*(i+1)/2;
            }
            return 0;
        }

        static int count(int n)
        {
            int result = 0;
            for (int i = 1; i * i <= n; i++)
            {
                if (n % i == 0)
                {
                    result += 2;
                    if (n / i == i)
                        result--;
                }
            }
            return result;
        }
    }
}
