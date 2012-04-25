using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterviewQuestions
{
    class Program
    {
        static void Main(string[] args)
        {
            //test("1231231234");

            //Console.WriteLine(Parser2(new int[] { 1, nil, 2, mul, 3, plus, 1, plus, 2, nil, 3, mul, 1, nil, 2, nil, 3, mul, 4 }));
            //Console.WriteLine(Parser2(new int[] { 3, mul, 4, mul, 5, nil, 6, plus, 2, plus, 3, mul, 7, plus, 4, nil, 9, nil, 0 }));
            test("1231231234", 11353);
            test("3456237490", 1185);
            test("3456237490", 9191);
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        static void test(string input, int answer)
        {
            output = "no answer";
            var ops = new int[3] { 0, 1, 2 }; //0:nothing, 1:plus 2:mul
            var eq = new int[input.Length * 2 - 1];
            var ind = 0;
            for (int i = 0; i < eq.Length; i++)
            {
                if (i % 2 == 0)
                    eq[i] = Convert.ToInt32(input[ind++]) - 48;
            }
            Generate(eq, nil, 1, answer);
            Generate(eq, plus, 1, answer);
            Generate(eq, mul, 1, answer);
            Console.WriteLine(output);
        }

        static string output = "no answer";
        static void Generate(int[] eq, int op, int current, int answer)
        {
            eq[current] = op;
            if (current == eq.Length - 2)
            {
                if (Parser2(eq) == answer)
                    output = eq.Aggregate(string.Empty, (s, n)
                        => s += n == nil ? string.Empty : n == plus ? "+" : n == mul ? "*" : n.ToString());
                return;
            }
            else
            {
                Generate(eq, nil, current + 2, answer);
                Generate(eq, plus, current + 2, answer);
                Generate(eq, mul, current + 2, answer);
            }
        }

        static int nil = -3;
        static readonly int plus = -2;
        static readonly int mul = -1;
        static long Parser2(int[] map)
        {
            var n = new Queue<long>();
            var f = new Stack<int>();
            int len = map.Length;
            int index = 0;
            string tmp = string.Empty;
            while (index < len)
            {
                switch (map[index])
                {
                    case -3:
                        break;
                    case -2:
                        n.Enqueue(Convert.ToInt64(tmp));
                        tmp = string.Empty;
                        while (f.Count > 0 && map[index] <= f.Peek())
                        {
                            n.Enqueue(f.Pop());
                        }
                        f.Push(map[index]);
                        break;
                    case -1:
                        n.Enqueue(Convert.ToInt64(tmp));
                        tmp = string.Empty;
                        f.Push(map[index]);
                        break;
                    default:
                        tmp += map[index];
                        break;
                }
                index++;
            }
            if (string.IsNullOrEmpty(tmp) == false)
                n.Enqueue(Convert.ToInt64(tmp));
            while (f.Count > 0)
                n.Enqueue(f.Pop());
            var p = new Stack<long>();
            long cur = 0;
            while (n.Count > 0)
            {
                cur = n.Dequeue();
                if (cur != plus && cur != mul)
                    p.Push(cur);
                else if (cur == plus)
                    p.Push(p.Pop() + p.Pop());
                else if (cur == mul)
                    p.Push(p.Pop() * p.Pop());

            }
            return p.Pop();
        }



        static long Parser(int[] map)
        {
            // stacks for shunting yard algorithm
            var n = new Queue<long>();
            var ops = new Stack<long>();

            int len = map.Length;
            int index = 0;
            string tmp = string.Empty;
            //parse
            while (index < len)
            {
                if (map[index] == nil)
                {
                    index++;
                    continue;
                }

                if (map[index] != mul && map[index] != plus)
                    tmp += map[index];
                else
                {
                    // so the number streak stopped, add the number to n stack
                    n.Enqueue(Convert.ToInt64(tmp));
                    tmp = string.Empty;
                    if (ops.Count == 0)
                        ops.Push(map[index]);
                    else
                    {
                        if (map[index] == plus)
                        {
                            if (ops.Peek() == mul)
                            {
                                ops.Pop();
                                var n2 = n.Dequeue();
                                var n1 = n.Dequeue();
                                n.Enqueue(n1 * n2);
                            }

                            if (ops.Count > 0 && ops.Peek() == plus)
                            {
                                ops.Pop();
                                var n2 = n.Dequeue();
                                var n1 = n.Dequeue();
                                n.Enqueue(n1 + n2);
                                ops.Push(map[index]);
                            }

                            if (ops.Count == 0)
                                ops.Push(map[index]);
                        }

                        if (map[index] == mul)
                            ops.Push(map[index]);
                    }
                }
                index++;
            }

            if (string.IsNullOrEmpty(tmp) == false)
                n.Enqueue(Convert.ToInt64(tmp));

            while (ops.Count > 0)
            {
                if (ops.Pop() == mul)
                {
                    var n2 = n.Dequeue();
                    var n1 = n.Dequeue();
                    n.Enqueue(n1 * n2);
                }
                else
                {
                    var n2 = n.Dequeue();
                    var n1 = n.Dequeue();
                    n.Enqueue(n1 + n2);
                }
            }

            return n.Dequeue();
        }
    }
}
