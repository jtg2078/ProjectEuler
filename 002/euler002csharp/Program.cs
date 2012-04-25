using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace euler002csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();

            Console.WriteLine("The sum of all the even-valued terms in Fibonacci sequence under 4mil");
            sw.Start();
            var sum1 = BruteForceWay(4000000);
            sw.Stop();
            Console.WriteLine(string.Format("While loop brute force way(tick:{0}): {1}", sw.ElapsedTicks, sum1));

            sw.Reset();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        static long BruteForceWay(int x)
        {
            int old = 1;
            int cur = 1;
            int next = 0;
            long sum = 0;

            while (cur < x)
            {
                if (cur % 2 == 0)
                {
                    Console.WriteLine(cur);
                    sum += cur;
                }
                
                next = cur + old;
                old = cur;
                cur = next;
            }

            return sum;
        }
    }
}
