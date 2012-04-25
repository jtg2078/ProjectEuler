using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace euler056csharp
{
    class pure_array_base
    {
        //static int test99()
        //{
        //    var max = 0;
        //    var sum = 0;
        //    for (int i = 1; i < 100; i++)
        //    {
        //        sum = exponentiation_by_squaring(i, i);
        //        if (sum > max)
        //            max = sum;
        //    }
        //    return max;
        //}


        //static void test100()
        //{
        //    var max = 0;
        //    var sum = 0;
        //    int a, b;
        //    using (StreamWriter sw = File.CreateText("result.txt"))
        //    {
        //        for (a = 1; a < 100; a++)
        //        {
        //            for (b = 1; b < 100; b++)
        //            {
        //                sum = exponentiation_by_squaring(a, b);
        //                if (sum > max)
        //                    max = sum;
        //                sw.WriteLine(string.Format("{0}^{1} sum:{2}", a, b, sum));
        //            }
        //        }
        //        sw.WriteLine("max is: " + max);
        //    }
        //}



        static int exponentiation_by_squaring2(int num, int power)
        {
            var r = new List<int>() { 1 };
            var n = new List<int>();
            var t = new List<int>();
            num_to_array2(num, n);
            while (power > 0)
            {
                if ((power & 1) == 1)
                    multiply2(r, n, t);

                power >>= 1;
                multiply2(n, n, t);
            }
            return r.Sum();
        }

        static int exponentiation_by_squaring3(int num, int power)
        {
            var r = new int[400];
            var r_len = 0;
            r[r_len++] = 1;
            var n = new int[400];
            var n_len = 0;
            var t = new int[400];
            num_to_array3(num, n, ref n_len);
            while (power > 0)
            {
                if ((power & 1) == 1)
                    multiply3(r, ref r_len, n, n_len, t);

                power >>= 1;
                multiply3(n, ref n_len, n, n_len, t);
            }
            return r.Sum();
        }

        static int test992()
        {
            var max = 0;
            var sum = 0;
            for (int i = 1; i < 100; i++)
            {
                sum = exponentiation_by_squaring2(i, i);
                if (sum > max)
                    max = sum;
            }
            return max;
        }

        static int test999()
        {
            var max = 0;
            var sum = 0;
            for (int i = 1; i < 100; i++)
            {
                sum = exponentiation_by_squaring3(i, i);
                if (sum > max)
                    max = sum;
            }
            return max;
        }

        static void test()
        {
            var r = new int[200];
            r[0] = 1;
            r[1] = 2;
            r[2] = 3;
            //r[3] = 4;
            var r_len = 3;
            var n = new int[200];
            n[0] = 4;
            n[1] = 5;
            n[2] = 6;
            //n[3] = 8;
            //n[4] = 9;
            var n_len = 3;
            var t = new int[200];
            multiply3(r, ref r_len, n, n_len, t);

        }

        static void testz()
        {
            var r = new int[3];
            r[0] = 1;
            r[1] = 2;
            r[2] = 3;
            //r[3] = 4;
            var r_len = 3;
            var n = new int[3];
            n[0] = 4;
            n[1] = 5;
            n[2] = 6;
            //n[3] = 8;
            //n[4] = 9;
            var n_len = 3;
            var t = new int[200];
            //multiply(r, n);

        }

        static void multiply2(List<int> a, List<int> b, List<int> result)
        {
            result.Clear();
            int shift, product, i, j;
            var index = 0;
            var a_len = a.Count;
            var b_len = b.Count;
            for (i = 0; i < a_len; i++)
            {
                shift = index;
                for (j = 0; j < b_len; j++)
                {
                    product = a[i] * b[j];
                    if (shift >= result.Count)
                        result.Add(0);
                    result[shift] += product;
                    shift++;
                }
                index++;
            }

            var carry = 0;
            for (i = 0; i < result.Count; i++)
            {
                result[i] += carry;
                carry = result[i] / 10;
                result[i] = result[i] % 10;
            }
            if (carry > 0)
                result.Add(carry);
            a.Clear();
            result.ForEach(n => a.Add(n));
        }

        static void multiply3(int[] a, ref int a_len, int[] b, int b_len, int[] r)
        {
            int shift, product, i, j;
            var index = 0;
            shift = 0;
            for (i = 0; i < 200; i++)
            {
                r[i] = 0;
            }
            for (i = 0; i < a_len; i++)
            {
                shift = index;
                for (j = 0; j < b_len; j++)
                {
                    product = a[i] * b[j];
                    r[shift] += product;
                    shift++;
                }
                index++;
            }

            a_len = 0;
            for (i = shift - 1; i >= 0; i--)
            {
                a[a_len++] = r[i];
            }

            var carry = 0;


            for (i = 0; i < a_len; i++)
            {
                a[i] += carry;
                carry = a[i] / 10;
                a[i] = a[i] % 10;
            }
            if (carry > 0)
            {
                a[a_len++] = carry;
            }
            //for (i = 0; i < shift; i++)
            //{
            //    a[i] = r[i];
            //}
            //a_len = shift;
        }

        static void num_to_array2(int n, List<int> result)
        {
            result.Clear();
            do
            {
                result.Add(n % 10);
            } while ((n /= 10) > 0);
        }

        static void num_to_array3(int n, int[] result, ref int r_len)
        {
            for (int i = 0; i < r_len; i++)
            {
                result[i] = 0;
            }
            r_len = 0;
            do
            {
                result[r_len++] = n % 10;
                //result.Add(n % 10);
            } while ((n /= 10) > 0);
        }
    }
}
