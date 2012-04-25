using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace euler017csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();

            Console.WriteLine("How many letters would be used in number range from 1 to 1000?");
            sw.Start();
            var sum1 = Test2(1000);
            sw.Stop();
            Console.WriteLine(string.Format("plain arithmetic brute force way(tick:{0}): {1}", sw.ElapsedTicks, sum1));

            sw.Reset();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        static int Test2(int max)
        {
            var sum = 0;
            var num = 0;
            for (int i = 1; i <= max; i++)
            {
                num = i;
                if (num >= 1000)
                {
                    sum += 11;
                    continue;
                }

                if (num >= 100)
                {
                    sum += (map_zero_to_nineteen[num / 100] + 7);
                    num = num % 100;
                    if (num > 0)
                        sum += 3;
                }

                if (num < 20)
                    sum += map_zero_to_nineteen[num];
                else
                    sum += (map_twenty_to_ninety[num / 10] + map_zero_to_nineteen[num % 10]);
            }
            return sum;
        }

        static readonly int[] map_zero_to_nineteen = new int[]
        {
            0, //place holder for 0
            "one"       .Length,
            "two"       .Length,  
            "three"     .Length,
            "four"      .Length, 
            "five"      .Length, 
            "six"       .Length, 
            "seven"     .Length,
            "eight"     .Length,
            "nine"      .Length,
            "ten"       .Length,     
            "eleven"    .Length,   
            "twelve"    .Length,   
            "thirteen"  .Length, 
            "fourteen"  .Length, 
            "fifteen"   .Length,  
            "sixteen"   .Length,  
            "seventeen" .Length,
            "eighteen"  .Length, 
            "nineteen"  .Length 
        };

        static readonly int[] map_twenty_to_ninety = new int[]
        {
            0, //place holder for 0
            0, //place holder for 1(10)
            "twenty"  .Length,     
            "thirty"  .Length,   
            "forty"   .Length,   
            "fifty"   .Length, 
            "sixty"   .Length, 
            "seventy" .Length,  
            "eighty"  .Length,  
            "ninety"  .Length,
        };
    }
}
