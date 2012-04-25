using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using C = System.Console;
using M = System.Math;
using System.IO;


namespace Pb54
{
    public class Pb54
    {
        public static int[] EvalHandValue(int[] p, int c)
        {
            int HighCard = 0,
                OnePair = 1,
                TwoPairs = 2,
                ThreeOfAKind = 3,
                Straight = 4,
                Flush = 5,
                FullHouse = 6,
                FourOfAKind = 7,
                StraightFlush = 8,
                RoyalFlush = 9;
            int[] res;
            bool isFlush = (c == 1);
            bool isStraight = true;
            for (int i = 1; i < 5; i++)
                isStraight &= (p[i] == (p[i - 1] + 1));
            Dictionary<int, int> dic = new Dictionary<int, int>();
            for (int i = 0; i < 5; i++)
            {
                if (!dic.ContainsKey(p[i]))
                    dic.Add(p[i], 1);
                else
                    dic[p[i]]++;
            }
            int four = -1;
            int three = -1;
            int two = -1;
            int twoTwo = -1;
            foreach (KeyValuePair<int, int> kv in dic)
            {
                if (kv.Value == 4)
                    four = kv.Key;
                else if (kv.Value == 3)
                    three = kv.Key;
                else if (kv.Value == 2)
                {
                    if (two < 0)
                        two = kv.Key;
                    else
                        twoTwo = kv.Key;
                }
            }
            if (isFlush)
            {
                res = new int[6];
                if (isStraight)
                {
                    if (p[4] == 12)
                    {
                        // Royal Flush
                        res[0] = RoyalFlush;
                    }
                    // Straight Flush
                    res[0] = StraightFlush;
                }
                if (four < 0 && !(three >= 0 && two >= 0)) // Flush is beaten by Four of a Kind and Full House
                {
                    // Flush
                    res[0] = Flush;
                    for (int i = 0; i < 5; i++)
                        res[i + 1] = p[4 - i];
                    return res;
                }
            }
            if (isStraight)
            {
                res = new int[6];
                // Straight
                res[0] = Straight;
                for (int i = 0; i < 5; i++)
                    res[i + 1] = p[4 - i];
                return res;
            }
            if (four >= 0)
            {
                // Four of a Kind
                res = new int[3];
                res[0] = FourOfAKind;
                res[1] = four;
                res[2] = (p[0] != four ? p[0] : p[4]);
                return res;
            }
            if (three >= 0)
            {
                if (two >= 0)
                {
                    // Full House
                    res = new int[3];
                    res[0] = FullHouse;
                    res[1] = three;
                    res[2] = two;
                    return res;
                }
                // Three of a Kind
                res = new int[4];
                res[0] = ThreeOfAKind;
                res[1] = three;
                int j = 2;
                for (int i = 4; i >= 0; i--)
                    if (p[i] != three)
                        res[j++] = p[i];
                return res;
            }
            if (two >= 0)
            {
                if (twoTwo >= 0)
                {
                    // Two pairs
                    res = new int[4];
                    res[0] = TwoPairs;
                    res[1] = two > twoTwo ? two : twoTwo;
                    res[2] = two > twoTwo ? twoTwo : two;
                    for (int i = 0; i < 5; i++)
                        if (p[i] != two && p[i] != twoTwo)
                            res[3] = p[i];
                    return res;
                }
                // One Pair
                res = new int[5];
                res[0] = OnePair;
                res[1] = two;
                int j = 2;
                for (int i = 4; i >= 0; i--)
                    if (p[i] != two)
                        res[j++] = p[i];
                return res;
            }
            // High Card
            res = new int[6];
            res[0] = HighCard;
            for (int i = 4; i >= 0; i--)
                res[5 - i] = p[i];
            return res;
        }

        public static void Main()
        {
            DateTime start = DateTime.Now;
            int res = 0;
            char[] arValues = { '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A' };
            char[] arColors = { 'C', 'D', 'H', 'S' };
            Dictionary<char, int> dicValues = new Dictionary<char, int>();
            for (int i = 0; i < arValues.Length; i++)
                dicValues.Add(arValues[i], i);
            using (StreamWriter sw = File.CreateText("result.txt"))
            {
                var counter = 1;
                StreamReader sr = File.OpenText("poker.txt");
                while (!sr.EndOfStream)
                {
                    string hands = sr.ReadLine();
                    string[] cards = hands.Split(' ');
                    int[] p1 = new int[5];
                    List<char> p1Colors = new List<char>();
                    int[] p2 = new int[5];
                    List<char> p2Colors = new List<char>();
                    for (int i = 0; i < 5; i++)
                    {
                        p1[i] = dicValues[cards[i][0]];
                        char color = cards[i][1];
                        if (!p1Colors.Contains(color))
                            p1Colors.Add(color);
                        p2[i] = dicValues[cards[i + 5][0]];
                        color = cards[i + 5][1];
                        if (!p2Colors.Contains(color))
                            p2Colors.Add(color);
                    }
                    Array.Sort<int>(p1);
                    Array.Sort<int>(p2);
                    int[] p1HandValue = EvalHandValue(p1, p1Colors.Count);
                    int[] p2HandValue = EvalHandValue(p2, p2Colors.Count);
                    if (p1HandValue[0] > p2HandValue[0])
                    {
                        //C.WriteLine(hands);
                        sw.WriteLine(counter + " " +1);
                        res++;
                    }
                    else if (p1HandValue[0] == p2HandValue[0])
                    {
                        for (int i = 1; i < p1HandValue.Length; i++)
                        {
                            if (p1HandValue[i] > p2HandValue[i])
                            {
                                //C.WriteLine(hands);
                                sw.WriteLine(counter + " " + 1);
                                res++;
                                break;
                            }
                            if (p1HandValue[i] < p2HandValue[i])
                            {
                                sw.WriteLine(counter + " " + 0);
                                break;
                            }
                        }
                    }
                    else
                        sw.WriteLine(counter + " " + 0);

                    counter++;
                }
            }
            C.WriteLine("Result : " + res);
            TimeSpan ts = DateTime.Now - start;
            C.WriteLine("Time taken : " + ts.ToString());
        }
    }
}
