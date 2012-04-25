using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace euler054csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The number of  hands Player 1 won:");

            TestBenchSetup();
            TestBenchLoader(just_parse_and_compare);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// basically parse each string and store in Card struct for easier comparsion
        /// and linq is used everywhere to reduce the pain lawl
        /// </summary>
        static int just_parse_and_compare()
        {
            return File.ReadAllLines("poker.txt")
                .Select(c => who_win(Card.ToCardList(c.Split(new char[] { ' ' }))))
                .Sum();
        }

        [DebuggerDisplay("{Value}{Suit}")]
        public struct Card
        {
            public int Value;
            public char Suit;
            public Card(int v, char s) { Value = v; Suit = s; }
            public static List<Card>[] ToCardList(IEnumerable<string> cards)
            {
                var counter = 0;
                var list = new List<Card>[2] { new List<Card>(), new List<Card>() };
                foreach (var card in cards)
                {
                    if (counter < 5)
                        list[0].Add(Card.ToCard(card));
                    else
                        list[1].Add(Card.ToCard(card));
                    counter++;
                }
                list[0] = list[0].OrderBy(o => o.Value).ToList();
                list[1] = list[1].OrderBy(o => o.Value).ToList();
                return list;
            }
            private static Card ToCard(string c)
            {
                var card = new Card();
                switch (c[0])
                {
                    case '2':
                        card.Value = 2;
                        break;
                    case '3':
                        card.Value = 3;
                        break;
                    case '4':
                        card.Value = 4;
                        break;
                    case '5':
                        card.Value = 5;
                        break;
                    case '6':
                        card.Value = 6;
                        break;
                    case '7':
                        card.Value = 7;
                        break;
                    case '8':
                        card.Value = 8;
                        break;
                    case '9':
                        card.Value = 9;
                        break;
                    case 'T':
                        card.Value = 10;
                        break;
                    case 'J':
                        card.Value = 11;
                        break;
                    case 'Q':
                        card.Value = 12;
                        break;
                    case 'K':
                        card.Value = 13;
                        break;
                    case 'A':
                        card.Value = 14;
                        break;
                }
                card.Suit = c[1];
                return card;
            }
        }

        static int who_win(List<Card>[] hands)
        {
            var v1 = 0;
            var v2 = 0;
            var p1 = true;
            var p2 = true;

            p1 = IsRoyalFlush(hands[0]);
            p2 = IsRoyalFlush(hands[1]);
            if (p1) return 1;
            if (p2) return 0;

            p1 = IsStraightFlush(hands[0]);
            p2 = IsStraightFlush(hands[1]);
            if (p1 == true || p2 == true)
            {
                if (p1 == p2) //screwed if there is A,2,3,4,5
                    return who_has_highest(hands); 
                else if (p1 == true)
                    return 1;
                else
                    return 0;
            }

            p1 = IsFourOfAKind(hands[0]);
            p2 = IsFourOfAKind(hands[1]);
            if (p1 == true || p2 == true)
            {
                if (p1 == p2)
                {
                    v1 = hands[0].GroupBy(c => c.Value).FirstOrDefault(g => g.Count() == 4).First().Value;
                    v2 = hands[1].GroupBy(c => c.Value).FirstOrDefault(g => g.Count() == 4).First().Value;
                    if (v1 == v2)
                        return who_has_highest(hands);
                    else if (v1 > v2)
                        return 1;
                    else
                        return 0;
                }
                else if (p1 == true)
                    return 1;
                else
                    return 0;
            }

            p1 = IsFullHouse(hands[0]);
            p2 = IsFullHouse(hands[1]);
            if (p1 == true || p2 == true)
            {
                if (p1 == p2)
                {
                    v1 = hands[0].GroupBy(c => c.Value).FirstOrDefault(g => g.Count() == 3).First().Value;
                    v2 = hands[1].GroupBy(c => c.Value).FirstOrDefault(g => g.Count() == 3).First().Value;
                    if (v1 == v2)
                        return who_has_highest(hands);
                    else if (v1 > v2)
                        return 1;
                    else
                        return 0;
                }
                else if (p1 == true)
                    return 1;
                else
                    return 0;
            }

            p1 = IsFlush(hands[0]);
            p2 = IsFlush(hands[1]);
            if (p1 == true || p2 == true)
            {
                if (p1 == p2)
                    return who_has_highest(hands);
                else if (p1 == true)
                    return 1;
                else
                    return 0;
            }

            p1 = IsStraight(hands[0]);
            p2 = IsStraight(hands[1]);
            if (p1 == true || p2 == true)
            {
                if (p1 == p2)
                    return who_has_highest(hands);
                else if (p1 == true)
                    return 1;
                else
                    return 0;
            }

            p1 = IsThreeOfAKind(hands[0]);
            p2 = IsThreeOfAKind(hands[1]);
            if (p1 == true || p2 == true)
            {
                if (p1 == p2)
                {
                    v1 = hands[0].GroupBy(c => c.Value).FirstOrDefault(g => g.Count() == 3).First().Value;
                    v2 = hands[1].GroupBy(c => c.Value).FirstOrDefault(g => g.Count() == 3).First().Value;
                    if (v1 == v2)
                        return who_has_highest(hands);
                    else if (v1 > v2)
                        return 1;
                    else
                        return 0;
                }
                else if (p1 == true)
                    return 1;
                else
                    return 0;
            }

            p1 = IsTwoPairs(hands[0]);
            p2 = IsTwoPairs(hands[1]);
            if (p1 == true || p2 == true)
            {
                if (p1 == p2)
                {
                    Console.WriteLine("oops"); // lol, good thing it never comes here
                }
                else if (p1 == true)
                    return 1;
                else
                    return 0;
            }

            p1 = IsOnePair(hands[0]);
            p2 = IsOnePair(hands[1]);
            if (p1 == true || p2 == true)
            {
                if (p1 == p2)
                {
                    v1 = hands[0].GroupBy(c => c.Value).FirstOrDefault(g => g.Count() == 2).First().Value;
                    v2 = hands[1].GroupBy(c => c.Value).FirstOrDefault(g => g.Count() == 2).First().Value;
                    if (v1 == v2)
                        return who_has_highest(hands);
                    else if (v1 > v2)
                        return 1;
                    else
                        return 0;
                }
                else if (p1 == true)
                    return 1;
                else
                    return 0;
            }

            return who_has_highest(hands);
        }

        static bool IsRoyalFlush(List<Card> hand)
        {
            var start = 10;
            var suit = hand[0].Suit;
            return hand.All(c => c.Suit == suit) &&
                hand.All(c => c.Value == start++);
        }

        static bool IsStraightFlush(List<Card> hand)
        {
            var start = hand[0].Value;
            var suit = hand[0].Suit;
            if (start == 2 && hand[4].Value == 14)
            {
                return hand.All(c => c.Suit == suit) && 
                    hand[0].Value == 2 &&
                    hand[1].Value == 3 &&
                    hand[2].Value == 4 &&
                    hand[3].Value == 5 &&
                    hand[4].Value == 14;
            }
            return hand.All(c => c.Suit == suit) && hand.All(c => c.Value == start++);
        }

        static bool IsFourOfAKind(List<Card> hand)
        {
            var kind = hand[1].Value;
            if (kind == hand[0].Value)
                return hand[0].Value == kind &&
                    hand[1].Value == kind &&
                    hand[2].Value == kind &&
                    hand[3].Value == kind;
            else
                return hand[1].Value == kind &&
                    hand[2].Value == kind &&
                    hand[3].Value == kind &&
                    hand[4].Value == kind;
        }

        static bool IsFullHouse(List<Card> hand)
        {
            var kind = hand[1].Value;
            if(kind == hand[2].Value)
                return hand[0].Value == kind &&
                    hand[1].Value == kind &&
                    hand[2].Value == kind &&
                    hand[3].Value == hand[4].Value;
            else
                return hand[0].Value == kind &&
                    hand[1].Value == kind &&
                    hand[2].Value == hand[3].Value &&
                    hand[3].Value == hand[4].Value;
        }

        static bool IsFlush(List<Card> hand)
        {
            var suit = hand[0].Suit;
            return hand.All(c => c.Suit == suit);
        }

        static bool IsStraight(List<Card> hand)
        {
            var start = hand[0].Value;
            if (start == 2 && hand[4].Value == 14)
            {
                return hand[0].Value == 2 &&
                    hand[1].Value == 3 &&
                    hand[2].Value == 4 &&
                    hand[3].Value == 5 &&
                    hand[4].Value == 14;
            }
            return hand.All(c => c.Value == start++);
        }

        static bool IsThreeOfAKind(List<Card> hand)
        {
            return hand.GroupBy(c => c.Value).Any(g => g.Count() == 3);
        }

        static bool IsTwoPairs(List<Card> hand)
        {
            return hand.GroupBy(c => c.Value).Count(g => g.Count() == 2) == 2;
        }
        
        static bool IsOnePair(List<Card> hand)
        {
            return hand.GroupBy(c => c.Value).Count(g => g.Count() == 2) == 1;
        }

        static int who_has_highest(List<Card>[] hands)
        {
            var index = 4;
            while (hands[0][index].Value == hands[1][index].Value)
            {
                index--;
            }
            if (hands[0][index].Value > hands[1][index].Value)
                return 1;
            else
                return 0;
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
