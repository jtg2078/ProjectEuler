using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace euler008csharp
{
    class Program
    {
        static readonly char[] number = @"73167176531330624919225119674426574742355349194934
                                          96983520312774506326239578318016984801869478851843
                                          85861560789112949495459501737958331952853208805511
                                          12540698747158523863050715693290963295227443043557
                                          66896648950445244523161731856403098711121722383113
                                          62229893423380308135336276614282806444486645238749
                                          30358907296290491560440772390713810515859307960866
                                          70172427121883998797908792274921901699720888093776
                                          65727333001053367881220235421809751254540594752243
                                          52584907711670556013604839586446706324415722155397
                                          53697817977846174064955149290862569321978468622482
                                          83972241375657056057490261407972968652414535100474
                                          82166370484403199890008895243450658541227588666881
                                          16427171479924442928230863465674813919123162824586
                                          17866458359124566529476545682848912883142607690042
                                          24219022671055626321111109370544217506941658960408
                                          07198403850962455444362981230987879927244284909188
                                          84580156166097919133875499200524063689912560717606
                                          05886116467109405077541002256983155200055935729725
                                          71636269561882670428252483600823257530420752963450".Replace(" ","")
                                                                                             .Replace(Environment.NewLine, "")
                                                                                             .ToCharArray();

        static void Main(string[] args)
        {
            var sw = new Stopwatch();

            Console.WriteLine("The greatest product of five consecutive digits in the 1000-digit number");
            sw.Start();
            var product1 = BruteForce();
            sw.Stop();
            Console.WriteLine(string.Format("plain loop and compare brute force way(tick:{0}): {1}", sw.ElapsedTicks, product1));
            sw.Reset();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// plain loop through
        /// moving in chunk of 5: [0][1][2][3][4], [1][2][3][4][5],...,[n-5][n-4][n-3][n-2][n-1]
        /// checking zero doesnt really help in performance(unmeasurable)
        /// i guess the compiler and cpu branch prediction are working at best :)
        /// takes around 800ish tick in my machine(core i7 920 @ 2.6ghz, 3gb ddr3)
        /// answer is 40824
        /// ps. the -48 is because the Convert.ToInt32 would return ascii value of the number(so 7 becomes 55)
        /// this is gazillion faster than doing num1 = number[0 + i] - '0', probably because of all the boxing involved
        /// too bad i cant get the profile to work(vs2008 + core i7 + profiler driver = BSOD)
        /// </summary>
        /// <returns></returns>
        static int BruteForce()
        {
            int len = number.Length - 4;
            int curMax = 0;
            int cur = 0;
            int num1 = 0;
            int num2 = 0;
            int num3 = 0;
            int num4 = 0;
            int num5 = 0;

            for (int i = 0; i < len; i++)
            {
                num1 = Convert.ToInt32(number[0 + i]) - 48;
                num2 = Convert.ToInt32(number[1 + i]) - 48;
                num3 = Convert.ToInt32(number[2 + i]) - 48;
                num4 = Convert.ToInt32(number[3 + i]) - 48;
                num5 = Convert.ToInt32(number[4 + i]) - 48;
                cur = num1 * num2 * num3 * num4 * num5;
                if (cur > curMax)
                {
                    curMax = cur;
                }
            }

            return curMax;
        }
    }
}
