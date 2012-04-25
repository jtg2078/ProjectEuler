using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace euler024csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();

            Console.WriteLine("The millionth lexicographic permutation of the digits 0, 1, 2, 3, 4, 5, 6, 7, 8 and 9?");
            sw.Start();
            var sum1 = test1();
            sw.Stop();
            Console.WriteLine(string.Format("using Knuth's algorithm way(ms:{0}): {1}", sw.ElapsedMilliseconds, sum1));

            sw.Reset();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// see http://en.wikipedia.org/wiki/Permutation#Algorithms_to_generate_permutations
        /// on section of "Systematic generation of all permutations"
        /// </summary>
        /// <returns></returns>
        static string test1()
        {
            var a = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var j = 0;
            var l = 0;
            var tmp = 0;
            var len = a.Length;
            var z = 0;
            var counter = 999999; // since the 1st perumation is "a" itself

            while (--counter >= 0)
            {
                //step 1. Find the largest index j such that a[j] < a[j + 1].
                for (j = len - 2; j >= 0; j--)
                {
                    if (a[j] < a[j + 1])
                        break;
                }
                if (j == -1)
                    break; // no more permutation, since no such index exist

                //step 2. Find the largest index l such that a[j] < a[l]
                for (l = len - 1; l >= 0; l--)
                {
                    if (a[j] < a[l])
                        break;
                }

                //step 3. Swap a[j] with a[l].
                tmp = a[j];
                a[j] = a[l];
                a[l] = tmp;

                //step 4. Reverse the sequence from a[j + 1] up to and including the final element a[n]
                z = len - 1;
                for (l = j + 1; l < len; l++)
                {
                    if (l >= z)
                        break;

                    tmp = a[l];
                    a[l] = a[z];
                    a[z] = tmp;
                    z--;
                }
            }
            return a.Aggregate(string.Empty, (s, n) => s += n);
        }
    }
}
