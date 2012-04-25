using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace euler001csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();
            
            Console.WriteLine("The sum of all the multiples of 3 or 5 below 1000");
            sw.Start();
            var sum1 = BruteForceWay(999);
            sw.Stop();
            Console.WriteLine(string.Format("LINQ brute force way(tick:{0}): {1}", sw.ElapsedTicks, sum1));
            
            sw.Reset();
            
            sw.Start();
            var sum2 = SmarterAritmeticWay(999);
            sw.Stop();
            Console.WriteLine(string.Format("Smart arithmetic way(tick:{0}): {1}", sw.ElapsedTicks, sum2));

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        static int BruteForceWay(int x)
        {
            return Enumerable.Range(1, x)
                    .Select(n => n % 3 == 0 || n % 5 == 0 ? n : 0)
                    .Aggregate((agg, cur) => agg += cur);
        }

        /// <summary>
        /// to find the sum of the terms 3,6,9,12,..., you would use (n/2)(a+l), 
        /// where n is the number of terms, a is the first term, and l is the last term. 
        /// But to find the last term requires a bit of work. 
        /// The nth term is given by a+(n-1)d, where d is the common difference. 
        /// So we need to solve 3+3(n-1)=1000, giving 3(n-1)=997, and n=997/3+1=333.333... 
        /// However, n must be integer, so int(333.333...)=333, and checking, 3+3(333-1)=999; 
        /// this is the last term before 1000.
        /// 
        /// In general, a+(n-1)d=x, gives n=int((x-a)/d+1). 
        /// But for this problem we can improve even further, as a=d we get n=int(x/a-a/a+1)=int(x/a). 
        /// The nth (last) term, l=a+(n-1)d=d+(int(x/d)-1)*d=d*int(x/d).
        /// 
        /// Combining this to find the sum, S=(n/2)(a+l)=(int(x/a)/2)*(a+a*int(x/a)). 
        /// </summary>
        /// <param name="x">ceiling</param>
        /// <returns>sum of 3s and 5s below ceiling</returns>
        static int SmarterAritmeticWay(int x)
        {
            // should be 3 * 333 * (1 + 333)) / 2 = 166833
            var sumOf3 = Formula(3, x);
            // should be 5 * 199 * (1 + 199)) / 2 = 199000
            var sumof5 = Formula(5, x);
            // since 3 and 5  produce duplicate (like 15)
            // so we need to substract them
            // should be (15 * 66 * (1 + 66)) / 2 = 33165
            var sumOf15 = Formula(15, x);

            return sumOf3 + sumof5 - sumOf15;
        }

        /// <summary>
        /// wow subtle differences, if we do division too early, we lose precision(since its int)
        /// for a = 3 and x = 999
        /// ((x / a) / 2) * ((1 + (x / a)) * a) = 166832
        /// (x / a) * (1 + x / a) * a / 2 = 166833
        /// </summary>
        /// <param name="a">the term difference</param>
        /// <param name="x">ceiling</param>
        /// <returns>sum</returns>
        static int Formula(int a, int x)
        {
            return (x / a) * (1 + x / a) * a / 2;
        }
    }
}
