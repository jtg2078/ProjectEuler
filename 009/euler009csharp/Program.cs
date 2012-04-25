using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace euler009csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();

            Console.WriteLine("There exists exactly one Pythagorean triplet for which a + b  + c = 1000, Find the product abc");
            sw.Start();
            var product1 = BruteForce();
            sw.Stop();
            Console.WriteLine(string.Format("plain loop brute force way(tick:{0}): {1}", sw.ElapsedTicks, product1));

            sw.Reset();

            Console.WriteLine("There exists exactly one Pythagorean triplet for which a + b  + c = 1000, Find the product abc");
            sw.Start();
            var product2 = ArithmeticWay();
            sw.Stop();
            Console.WriteLine(string.Format("Euclid's formula way, with k(tick:{0}): {1}", sw.ElapsedTicks, product2));


            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// plain loop all possibilities way
        /// a or b cannot exceed 500, so set that as bound
        /// and since a+b+c has to be 1000, which makes c = (1000-a-b)
        /// and a^2 + b^2 = c^2, so two nested loops are enough
        /// </summary>
        /// <returns></returns>
        static int BruteForce()
        {
            for (int a = 1; a < 500; a++)
            {
                for (int b = a + 1; b < 500; b++)
                {
                    if (a * a + b * b == (1000 - a - b) * (1000 - a - b))
                    {
                        return a * b * (1000 - a - b);
                    }
                }
            }

            return 0;
        }

        /// <summary>
        /// using the Euclid's formula with k, see http://en.wikipedia.org/wiki/Pythagorean_triple
        /// a = k * (m^2 - n^2)
        /// b = k * (2mn)
        /// c = k * (m^2 + n^2)
        /// starting with k=1
        /// (m^2 - n^2) + (2mn) + (m^2 + n^2) = 1000
        /// (2m^2 + 2mn) = 1000
        /// m^2 + mn = 500
        /// since m and n has to be integer and non-zero
        /// so 500 has to be divisible by k and
        /// we can set the m_bound to be less or equal to Math.Sqrt(k_bound), since n cannot be zero
        /// </summary>
        /// <returns></returns>
        static int ArithmeticWay()
        {
            int k_bound = 0;
            int m_bound = 0;
            for (int k = 1; k <= 500; k++)
            {
                if (500 % k == 0)
                {
                    k_bound = 500 / k;
                    m_bound = (int)Math.Sqrt(k_bound);

                    for (int m = 1; m <= m_bound; m++)
                    {
                        if (k_bound % m == 0)
                        {
                            int n = k_bound / m - m;
                            if (m > n)
                                return k * (m * m - n * n) * k * (2 * m * n) * k * (m * m + n * n);
                        }
                    }
                }
            }
            return 0;
        }
    }

    class PrimitiveTriplet
    {
        static int order = 0;

        struct Triplet
        {
            public int a;
            public int b;
            public int c;
        }

        static void RecursiveMap(int a, int b, int c, int count)
        {
            order++;
            //Console.WriteLine(order + ". " + a + " " + b + " " + c + " count: " + count);

            if (a + b + c == 1000)
                Console.WriteLine(order + ". " + a + " " + b + " " + c + " count: " + count);


            var children = ParentChildRelationships(a, b, c);
            count++;

            foreach (var child in ParentChildRelationships(a, b, c))
            {
                if (child.a + child.b + child.c <= 1000)
                    RecursiveMap(child.a, child.b, child.c, count);
            }

            //RecursiveMap(children[0].a, children[0].b, children[0].c, count);
            //RecursiveMap(children[1].a, children[1].b, children[1].c, count);
            //RecursiveMap(children[2].a, children[2].b, children[2].c, count);
        }

        static Triplet[] ParentChildRelationships(int a, int b, int c)
        {
            var p1 = new Triplet();
            p1.a = a - 2 * b + 2 * c;
            p1.b = 2 * a - b + 2 * c;
            p1.c = 2 * a - 2 * b + 3 * c;

            var p2 = new Triplet();
            p2.a = a + 2 * b + 2 * c;
            p2.b = 2 * a + b + 2 * c;
            p2.c = 2 * a + 2 * b + 3 * c;

            var p3 = new Triplet();
            p3.a = -a + 2 * b + 2 * c;
            p3.b = -2 * a + b + 2 * c;
            p3.c = -2 * a + 2 * b + 3 * c;

            return new Triplet[3] { p1, p2, p3 };
        }
    }
}
