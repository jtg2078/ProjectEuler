using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace euler037csharp
{
    class Additional
    {
        static bool isPrime(int n)
        {
            if (n <= 1)
                return false;

            var limit = (int)Math.Sqrt(n) + 1;
            for (int i = 2; i < limit; i++)
            {
                if (n % i == 0)
                    return false;
            }
            return true;
        }

        static void leftprime()
        {
            var c = new List<int>() { 2, 3, 5, 7 };
            int left;
            var start = 0;
            var end = 0;
            var index = 0;
            var tens = 10;
            while (end < 5)
            {
                end = c.Count;
                for (index = start; index < end; index++)
                {
                    for (int i = 1; i < 10; i++)
                    {
                        left = i * tens + c[index];
                        if (isPrime(left))
                        {
                            Console.WriteLine(left);
                            c.Add(left);
                        }
                    }
                }
                start = end;
                tens *= 10;
            }
        }

        static void rightprime()
        {
            var c = new List<int>() { 2, 3, 5, 7 };
            int right;
            var start = 0;
            var end = 0;
            var index = 0;
            var tens = 10;
            while (end < 5)
            {
                end = c.Count;
                for (index = start; index < end; index++)
                {
                    for (int i = 1; i < 10; i++)
                    {
                        right = c[index] * 10 + i;
                        if (isPrime(right))
                        {
                            Console.WriteLine(right);
                            c.Add(right);
                        }
                    }
                }
                start = end;
                tens *= 10;
            }
        }
    }
}
