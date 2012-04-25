using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace euler058csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The ratio of primes along both diagonals first falls below 10%: ");

            TestBenchLoader(parallelized_corner_number_generation_and_primality_test);
            TestBenchSetup(); // moved to here so it wont hinder the multithreaded version 
            TestBenchLoader(generate_each_corner_and_test_for_primality_and_ratio);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
        /// <summary>
        /// this problem is really similar to question 28, so the forumla 
        /// n^2-cn+c, where c is corner(starting with SE=0, SW=1, NW=2, NE=2) can be reused :)
        /// since the corner number gets too large in this problem, generating seive is not feasible
        /// so the old dividing all possible factors primality test is used, and this is where most of the time
        /// is consumed
        /// </summary>
        public static int generate_each_corner_and_test_for_primality_and_ratio()
        {
            var prime_counter = 0.0;
            var total_counter = 1.0;
            var ratio = 0.0;
            int i = 3; //the length, it will always be in odd number
            Func<long, long, long> formula = (n, c) => n * n - c * n + c;
            while (true)
            {
                if (is_prime(formula(i, 0))) // SE corner
                    prime_counter++;

                if (is_prime(formula(i, 1))) // SW
                    prime_counter++;

                if (is_prime(formula(i, 2))) // NW
                    prime_counter++;

                if (is_prime(formula(i, 3))) // NE
                    prime_counter++;

                total_counter += 4;
                ratio = prime_counter / total_counter;
                if (ratio < 0.1)
                    return i;

                i += 2;
            }
        }

        class WorkItem
        {
            public int size;
            public int corner;
            public int start_num;
            public bool[,] isPrime;
            public AutoResetEvent signal;
        }

        /// <summary>
        /// the multithreaded version, basically divides the corners into 4 separate threads(threadpool)
        /// and processing them in chunks(where the variable chunk_size is controlling)
        /// in this method, the chunk size is 1k, so each corner of each matrix length(3,5,7,9...)
        /// is generated and tested for primality and stored in the result matrix.
        /// after all the corners are tested, the main thread loop though the result and calculates the ratio
        /// performance is a little lower than 3x better compared with single threaded version
        /// the code is programmed in such way that no lock is needed
        /// </summary>
        public static int parallelized_corner_number_generation_and_primality_test()
        {
            var prime_counter = 0.0;
            var total_counter = 1.0;
            var ratio = 0.0;
            var num = 3;
            var chunk_size = 1000;
            var result_size = chunk_size / 2;
            var result = new bool[4, result_size];
            var work_items = Enumerable.Range(0, 4) //creates the workitem list(4 items, one for each corner)
                   .Select(w => new WorkItem
                   {
                       size = chunk_size,
                       corner = w,
                       start_num = num,
                       isPrime = result,
                       signal = new AutoResetEvent(false)
                   }).ToList();

            while (true)
            {
                foreach (var item in work_items)
                {
                    ThreadPool.QueueUserWorkItem((o) =>
                    {
                        var work_item = o as WorkItem;
                        var start = work_item.start_num;
                        var end = start + work_item.size;
                        var corner = work_item.corner;
                        var index = 0;
                        var i = 0;
                        for (i = start; i < end; i += 2) //only half of the chunk is calculated(skips even number)
                        {
                            work_item.isPrime[corner, index++] = is_prime(calculate_corner(i, corner));
                        }

                        work_item.signal.Set();
                    }, item);
                }

                work_items.ForEach(w => w.signal.WaitOne());

                for (int i = 0; i < result_size; i++) //ratio calculation
                {
                    if (result[0, i])
                        prime_counter++;

                    if (result[1, i])
                        prime_counter++;

                    if (result[2, i])
                        prime_counter++;

                    if (result[3, i])
                        prime_counter++;

                    total_counter += 4;
                    ratio = prime_counter / total_counter;
                    if (ratio < 0.1)
                        return num + i * 2;
                }

                num += chunk_size; //advance the chunk starting number 
                work_items.ForEach(w => //prep the work items for next chunk calculation
                {
                    w.start_num = num; 
                    w.signal.Reset();
                });
            }
        }

        static long calculate_corner(long n, int c)
        {
            return n * n - c * n + c;
        }

        static bool is_prime(long n)
        {
            if (n <= 1)
                return false;
            var limit = (long)Math.Sqrt(n) + 1;
            for (long i = 2; i < limit; i++)
            {
                if (n % i == 0)
                    return false;
            }
            return true;
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
