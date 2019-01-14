using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Collections.Tasks
{

    /// <summary>
    ///  Tree node item 
    /// </summary>
    /// <typeparam name="T">the type of tree node data</typeparam>
    public interface ITreeNode<T>
    {
        T Data { get; set; }                             // Custom data
        IEnumerable<ITreeNode<T>> Children { get; set; } // List of childrens
    }


    public class Task
    {

        /// <summary> Generate the Fibonacci sequence f(x) = f(x-1)+f(x-2) </summary>
        /// <param name="count">the size of a required sequence</param>
        /// <returns>
        ///   Returns the Fibonacci sequence of required count
        /// </returns>
        /// <exception cref="System.InvalidArgumentException">count is less then 0</exception>
        /// <example>
        ///   0 => { }  
        ///   1 => { 1 }    
        ///   2 => { 1, 1 }
        ///   12 => { 1, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144 }
        /// </example>
        public static IEnumerable<int> GetFibonacciSequence(int count)
        {
            if (count < 0)
                throw new ArgumentException();

            if (count == 0)
                yield break;

            int result = 0, left = 0, right = 1;

            yield return result + 1;

            for (var i = 1; i < count; i++)
            {
                result = left + right;
                yield return result;
                left = right;
                right = result;
            }
        }

        /// <summary>
        ///    Parses the input string sequence into words
        /// </summary>
        /// <param name="reader">input string sequence</param>
        /// <returns>
        ///   The enumerable of all words from input string sequence. 
        /// </returns>
        /// <exception cref="System.ArgumentNullException">reader is null</exception>
        /// <example>
        ///  "TextReader is the abstract base class of StreamReader and StringReader, which ..." => 
        ///   {"TextReader","is","the","abstract","base","class","of","StreamReader","and","StringReader","which",...}
        /// </example>
        public static IEnumerable<string> Tokenize(TextReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException();
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                foreach (var a in line.Split(new[] { ',', ' ', '.', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    yield return a;
                }
            }
        }



        /// <summary>
        ///   Traverses a tree using the depth-first strategy
        /// </summary>
        /// <typeparam name="T">tree node type</typeparam>
        /// <param name="root">the tree root</param>
        /// <returns>
        ///   Returns the sequence of all tree node data in depth-first order
        /// </returns>
        /// <example>
        ///    source tree (root = 1):
        ///    
        ///                      1
        ///                    / | \
        ///                   2  6  7
        ///                  / \     \
        ///                 3   4     8
        ///                     |
        ///                     5   
        ///                   
        ///    result = { 1, 2, 3, 4, 5, 6, 7, 8 } 
        /// </example>
        public static IEnumerable<T> DepthTraversalTree<T>(ITreeNode<T> root)
        {
            if (root == null)
                throw new ArgumentNullException();

            Stack<ITreeNode<T>> treeNodes = new Stack<ITreeNode<T>>();
            treeNodes.Push(root);

            while (treeNodes.Count > 0)
            {
                ITreeNode<T> treeNode = treeNodes.Pop();
                yield return treeNode.Data;
                if (treeNode.Children != null)
                {
                    for (int i = treeNode.Children.Count() - 1; i >= 0; i--)
                    {
                        treeNodes.Push(treeNode.Children.ElementAt(i));
                    }
                }
            }
        }

        /// <summary>
        ///   Traverses a tree using the width-first strategy
        /// </summary>
        /// <typeparam name="T">tree node type</typeparam>
        /// <param name="root">the tree root</param>
        /// <returns>
        ///   Returns the sequence of all tree node data in width-first order
        /// </returns>
        /// <example>
        ///    source tree (root = 1):
        ///    
        ///                      1
        ///                    / | \
        ///                   2  3  4
        ///                  / \     \
        ///                 5   6     7
        ///                     |
        ///                     8   
        ///                   
        ///    result = { 1, 2, 3, 4, 5, 6, 7, 8 } 
        /// </example>
        public static IEnumerable<T> WidthTraversalTree<T>(ITreeNode<T> root)
        {
            if (root == null)
                throw new ArgumentNullException();

            Queue<ITreeNode<T>> treeNodes = new Queue<ITreeNode<T>>();
            treeNodes.Enqueue(root);

            while (treeNodes.Count > 0)
            {
                ITreeNode<T> treeNode = treeNodes.Dequeue();
                yield return treeNode.Data;
                if (treeNode.Children != null)
                {
                    foreach (var children in treeNode.Children)
                    {
                        treeNodes.Enqueue(children);
                    }
                }
            }
        }



        /// <summary>
        ///   Generates all permutations of specified length from source array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">source array</param>
        /// <param name="count">permutation length</param>
        /// <returns>
        ///    All permuations of specified length
        /// </returns>
        /// <exception cref="System.InvalidArgumentException">count is less then 0 or greater then the source length</exception>
        /// <example>
        ///   source = { 1,2,3,4 }, count=1 => {{1},{2},{3},{4}}
        ///   source = { 1,2,3,4 }, count=2 => {{1,2},{1,3},{1,4},{2,3},{2,4},{3,4}}
        ///   source = { 1,2,3,4 }, count=3 => {{1,2,3},{1,2,4},{1,3,4},{2,3,4}}
        ///   source = { 1,2,3,4 }, count=4 => {{1,2,3,4}}
        ///   source = { 1,2,3,4 }, count=5 => ArgumentOutOfRangeException
        /// </example>
        public static IEnumerable<T[]> GenerateAllPermutations<T>(T[] source, int count)
        {
            if (count < 0 || count > source.Length)
                throw new ArgumentOutOfRangeException();

            if (count == 0)
                yield break;

            var indexesArray = new int[count];

            for (int i = 0; i < count; i++)
            {
                indexesArray[i] = i;
            }

            var position = count;

            while (position >= 0)
            {
                var t = new T[count];

                for (int i = 0; i < indexesArray.Length; i++)
                {
                    t[i] = source.ElementAt(indexesArray[i]);
                }

                yield return t;

                if (count == source.Length)
                    yield break;

                position = (indexesArray[count - 1] == source.Length - 1) ? position - 1 : count - 1;

                if (position >= 0)
                {
                    for (int i = count - 1; i >= position; i--)
                    {
                        indexesArray[i] = indexesArray[position] + i - position + 1;
                    }
                }
            }
        }

    }

    public static class DictionaryExtentions
    {

        /// <summary>
        ///    Gets a value from the dictionary cache or build new value
        /// </summary>
        /// <typeparam name="TKey">TKey</typeparam>
        /// <typeparam name="TValue">TValue</typeparam>
        /// <param name="dictionary">source dictionary</param>
        /// <param name="key">key</param>
        /// <param name="builder">builder function to build new value if key does not exist</param>
        /// <returns>
        ///   Returns a value assosiated with the specified key from the dictionary cache. 
        ///   If key does not exist than builds a new value using specifyed builder, puts the result into the cache 
        ///   and returns the result.
        /// </returns>
        /// <example>
        ///   IDictionary<int, Person> cache = new SortedDictionary<int, Person>();
        ///   Person value = cache.GetOrBuildValue(10, ()=>LoadPersonById(10) );  // should return a loaded Person and put it into the cache
        ///   Person cached = cache.GetOrBuildValue(10, ()=>LoadPersonById(10) );  // should get a Person from the cache
        /// </example>
        public static TValue GetOrBuildValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> builder)
        {
            if (dictionary.TryGetValue(key, out TValue value))
            {
                return value;
            }
            value = builder();
            dictionary.Add(key, value);
            return value;
        }

    }
}
