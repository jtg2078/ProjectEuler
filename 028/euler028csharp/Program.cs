using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace euler028csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The sum of the numbers on the diagonals in a 1001 by 1001 spiral:");

            TestBenchSetup();
            TestBenchLoader(traverse_matrix_in_spiral_fashion);
            TestBenchLoader(arithmetic_calculate_corners_for_each_rim_with_formula);
            
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// 1.  basically traverse the matrix starting from top right corner
        ///     in counter-clockwise spiral fahsaion and populate the value for each
        ///     cell advanced.
        /// 2.  starting value is n*n, which n is the width and height of the matrix
        /// 3.  after matrix is populated with n*n, n*n-1, n*n-2,...,1. another loop is called
        ///     to get the cells lie within the diagonal line
        /// 4.  the top right corner value is always n*n in the domain of this problem
        /// </summary>
        /// <returns></returns>
        static int traverse_matrix_in_spiral_fashion()
        {
            var len = 1001;
            var map = new int[len, len];
            var num = len * len;
            var row = 0;
            var col = len - 1;
            var direction = 0; //0:left 1:down 2:right 3:up
            while (true)
            {
                if (num <= 1)
                    break;

                switch (direction)
                {
                    case 0:
                        if (col > 0 && map[row, col - 1] == 0)
                            map[row, col--] = num--;
                        else
                            direction = 1;
                        break;
                    case 1:
                        if (row < len - 1 && map[row + 1, col] == 0)
                            map[row++, col] = num--;
                        else
                            direction = 2;
                        break;
                    case 2:
                        if (col < len - 1 && map[row, col + 1] == 0)
                            map[row, col++] = num--;
                        else
                            direction = 3;
                        break;
                    case 3:
                        if (row > 0 && map[row - 1, col] == 0)
                            map[row--, col] = num--;
                        else
                            direction = 0;
                        break;
                }
            }
            var sum = 1;
            for (int i = 0; i < len; i++)
            {
                sum += map[i, i];
                sum += map[i, len - 1 - i];
            }
            return sum;
        }

        /// <summary>
        /// 1.  since the top right corner is n*n
        /// 2.  and top left corner is top right corner minus the len plus 1
        /// 3.  so it becomes (NE = north-east corner, and so on...) 
        ///     NE: n^2, NW: n^2-n+1, SW: n^2-2n+2, SE: n^2-3n+3
        /// 4.  so formula can be formed: n^2-cn+c, where c is corner(starting with NE=0, NW=1,...)
        /// 5.  thinking the matrix is composed of matrices, the question's sample matrix is
        ///     composed of matrix with len=5 and len=3;
        /// 6.  so starting with len=n, the next inner matrix will have len-2, so on...
        /// 7.  by calculating the corners of each matrix, and sum them up, we get the answer to this
        ///     problem!
        /// </summary>
        /// <returns></returns>
        static int arithmetic_calculate_corners_for_each_rim_with_formula()
        {
            var len = 1001;
            var sum = 1;
            var corner = 0;
            Func<int, int, int> formula = (n, c) => n * n - c * n + c;
            for (int i = 3; i <= len; i += 2)
            {
                corner = 0;
                sum += formula(i, corner++); // NE corner
                sum += formula(i, corner++); // NW
                sum += formula(i, corner++); // SW
                sum += formula(i, corner++); // SE
            }
            return sum;
        }

        static Stopwatch stopwatch = new Stopwatch();
        static void TestBenchSetup()
        {
            // Uses the second Core or Processor for the Test
            Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(2);
            // Prevents "Normal" processes from interrupting Threads
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            // Prevents "Normal" Threads from interrupting this thread
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
        }
        // see http://www.codeproject.com/KB/testing/stopwatch-measure-precise.aspx
        static void TestBenchLoader(Func<int> test_method)
        {
            stopwatch.Reset();
            stopwatch.Start();
            var result = 0;
            long avg_tick = 0;
            long avg_ms = 0;
            while (stopwatch.ElapsedMilliseconds < 1200)  // A Warmup of 1000-1500 ms 
            // stabilizes the CPU cache and pipeline.
            {
                result = test_method(); // Warmup
            }
            stopwatch.Stop();
            for (int repeat = 0; repeat < 20; ++repeat)
            {
                stopwatch.Reset();
                stopwatch.Start();
                result = test_method();
                stopwatch.Stop();
                avg_tick += stopwatch.ElapsedTicks;
                avg_ms += stopwatch.ElapsedMilliseconds;
            }
            avg_tick = avg_tick / 20;
            avg_ms = avg_ms / 20;
            Console.WriteLine(string.Format("{0} way(ticks:{1}, ms:{2}) Ans:{3}",
                test_method.Method.Name.Replace('_', ' '), avg_tick, avg_ms, result));
        }
    }
}
