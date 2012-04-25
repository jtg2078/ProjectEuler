using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace euler120csharp
{
    class Program
    {

        /// <summary>
        /// Let r be the remainder when (a−1)n + (a+1)n is divided by a2.
        /// For example, if a = 7 and n = 3, then r = 42: 63 + 83 = 728 ≡ 42 mod 49. 
        /// And as n varies, so too will r, but for a = 7 it turns out that rmax = 42.
        ///For 3 ≤ a ≤ 1000, find ∑ rmax.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var a = 7l;
            var div = a * a;

            for (int n = 0; n <= 15; n++)
            {
                var first = Math.Pow(a - 1, n);
                var second = Math.Pow(a + 1, n);
                var result = first + second;

                var rem = result % div;
                Console.WriteLine(string.Format("n={0} rem={1} first={2},{3}, second={4},{5}", n, rem, first, first % div, second, second % div));
            }
            Console.WriteLine("----");
            var ans = test2(a);
            Console.WriteLine("ans: " + ans);
            Console.WriteLine("----");
            ans = test3(a);
            Console.WriteLine("ans: " + ans);

            Console.WriteLine("----");
            the_one();

            Console.WriteLine("test modulus method");
            var expected = 445L;
            var z = modular_pow(4, 13, 497);
            Console.WriteLine(z == expected);


            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        static long test2(long a)
        {
            var div = a * a;
            var mask = (long)Math.Pow(10, (int)Math.Log10(div) + 2);
            var set_f = new HashSet<long>();
            var set_s = new HashSet<long>();
            var max = 0l;
            var sum = 0l;
            for (long n = 0, f = 1, s = 1, rem_f = 0, rem_s = 0; ; n++)
            {
                rem_f = f % div;
                rem_s = s % div;
                sum = (rem_f + rem_s) % div;
                if (max < sum)
                    max = sum;
                Console.WriteLine(string.Format("n={0} rem={1} first={2},{3}, second={4},{5}", n, sum, f, rem_f, s, rem_s));
                if (set_f.Add(rem_f) == false || set_s.Add(rem_s) == false)
                {
                    return max;
                }
                f = (f * (a - 1));
                s = (s * (a + 1));

            }
        }
        //332833002
        static void the_one()
        {
            var sum = 0L;
            for (int a = 3; a <= 1000; a++)
            {
                sum += test3(a);
            }
            Console.WriteLine("the answer for no.120 is: " + sum);
        }

        static long test3(long a)
        {
            var div = a * a;
            var set_f = new HashSet<long>();
            var set_s = new HashSet<long>();
            var max = 0L;
            var sum = 0L;
            //Console.WriteLine("---");
            for (int n = 0; ; n++)
            {
                var rem_f = modular_pow(a - 1, n, div);
                var rem_s = modular_pow(a + 1, n, div);
                sum = (rem_f + rem_s) % div;
                
                //Console.WriteLine(string.Format("n={0} rem={1} first={2}, second={3}", n, sum, rem_f, rem_s));
                if (set_f.Add(rem_f) == false && set_s.Add(rem_s) == false)
                {
                    return max;
                }

                if (max < sum)
                    max = sum;
            }
        }

        //    function modular_pow(base, exponent, modulus)
        //      result := 1
        //      while exponent > 0
        //          if (exponent & 1) equals 1:
        //             result = (result * base) mod modulus
        //          exponent := exponent >> 1
        //          base = (base * base) mod modulus
        //      return result

        static long modular_pow(long b, int exponent, long modulus)
        {
            long result = 1;
            while (exponent > 0)
            {
                if ((exponent & 1) == 1)
                    result = (result * b) % modulus;

                exponent >>= 1;
                b = (b * b) % modulus;
            }
            return result;
        }



    }
}
