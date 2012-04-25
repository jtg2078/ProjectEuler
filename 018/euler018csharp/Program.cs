using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace euler018csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();

            Console.WriteLine("The maximum total from top to bottom of the triangle below:");
            sw.Start();
            var sum1 = test();
            sw.Stop();
            Console.WriteLine(string.Format("cascading addition arithmetic way(tick:{0}): {1}", sw.ElapsedTicks, sum1));

            sw.Reset();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        // i <3 alt select + copy/paste
        static int[][] map = new int[][]
        {
            new int[] {75},
            new int[] {95, 64},
            new int[] {17, 47, 82},
            new int[] {18, 35, 87, 10},
            new int[] {20, 04, 82, 47, 65},
            new int[] {19, 01, 23, 75, 03, 34},
            new int[] {88, 02, 77, 73, 07, 63, 67},
            new int[] {99, 65, 04, 28, 06, 16, 70, 92},
            new int[] {41, 41, 26, 56, 83, 40, 80, 70, 33},
            new int[] {41, 48, 72, 33, 47, 32, 37, 16, 94, 29},
            new int[] {53, 71, 44, 65, 25, 43, 91, 52, 97, 51, 14},
            new int[] {70, 11, 33, 28, 77, 73, 17, 78, 39, 68, 17, 57},
            new int[] {91, 71, 52, 38, 17, 14, 91, 43, 58, 50, 27, 29, 48},
            new int[] {63, 66, 04, 68, 89, 53, 67, 30, 73, 16, 69, 87, 40, 31},
            new int[] {04, 62, 98, 27, 23, 09, 70, 98, 73, 93, 38, 53, 60, 04, 23},
        };

        //static int[][] map = new int[][]
        //{
        //    new int[] {3},          new int[] {3},           new int[] {3},         *new int[] {23},
        //    new int[] {7,4},    ->  new int[] {7,4},     -> *new int[] {20,19},   -> new int[] {20,19},    
        //    new int[] {2,4,6},     *new int[] {10,13,15},    new int[] {10,13,15},   new int[] {10,13,15},
        //   *new int[] {8,5,9,3},    new int[] {8,5,9,3},     new int[] {8,5,9,3},    new int[] {8,5,9,3},
        //};

        /// <summary>
        /// replace each cell in each row with higher sum of cell + right or left children, so the follow
        ///  2             (2+8)             10
        /// 8  5 becomes  8     5 becomes   8   5
        /// eventually the 1st cell of the whole thing would contain the highest sum (see above)
        /// </summary>
        /// <returns></returns>
        static int test()
        {
            var left = 0;
            var right = 0;
            for (int row = map.Length - 2; row >= 0; row--)
            {
                for (int col = 0; col < map[row].Length; col++)
                {
                    left = map[row + 1][col];
                    right = map[row + 1][col + 1];
                    map[row][col] = map[row][col] + (left > right ? left : right);
                }
            }
            return map[0][0];
        }
    }
}
