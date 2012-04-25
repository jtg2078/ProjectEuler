using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace euler014csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();

            Console.WriteLine("The starting number which under one million and produces the longest chain");
            sw.Start();
            var sum1 = test1();
            sw.Stop();
            Console.WriteLine(string.Format("hand roll for-loop memoization brute force way(ms:{0}): {1}", sw.ElapsedMilliseconds, sum1));

            sw.Reset();

            sw.Start();
            var sum2 = test2();
            sw.Stop();
            Console.WriteLine(string.Format("no memoization for-loop brute force way(ms:{0}): {1}", sw.ElapsedMilliseconds, sum2));

            sw.Reset();

            sw.Start();
            var sum3 = test3();
            sw.Stop();
            Console.WriteLine(string.Format("cool LINQ recursive memoization brute force way(ms:{0}): {1}", sw.ElapsedMilliseconds, sum3));

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }        

        /// <summary>
        /// wow took unusually long on this question, 
        /// formulated the code to get the answer but it was wrong,
        /// finally i gave up and google for solution
        /// turns out the "current" can be greater than int, so had to use long
        /// lawl~~~~~~
        /// pretty interesting that linq recursive solution is slightly faster
        /// since its caching a lot more comparing to test1 (999999 vs 2168611 entries), probably negate 
        /// linq and recursive overhead.
        /// but then again, i cant think of a way for for loop to cache same as recurive way, cant do backtrack
        /// </summary>
        /// <returns></returns>
        static int test1()
        {
            var map = new Dictionary<long, int>();
            long current = 0;
            var count = 0;
            var max = 0;
            var max_index = 0;
            for (int i = 1; i < 1000000; i++)
            {
                current = i;
                count = 1;
                while (current > 1)
                {
                    if (map.ContainsKey(current))
                    {
                        count += map[current];
                        break;
                    }

                    current = current % 2 == 0 ?
                        current = current / 2 : current = 3 * current + 1;

                    count++;
                }
                map[i] = count;
                if (count > max)
                {
                    max = count;
                    max_index = i;
                }
            }
            return max_index;
        }

        static int test2()
        {
            long current = 0;
            var count = 0;
            var max = 0;
            var max_index = 0;
            for (int i = 1; i < 1000000; i++)
            {
                current = i;
                count = 1;
                while (current > 1)
                {
                    current = current % 2 == 0 ?
                        current = current / 2 : current = 3 * current + 1;

                    count++;
                }
                if (count > max)
                {
                    max = count;
                    max_index = i;
                }
            }
            return max_index;
        }

        static int test3()
        {
            var cache = new Dictionary<long, int>();
            var ChainCount = Memoize<long, int>(((n, self) => 
                n <= 1 ? 1 : n % 2 == 0 ? 1 + self(n / 2) : 1 + self(3 * n + 1)), cache);

            return Enumerable.Range(1, 1000000)
                .Select(n => ChainCount(n))
                .MaxIndex() + 1;
        }

        /// <summary>
        /// found on http://explodingcoder.com/cms/content/painless-caching-memoization-net
        /// written by spoulson
        /// </summary>
        public static Func<TArg, TResult> Memoize<TArg, TResult>
            (Func<TArg, Func<TArg, TResult>, TResult> function, IDictionary<TArg, TResult> cache)
        {
            Func<TArg, TResult> memoizeFunction = null;
            return memoizeFunction = key => cache.ContainsKey(key) ? cache[key] : (cache[key] = function(key, memoizeFunction));
        }
    }
    static class Extension
    {
        /// <summary>
        /// http://stackoverflow.com/questions/462699/how-do-i-get-the-index-of-the-highest-value-in-an-array-using-linq
        /// written by Jon skeet
        /// </summary>
        public static int MaxIndex<T>(this IEnumerable<T> sequence)
            where T : IComparable<T>
        {
            int maxIndex = -1;
            T maxValue = default(T); // Immediately overwritten anyway

            int index = 0;
            foreach (T value in sequence)
            {
                if (value.CompareTo(maxValue) > 0 || maxIndex == -1)
                {
                    maxIndex = index;
                    maxValue = value;
                }
                index++;
            }
            return maxIndex;
        }
    }
}
