using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace euler022csharp
{
    class BurstSortx
    {
        #region nothing
        static void quickSortLoader()
        {
            var list = new List<int> { 3, 7, 8, 5, 2, 1, 9, 5, 4 };
            Console.WriteLine(list.Aggregate(string.Empty, (sum, n) => sum += (" " + n)));
            //var index = partition<int>(list, 0, 8, 3);
            //quicksort<int>(list, 0, list.Count - 1);
            var list2 = new List<string> { "aa", "ab", "bab", "abba", "aa", "azz" };
            multikey_quicksort(list2, 0, list2.Count - 1, list2.Count / 2 - 1);
            Console.WriteLine(list2.Aggregate(string.Empty, (sum, n) => sum += (" " + n)));

        }

        static void quicksort<T>(List<T> list, int left, int right) where T : IComparable
        {
            if (right > left)
            {
                var pivot = partition<T>(list, left, right, (left + right) / 2);
                quicksort<T>(list, left, pivot - 1);
                quicksort<T>(list, pivot + 1, right);
            }
        }

        static int partition<T>(List<T> list, int left, int right, int index) where T : IComparable
        {
            var pivot = list[index];
            T tmp;
            int storeIndex = left;
            //swap, move pivot to end
            tmp = list[right];
            list[right] = list[index];
            list[index] = tmp;

            for (int i = left; i <= right - 1; i++)
            {
                if (list[i].CompareTo(pivot) <= 0)
                {
                    tmp = list[storeIndex];
                    list[storeIndex] = list[i];
                    list[i] = tmp;
                    storeIndex++;
                }
            }

            tmp = list[right];
            list[right] = list[storeIndex];
            list[storeIndex] = tmp;
            return storeIndex;
        }

        static bool less(string a, string b, int d)
        {
            if (b.Length <= d)
                return false;
            if (a.Length <= d)
                return true;
            return a[d] < b[d];
        }

        static bool equal(string a, string b, int d)
        {
            return !less(a, b, d) && !less(b, a, d);
        }

        static void exch(List<string> list, int index1, int index2)
        {
            var tmp = list[index1];
            list[index1] = list[index2];
            list[index2] = tmp;
        }

        static void multikey_quicksort(List<string> list, int left, int right, int pivot)
        {
            if (left >= right)
                return;
            var tmp = list[right];
            var i = left - 1;
            var j = right;
            var p = left - 1;
            var q = right;
            var k = 0;
            while (i < j)
            {
                while (less(list[++i], tmp, pivot)) ;
                while (less(tmp, list[--j], pivot)) ;
                if (j == left)
                    break;
                if (i > j)
                    break;
                exch(list, i, j);
                if (equal(list[i], tmp, pivot))
                    exch(list, ++p, i);
                if (equal(tmp, list[j], pivot))
                    exch(list, --q, j);
            }
            if (p == q)  // first d+1 chars of all keys equal
            {
                if (tmp.Length > pivot)
                    multikey_quicksort(list, left, right, pivot + 1);
            }
            if (p == q)
                return;
            if (less(list[i], tmp, pivot))
                i++;
            for (k = left; k <= p; k++, j--)
            {
                exch(list, k, j);
            }
            for (k = right; k >= q; k--, i++)
            {
                exch(list, k, i);
            }
            multikey_quicksort(list, left, j, pivot);
            if ((i == right) && (equal(list[i], tmp, pivot)))
                i++;
            if (tmp.Length >= pivot)
                multikey_quicksort(list, j + 1, i - 1, pivot + 1);
            multikey_quicksort(list, i, right, pivot);
        }
        #endregion



        static readonly int BUCKETSIZE = 10;
        static readonly int ASCIIA = 97;
        class TrieNode
        {
            public TrieNode[] nodes;
            public Dictionary<char, List<string>> values;
            public bool isEnd = false;
            public TrieNode()
            {
                nodes = new TrieNode[26];
                values = new Dictionary<char, List<string>>();
            }

            public bool AddValue(string s)
            {
                var key = s[0];
                if (values.ContainsKey(key))
                {
                    values[key].Add(s.Substring(1));
                    if (values[key].Count >= BUCKETSIZE)
                        return false;
                }
                else
                    values.Add(key, new List<string> { s.Substring(1) });

                return true;
            }

            public bool Contains(char c)
            {
                int n = Convert.ToByte(c) - ASCIIA;
                if (n < 26)
                    return (nodes[n] != null);
                else
                    return false;
            }

            public TrieNode GetChild(char c)
            {
                int n = Convert.ToByte(c) - ASCIIA;
                return nodes[n];
            }
        }

        class BurstSort
        {
            private TrieNode root = new TrieNode();
            //public void start()
            //{
            //    var list = new List<string> { "aa", "ab", "bab", "abba", "aa", "azz" };
            //    TrieNode node = root;
            //    foreach (var item in list)
            //    {
            //        node = Insert(item, node);
            //    }

            //    //traversal part
            //    Traverse(root,string.Empty);
            //    root.isEnd = true;

            //}

            public void start2()
            {
                TrieNode node = root;
                //try
                //{
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                using (StreamReader sr = new StreamReader("names.txt"))
                {
                    String line;
                    // Read and display lines from the file until the end of 
                    // the file is reached.
                    while ((line = sr.ReadLine()) != null)
                    {
                        foreach (var item in line.Split(','))
                        {
                            sorted2.Add(item.Substring(1, item.Length - 2).ToLowerInvariant());
                        }

                        line.Split(',')
                            .ToList()
                            .ForEach(l => node = Insert(l));
                    }
                }
                //sorted2.Sort();
                //    TextWriter stringWriter = new StringWriter();
                //    using (TextWriter streamWriter =
                //        new StreamWriter("InvalidPathChars.txt"))
                //    {
                //        sorted2.ForEach(l=>streamWriter.WriteLine(l));
                //        //streamWriter.WriteLine(
                //    }

                //}
                //catch (Exception e)
                //{
                //    // Let the user know what went wrong.
                //    Console.WriteLine("The file could not be read:");
                //    Console.WriteLine(e.Message);
                //}

                //traversal part
                Traverse(root, string.Empty);
                root.isEnd = true;
            }

            private List<string> sorted = new List<string>();
            private List<string> sorted2 = new List<string>();
            private string prefix2 = string.Empty;
            private int burst_count = 0;
            private void Traverse(TrieNode node, string prefix)
            {
                var len = node.nodes.Length;
                for (int i = 0; i < len; i++)
                {
                    var c = Convert.ToChar(ASCIIA + i);

                    if (node.nodes[i] != null)
                    {
                        Traverse(node.nodes[i], prefix + c);
                    }
                    else
                    {

                        if (node.values.ContainsKey(c))
                        {
                            node.values[c].Sort();
                            node.values[c].ForEach(l => sorted.Add(prefix + c + l));
                            //node.values[c].ForEach(l => sorted.Add(prefix2 +c + l));

                        }
                    }
                }
                prefix2 = string.Empty;
            }

            public TrieNode Insert(string s)
            {
                TrieNode node = root;
                s = s.Substring(1, s.Length - 2).ToLowerInvariant();
                Insert(s, node);
                return root;
            }

            private void Insert(string s, TrieNode node)
            {
                if (string.IsNullOrEmpty(s))
                    return;
                var c = s[0];

                if (node.Contains(c))
                {
                    // this means that the bucket for this char is busted, so it contain the next trie node;
                    Insert(s, node.GetChild(c));
                    //return node.GetChild(c);
                }
                else
                {
                    var within_limit = node.AddValue(s);
                    if (within_limit == false)
                    {
                        // beyound the bucket limit, so we are going to burst phase
                        int n = Convert.ToByte(c) - ASCIIA;
                        TrieNode t = new TrieNode();
                        node.nodes[n] = t;
                        burst_count++;
                        foreach (var value in node.values[c])
                        {
                            if (value.Length > 1)
                                Insert(value.Substring(1), t);
                            // else

                        }
                        //node.values.Remove(c);
                        //return node;
                    }
                }
            }
        }

        class Tries
        {
            private TrieNode root = new TrieNode();

            public void start()
            {

            }

            public TrieNode Insert(string s)
            {
                char[] charArray = s.ToLower().ToCharArray();
                TrieNode node = root;
                foreach (char c in charArray)
                {
                    node = Insert(c, node);
                }
                node.isEnd = true;
                return root;
            }

            private TrieNode Insert(char c, TrieNode node)
            {
                if (node.Contains(c))
                    return node.GetChild(c);
                else
                {
                    int n = Convert.ToByte(c) - ASCIIA;
                    TrieNode t = new TrieNode();
                    node.nodes[n] = t;
                    return t;
                }
            }

            public bool Contains(string s)
            {
                char[] charArray = s.ToLower().ToCharArray();
                TrieNode node = root;
                bool contains = true;
                foreach (char c in charArray)
                {
                    node = Contains(c, node);

                    if (node == null)
                    {
                        contains = false;
                        break;
                    }
                }
                if ((node == null) || (!node.isEnd))
                    contains = false;
                return contains;
            }

            private TrieNode Contains(char c, TrieNode node)
            {
                if (node.Contains(c))
                    return node.GetChild(c);
                else
                    return null;
            }
        }
    }
}
