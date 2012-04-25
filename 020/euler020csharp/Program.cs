using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace euler020csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();

            Console.WriteLine("Find the sum of the digits in the number 100!");
            sw.Start();
            var sum1 = factorial(100);
            sw.Stop();
            Console.WriteLine(string.Format("for loop and Schönhage–Strassen multiplication(tick:{0}): {1}", sw.ElapsedTicks, sum1));

            sw.Reset();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// store number in int array, and use Schönhage–Strassen algorithm to do multiplication
        /// the reason to do this is because the number would get far bigger than double or int64
        /// </summary>
        /// <param name="max">the last number, in this problem 100</param>
        /// <returns>sum of all </returns>
        static int factorial(int max)
        {
            var a = new int[1] { 1 };
            var b = new int[max.ToString().Length];
            var num = 0;
            for (int i = 1; i <= max; i++)
            {
                num = i;
                for (int j = b.Length - 1; j >= 0; j--)
                {
                    b[j] = num % 10;
                    num = num / 10;
                }
                a = multiply(a, b);
                //Console.WriteLine(a.Aggregate(string.Empty, (str, n) => str += n));
            }
            return a.Sum();
        }

        /// <summary>
        /// Schönhage–Strassen algorithm
        /// http://en.wikipedia.org/wiki/Sch%C3%B6nhage%E2%80%93Strassen_algorithm
        /// </summary>
        /// <param name="a">previous number</param>
        /// <param name="b">next number to multiply</param>
        /// <returns>the production in array</returns>
        static int[] multiply(int[] a, int[] b)
        {
            var result = new int[a.Length + b.Length];
            var product = 0;
            var index = result.Length - 1;
            var shift = 0;

            for (int i = a.Length - 1; i >= 0; i--)
            {
                shift = 0;
                for (int j = b.Length - 1; j >= 0; j--)
                {
                    product = a[i] * b[j];
                    result[index - shift] += product;
                    shift++;
                }
                index--;
            }

            var carry = 0;
            for (int i = result.Length - 1; i >= 0; i--)
            {
                result[i] += carry;
                carry = result[i] / 10;
                result[i] = result[i] % 10;
            }

            return result.SkipWhile(l => l == 0).ToArray<int>();
        }
    }
}
