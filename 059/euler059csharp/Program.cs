using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace euler059csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The sum of the ASCII values in the original text: ");

            TestBenchSetup();
            TestBenchLoader(using_heuristic_method_to_find_best_matching_key);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// basically by finding out all the keys that produce the most alphabetical characters,
        /// since the message is likely to be paragraphs of english text. And then base on the commonly
        /// occured english word, narrow down the possible keys to one with highest "score", and decrypt
        /// the message with that key, and sums up all the ascii value of each character
        /// </summary>
        /// <returns></returns>
        static int using_heuristic_method_to_find_best_matching_key()
        {
            var keys = Enumerable.Range(97, 26).ToArray();
            var text = File.ReadAllText("cipher1.txt")
                .Split(new char[] { ',' })
                .Select(c => int.Parse(c)).ToArray();
            var possible_keys = new int[3][];
            possible_keys[0] = find_key_candidate(text, keys, 0);
            possible_keys[1] = find_key_candidate(text, keys, 1);
            possible_keys[2] = find_key_candidate(text, keys, 2);
            var max = 0;
            var cur = 0;
            var best = new int[3];
            int[] decrypted_text = null;
            int[] tmp;
            foreach (var k1 in possible_keys[0])
            {
                foreach (var k2 in possible_keys[1])
                {
                    foreach (var k3 in possible_keys[2])
                    {
                        tmp = decrypt_with_key(text, new int[] { k1, k2, k3 });
                        cur = calculate_key_score_using_heuristic(tmp);
                        if (cur > max)
                        {
                            decrypted_text = tmp;
                            max = cur;
                        }
                    }
                }
            }
            return decrypted_text.Sum();
        }

        /// <summary>
        /// so this method return all the possible key candidate based on
        /// how many whites space or alphabetical characters the decrypted text returns
        /// that are above the value of (average + 2x STD)
        /// </summary>
        /// <param name="text">the encrypted text</param>
        /// <param name="keys">array of all possible keys(a-z)</param>
        /// <param name="key_selector">indicates which key</param>
        /// <returns>array contains all the key that has value above cut off point</returns>
        static int[] find_key_candidate(int[] text, int[] keys, int key_selector)
        {
            int i, j, d, k;
            var result = new int[26];
            for (i = 0; i < keys.Length; i++)
            {
                k = keys[i];
                for (j = key_selector; j < text.Length; j += 3)
                {
                    d = text[j] ^ k;
                    if (is_ascii_char_or_space(d) == true)
                        result[i]++;
                }
            }
            var std = standard_deviation(result.ToList());
            var cut_off = result.Average() + 2 * std;
            return result.Select((num, index) => new { ch = index + 97, count = num })
               .Where(l => l.count > cut_off)
               .Select(l => l.ch)
               .ToArray();
        }

        /// <summary>
        /// using the most commonly used english word(http://www.world-english.org/english500.htm)
        /// top 4: (T)the, (O)of, (T)to, and, (A)and
        /// each word gives a score of 1;
        /// </summary>
        /// <param name="text"> already decrypted text using the key to be tested</param>
        /// <returns>score base on commonly used words</returns>
        static int calculate_key_score_using_heuristic(int[] text)
        {
            var score = 0;
            int i, d1, d2, d3, d4, d5;
            for (i = 0; i < text.Length - 4; i++)
            {
                d1 = text[i];
                d2 = text[i + 1];
                d3 = text[i + 2];
                d4 = text[i + 3];
                d5 = text[i + 4];

                if (d1 == ' ' && (d2 == 'T' || d2 == 't'))
                {
                    if (d3 == 'h' && d4 == 'e' && d5 == ' ')
                        score++;
                    else if (d3 == 'o' && d4 == ' ')
                        score++;
                }
                else if (d1 == ' ' && (d2 == 'O' || d2 == 'o'))
                {
                    if (d3 == 'f' && d4 == ' ')
                        score++;
                }
                else if (d1 == ' ' && (d2 == 'A' || d2 == 'a'))
                {
                    if (d3 == 'n' && d4 == 'd' && d5 == ' ')
                        score++;
                }
            }
            return score;
        }

        static int[] decrypt_with_key(int[] text, int[] key)
        {
            var index = 0;
            return text.Select(c => c ^ key[(index++ % 3)]).ToArray();
        }

        static bool is_ascii_char_or_space(int code)
        {
            return code == 32 || (code >= 64 && code <= 90) || (code >= 97 && code <= 122);
        }

        /// <summary>
        /// http://stackoverflow.com/questions/895929/how-do-i-determine-the-standard-deviation-stddev-of-a-set-of-values
        /// using Welford's method
        /// </summary>
        static double standard_deviation(List<int> nums)
        {
            double M = 0.0;
            double S = 0.0;
            double tmpM = 0.0;
            int k = 1;
            foreach (double value in nums)
            {
                tmpM = M;
                M += (value - tmpM) / k;
                S += (value - tmpM) * (value - M);
                k++;
            }
            return Math.Sqrt(S / (k - 1));
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
