using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace euler102csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The number of triangles for which the interior contains the origin: ");

            TestBenchSetup();
            TestBenchLoader(using_vector_cross_product_to_get_z_vector_direction);

            Console.WriteLine("press any key to exit");
            Console.ReadKey();
        }

        struct Point
        {
            public int x;
            public int y;
            public Point(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        /// <summary>
        /// using the info from
        /// http://stackoverflow.com/questions/2049582/how-to-determine-a-point-in-a-triangle
        /// http://mathforum.org/library/drmath/view/54505.html
        /// </summary>
        static int using_vector_cross_product_to_get_z_vector_direction()
        {
            // funtion to check if the Up vector is point in/out to the screen
            Func<Point, Point, Point, int> calculate_z_vector = (p1, p2, p3)
                => (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
            // use the calculate_z_vector to find out z vector's direction(+/-)
            var coords = new int[6];
            var origin = new Point(0, 0);
            var count = 0;
            foreach (var line in File.ReadAllLines(@"..\..\triangles.txt"))
            {
                coords = line.Split(new char[] { ',' }).Select(s => Convert.ToInt32(s)).ToArray();
                Point t1 = new Point(coords[0], coords[1]);
                Point t2 = new Point(coords[2], coords[3]);
                Point t3 = new Point(coords[4], coords[5]);
                // since the order of the point can be clockwise or counter-clockwise, test both scenario by
                // making sure the z vector sign is consistent respectively with other two sides
                var z1 = calculate_z_vector(origin, t1, t2) < 0;
                var z2 = calculate_z_vector(origin, t2, t3) < 0;
                var z3 = calculate_z_vector(origin, t3, t1) < 0;

                if (z1 == z2 && z2 == z3)
                    count++;
            }
            return count;
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
            long result = 0;
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
