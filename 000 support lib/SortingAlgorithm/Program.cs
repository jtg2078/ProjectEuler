using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SortingAlgorithm
{
    class Program
    {
        static void Main(string[] args)
        {
            TestBenchLoader(bubble_sort);
            TestBenchLoader(selection_sort);
            TestBenchLoader(insertion_sort);
            TestBenchLoader(merge_sort);
            TestBenchLoader(heap_sort);

            BinaryTreeSort();

            heap_sort(new int[] { 11, 3, 4, 8, 5 });

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        static void TestBenchLoader(Action<int[]> test_method)
        {
            //var test = new int[] { 3, 7, 2, 1, 4, 1, 2, 5, 11, 90, 102, 77 };
            var test = new int[] { 38, 27, 43, 3, 9, 82, 10 };
            var expected = test.OrderBy(l => l).ToArray();

            test_method(test);

            var correct = expected.SequenceEqual(test);
            Console.WriteLine(string.Format("Is {0} implementation produced correct result: {1}", test_method.Method.Name, correct));
        }

        static void bubble_sort(int[] list)
        {
            var len = list.Length;
            var temp = 0;
            var swapped = false;
            for (int i = 0; i < len; i++)
            {
                swapped = false;
                for (int j = len - 1; j > i; j--)
                {
                    if (list[j] < list[j - 1])
                    {
                        temp = list[j];
                        list[j] = list[j - 1];
                        list[j - 1] = temp;
                        swapped = true;
                    }
                }
                if (swapped == false)
                    break;
            }
        }

        static void selection_sort(int[] list)
        {
            var len = list.Length;
            var temp = 0;
            var index = 0;
            for (int i = 0; i < len; i++)
            {
                index = i;
                for (int j = i + 1; j < len; j++)
                {
                    if (list[j] < list[index])
                        index = j;
                }
                if (index != i)
                {
                    temp = list[i];
                    list[i] = list[index];
                    list[index] = temp;
                }
            }
        }

        static void insertion_sort(int[] list)
        {
            var len = list.Length;
            var value = 0;
            var j = 0;
            for (int i = 0; i < len; i++)
            {
                value = list[i];
                j = i;
                while (j > 0 && list[j - 1] > value)
                {
                    list[j] = list[j - 1];
                    j--;
                }
                list[j] = value;
            }
        }

        [DebuggerDisplay("{value}")]
        class Node
        {
            public int value;
            public Node parent;
            public Node left;
            public Node right;
        }

        static void InsertNode(ref Node treeNode, Node newNode)
        {
            if (treeNode == null)
                treeNode = newNode;
            else if (newNode.value < treeNode.value)
                InsertNode(ref treeNode.left, newNode);
            else
                InsertNode(ref treeNode.right, newNode);
        }

        static bool SearchTree(Node node, int value)
        {
            while (node != null)
            {
                if (value == node.value)
                    return true;
                else if (value < node.value)
                    node = node.left;
                else if (value > node.value)
                    node = node.right;
            }
            return false;
        }

        static Node CreateTree()
        {
            var node = new Node() { value = 8 };
            InsertNode(ref node, new Node() { value = 3 });
            InsertNode(ref node, new Node() { value = 10 });
            InsertNode(ref node, new Node() { value = 1 });
            InsertNode(ref node, new Node() { value = 6 });
            InsertNode(ref node, new Node() { value = 14 });
            InsertNode(ref node, new Node() { value = 4 });
            InsertNode(ref node, new Node() { value = 7 });
            InsertNode(ref node, new Node() { value = 13 });
            return node;
        }

        static void BinaryTreeSort()
        {
            var node = CreateTree();

            Action<Node> traverse = null;
            var result = new List<int>();
            traverse = n =>
            {
                if (n == null)
                    return;
                traverse(n.left);
                result.Add(n.value);
                traverse(n.right);
            };
            traverse(node);
        }

        static void merge_sort(int[] list)
        {
            Func<List<int>, List<int>, List<int>> merge = null;
            Func<List<int>, List<int>> merge_sort = null;
            merge_sort = (m) =>
            {
                if (m.Count <= 1)
                    return m;

                var left = new List<int>();
                var right = new List<int>();
                var result = new List<int>();
                var mid = m.Count / 2;
                for (int i = 0; i < mid; i++)
                {
                    left.Add(m[i]);
                }
                for (int i = mid; i < m.Count; i++)
                {
                    right.Add(m[i]);
                }
                left = merge_sort(left);
                right = merge_sort(right);
                if (left.Last() > right.First())
                    result = merge(left, right);
                else
                    result = left.Concat(right).ToList();
                return result;
            };

            merge = (left, right) =>
            {
                var result = new List<int>();
                while (left.Count > 0 && right.Count > 0)
                {
                    if (left[0] <= right[0])
                    {
                        result.Add(left[0]);
                        left.RemoveAt(0);
                    }
                    else
                    {
                        result.Add(right[0]);
                        right.RemoveAt(0);
                    }
                }
                while (left.Count > 0)
                {
                    result.Add(left[0]);
                    left.RemoveAt(0);
                }
                while (right.Count > 0)
                {
                    result.Add(right[0]);
                    right.RemoveAt(0);
                }
                return result;
            };

            if (list.Length <= 1)
                return;

            var r = merge_sort(list.ToList());
            for (int i = 0; i < list.Length; i++)
            {
                list[i] = r[i];
            }
        }

        static void heap_sort(int[] list)
        {
            var head = new Node() { value = list[0] };
            var cur = head;
            Action<int> insert_node = null;
            insert_node = num =>
            {
                // traverse to right most bottom of the tree
                while (cur.right != null)
                {
                    cur = cur.right;
                }
                // add the node
                if (cur.left == null)
                {
                    cur.left = new Node() { value = num };
                    cur.left.parent = cur;
                }
                else
                {
                    cur.right = new Node() { value = num };
                    cur.right.parent = cur;
                }

                // rebalancing the tree
                cur = cur.left;
                while (cur.parent != null && cur.value > cur.parent.value)
                {
                    var tmp = cur.parent.value;
                    cur.parent.value = cur.value;
                    cur.value = tmp;
                }
            };

            for (int i = 1; i < list.Length; i++)
            {
                insert_node(list[i]);
            }
        }
    }
}
