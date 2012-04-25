using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Trie
{
    class Program
    {
        static void Main(string[] args)
        {
            Trie<string> trie = new Trie<string>();
            trie.Add("1318", "Jppphn");
            trie.Add("1319", "J3hn");
            trie.Add("1325", "Jshn");
            trie.Add("13258", "hola");
            trie.Add("1330", "J1hn");

            Console.WriteLine("testing containkeys");
            Console.WriteLine("Find key 1319, should return true. Result: " + trie.ContainsKey("1319"));
            Console.WriteLine("Find key 1219, should return false. Result: " + trie.ContainsKey("1219"));

            Console.WriteLine("testing get all values");
            trie.GetKeys().ForEach(l => Console.WriteLine(l));

            Console.WriteLine("testing traverse all keys");
            trie.Keys.ToList().ForEach(l => Console.WriteLine(l));

            Console.WriteLine("testing remove key:1325");
            trie.Remove("1325");
            Console.WriteLine("out out all key after the removal");
            trie.Keys.ToList().ForEach(l => Console.WriteLine(l));

            Console.ReadKey();

        }
    }

    public class Trie<V> : IDictionary<string, V>
    {
        private class TrieNode
        {
            public V value;
            public Dictionary<char, TrieNode> children;
            public bool hasValue;
            public char key;
            public TrieNode()
            {
                children = new Dictionary<char, Trie<V>.TrieNode>();
                hasValue = false;
            }
            public System.Collections.IEnumerator GetDepthNodes()
            {
                yield return this;

                foreach (var child in children.Values)
                {
                    IEnumerator childEnumerator = child.GetDepthNodes();
                    while (childEnumerator.MoveNext())
                        yield return childEnumerator.Current;
                }
            }
        }

        private TrieNode root;
        private List<KeyValuePair<string, V>> master_list;
        public Trie()
        {
            root = new Trie<V>.TrieNode();
            master_list = new List<KeyValuePair<string, V>>();
        }

        private TrieNode Insert(string key, V value, TrieNode root)
        {
            var current = root;
            var prefix = key[0];
            var index = 1;

            while (index <= key.Length)
            {
                prefix = key[index - 1];
                if (current.children.ContainsKey(prefix))
                {
                    current = current.children[prefix];
                }
                else //new child
                {
                    var child = new TrieNode();
                    child.key = prefix;
                    current.children.Add(prefix, child);
                    current = child;
                }
                index++;
            }
            master_list.Add(new KeyValuePair<string, V>(key, value));
            current.value = value;
            current.hasValue = true;
            return root;
        }

        private bool Find(TrieNode node, string key)
        {
            if (string.IsNullOrEmpty(key) && node.hasValue)
                return true;

            var prefix = key[0];
            var tail = key.Substring(1);
            if (node.children.ContainsKey(prefix))
                return Find(node.children[prefix], tail);
            else
                return false;
        }

        public List<string> GetKeys()
        {
            var list = new List<string>();
            var iterator = root.GetDepthNodes();
            var key = string.Empty;
            while (iterator.MoveNext())
            {
                var cur = iterator.Current as TrieNode;
                list.Add(cur.key + string.Empty);
                //if (cur.hasValue)
                //    list.Add(cur.value);
            }
            return list;
        }

        private List<string> keys = new List<string>();
        private void traverse(TrieNode node, string prefix)
        {
            if (node.hasValue)
                keys.Add(prefix + node.key);

            foreach (var item in node.children)
            {
                if (node.key == '\0')
                    traverse(item.Value, prefix);
                else
                    traverse(item.Value, prefix + node.key);
            }
        }

        private TrieNode remove(string key, TrieNode root)
        {
            var current = root;
            var prefix = key[0];
            var index = 1;
            TrieNode parent = null;

            while (index <= key.Length)
            {
                prefix = key[index - 1];
                if (current.children.ContainsKey(prefix))
                {
                    parent = current;
                    current = current.children[prefix];
                }
                index++;
            }

            if (current.children.Count == 0)
                parent.children.Remove(key[key.Length - 1]);
            else
            {
                current.hasValue = false;
                current.value = default(V);
            }

            return root;
        }

        #region IDictionary<string,V> Members

        public void Add(string key, V value)
        {
            if (string.IsNullOrEmpty(key) || value == null)
                return;
            root = Insert(key, value, root);
        }

        public bool ContainsKey(string key)
        {
            return Find(root, key);
        }

        public ICollection<string> Keys
        {
            get
            {
                keys.Clear();
                traverse(root, string.Empty);
                return keys;
            }
        }

        public bool Remove(string key)
        {
            if (string.IsNullOrEmpty(key))
                return false;

            root = remove(key, root);
            return true;
        }

        public bool TryGetValue(string key, out V value)
        {
            throw new NotImplementedException();
        }

        public ICollection<V> Values
        {
            get { throw new NotImplementedException(); }
        }

        public V this[string key]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region ICollection<KeyValuePair<string,V>> Members

        public void Add(KeyValuePair<string, V> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<string, V> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<string, V>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public bool Remove(KeyValuePair<string, V> item)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable<KeyValuePair<string,V>> Members

        public IEnumerator<KeyValuePair<string, V>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
