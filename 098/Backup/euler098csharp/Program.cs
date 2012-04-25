using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace euler098csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The largest square number formed by any member of such a pair: ");

            TestBenchSetup();
            TestBenchLoader(plain_group_and_iterating_through_each_anagram_pairs);

            Console.WriteLine("press any key to exit");
            Console.ReadKey();
        }

        static int plain_group_and_iterating_through_each_anagram_pairs()
        {
            // ---- create a dictionary which group words that are anagrams to each other ----
            var word_map = new Dictionary<string, List<string>>();
            foreach (var word in File.ReadAllText("words.txt")
                .Split(new char[] { ',', '"' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var key = new string(word.ToCharArray().OrderBy(c => c).ToArray());
                if (word_map.ContainsKey(key) == false)
                    word_map.Add(key, new List<string>());

                word_map[key].Add(word);
            }
            // ---- create a square number dictionary which firstly group numbers by their digits count ----
            //      then map the number to their square value
            var bound = (int)Math.Sqrt(999999999); // since the longest anagrams group is 9 in length
            var power_map = new Dictionary<int, Dictionary<int, int>>();
            var len = 1;
            var tens = 10;
            var square = 0;
            for (int i = 0; i < bound; i++)
            {
                square = i * i;
                if (square >= tens)
                {
                    tens *= 10;
                    len++;
                }
                if (power_map.ContainsKey(len) == false)
                    power_map.Add(len, new Dictionary<int, int>());

                power_map[len].Add(square, i);
            }
            // ---- for each anagrams group, try to create a working and valid mapping for each word and each ----
            //      square number with same length, and then use the mapping to test rest of the word to see
            //      if such pair exists, and store the pair with highest square number produced
            var result_num = 0;
            foreach (var groups in word_map.Where(e => e.Value.Count > 1)) // only check if group has pairs
            {
                len = groups.Key.Length;

                var powers = power_map[len];
                bound = groups.Value.Count;
                for (int i = 0; i < bound; i++)
                {
                    var word1 = groups.Value[i];
                    foreach (var num1 in powers.Keys) // iterate through all the square numbers that have same length as word
                    {
                        var mapping = create_mapping(word1, num1);
                        if (mapping != null)
                        {
                            for (int j = i + 1; j < bound; j++)
                            {
                                var word2 = groups.Value[j];
                                var num2 = map_word_to_num(mapping, word2);
                                if (powers.ContainsKey(num2) == true)
                                {
                                    if (num2 > result_num)
                                        result_num = Math.Max(num1, num2);
                                }
                            }
                        }
                    }
                }
            }
            return result_num;
        }

        /// <summary>
        /// attempt to create a mapping from word to num.
        /// return null, if unable to create one to one mapping in both direction
        /// </summary>
        static Dictionary<char, int> create_mapping(string word, int num)
        {
            var map_int_char = new Dictionary<int, char>();
            var c_index = word.Length - 1;
            do
            {
                var n = num % 10;
                var c = word[c_index--];
                if (map_int_char.ContainsKey(n) == false)
                    map_int_char.Add(n, c);
                else
                {
                    if (map_int_char[n] != c)
                        return null;
                }
            } while ((num /= 10) > 0);
            var map_char_int = new Dictionary<char, int>();
            foreach (var item in map_int_char) // double check both int->char and char->int are one to one
            {
                if (map_char_int.ContainsKey(item.Value) == true)
                    return null;
                else
                    map_char_int.Add(item.Value, item.Key);
            }
            return map_char_int;
        }

        /// <summary>
        /// using mapping dictionary to tranform word into numeric value
        /// </summary>
        static int map_word_to_num(Dictionary<char, int> mapping, string word)
        {
            var num = 0;
            for (int i = 0; i < word.Length; i++)
            {
                var n = mapping[word[i]];
                num += n;
                num *= 10;
            }
            return num / 10;
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
