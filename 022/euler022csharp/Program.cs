using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace euler022csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();

            Console.WriteLine("The total of all the name scores in the file?");
            sw.Start();
            var sum1 = test();
            sw.Stop();
            Console.WriteLine(string.Format("plain read-sort-calculate way(tick:{0}): {1}", sw.ElapsedTicks, sum1));

            sw.Reset();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
        static readonly int ASCIIA = 96; //since in this case a = 1
        /// <summary>
        /// looks like the built-in sort is quick sort
        /// http://msdn.microsoft.com/en-us/library/b0zbh7b6%28VS.96%29.aspx
        /// </summary>
        /// <returns></returns>
        static long test()
        {
            var list = new List<string>();

            using (StreamReader sr = new StreamReader("names.txt"))
            {
                String line;
                // Read and display lines from the file until the end of 
                // the file is reached.
                while ((line = sr.ReadLine()) != null)
                {
                    foreach (var item in line.Split(','))
                    {
                        list.Add(item.Substring(1, item.Length - 2).ToLowerInvariant());
                    }

                }
            }
            list.Sort();
            long sum = 0;
            long local = 0;
            for (int i = 0; i < list.Count; i++)
            {
                local = 0;
                local = list[i].ToCharArray()
                    .Aggregate(local, (s, c) => s += (Convert.ToByte(c) - ASCIIA));
                sum += (local * (i + 1));
            }
            return sum;
        }
    }
}
