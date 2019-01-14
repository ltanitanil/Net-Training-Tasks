using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Collections.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Collections.Test {

    [TestClass]
    public class Tests {

        #region Performance tests 
        public Tuple<string, TimeSpan, TimeSpan> EvaluateDictionaryPerformance(IDictionary<string, int> dict) {
            int MaxItemsCount = 10000;
            // Add item performance
            TimeSpan addTime = Utils.EvaluatePerformance(() => {
                for (int i = 0; i < MaxItemsCount; i++) dict.Add(i.ToString(), i);
            });
            // Get item performance
            int n;
            TimeSpan searchTime = Utils.EvaluatePerformance(() => {
                for (int i = 0; i < MaxItemsCount; i++) n = dict[i.ToString()];
            });
            return Tuple.Create(dict.GetType().ToString(), addTime, searchTime);
        }


        [TestMethod]
        [TestCategory("Performance tests for dictionaries")]
        public void TestDictionaryPerformance() {
            IDictionary<string, int>[] dictionaries = new IDictionary<string, int>[] {
                new Dictionary<string, int>(),
                new SortedDictionary<string,int>(),
                new SortedList<string, int>()
            };
            var data = dictionaries.Select(x => EvaluateDictionaryPerformance(x)).ToList();
            Console.WriteLine("Add/Search test results : ");
            data.ForEach(x => Console.WriteLine("Add time: {0}  Search time: {1} => {2}", x.Item2, x.Item3, x.Item1));
        }

        #endregion Performance tests

        #region GetFibonacciSequence tests
        private void GetFibonacciSequenceTest(int count, IEnumerable<int> expected, string assertMessage) {
            var actual = Task.GetFibonacciSequence(count);
            Assert.IsTrue(expected.IsEqual(actual), assertMessage);
        }

        [TestMethod()]
        [TestCategory("GetFibonacciSequence")]
        public void GetFibonacciSequence_Should_Return_Right_Result() {
            GetFibonacciSequenceTest(12,
                new[] { 1, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144 },
                "GetFibonacciSequence should return the 1,1,2,3,5,8,13,21,34,55,89,144,..."
                );
            GetFibonacciSequenceTest(0,
                new int[] { },
                "GetFibonacciSequence should return the empty sequence if count=0"
                );
        }

        [TestMethod()]
        [TestCategory("GetFibonacciSequence")]
        [ExpectedException(typeof(ArgumentException), "GetFibonacciSequence should throw ArgumentException if count is negative")]
        public void GetFibonacciSequence_Should_Throw_ArgumentException_If_count_Is_Negative() {
            GetFibonacciSequenceTest(-1, new int[] { }, null);
        }
        #endregion GetFibonacciSequence tests

        #region Tokenize tests
        public class MockLargeStringReader : StringReader {
            public MockLargeStringReader(string text) : base(text) { }
            //public override string ReadLine() { throw new OutOfMemoryException(); }
            public override string ReadToEnd() { throw new OutOfMemoryException(); }
        }

        private void TokenizeTest(TextReader reader, IEnumerable<string> expected, string assertMessage) {
            var actual = Task.Tokenize(reader);
            Assert.IsTrue(expected.IsEqual(actual), assertMessage);
        }

        [TestMethod]
        [TestCategory("Tokenize")]
        public void Tokenize_Should_Split_Text_By_Words() {
            string testText = "TextReader is the abstract base " +
            "class of StreamReader and StringReader, which read " +
            "characters from streams and strings, respectively.\n\n" +
            "Create an instance of TextReader to open a text file " +
            "for reading a specified range of characters, or to " +
            "create a reader based on an existing stream.\n\n" +
            "You can also use an instance of TextReader to read " +
            "text from a custom backing store using the same " +
            "APIs you would use for a string or a stream.\n\n";
            using (var reader = new MockLargeStringReader(testText)) {
                TokenizeTest(reader, testText.Split(new[] {' ','\n',',','.','\t'}, StringSplitOptions.RemoveEmptyEntries), 
                    "Tokenize should split text by words");
            }

            using (var reader = new MockLargeStringReader("oneword")) {
                TokenizeTest(reader, new[] {"oneword"},
                    "Tokenize should return the text if text is onw word only");
            }

            using (var reader = new MockLargeStringReader("\n")) {
                TokenizeTest(reader, Enumerable.Empty<string>(),
                    "Tokenize should return empty sequence if text is empty");
            }

            TokenizeTest(TextReader.Null, Enumerable.Empty<string>(),
                "Tokenize should return empty sequence if reader is empty");
        }

        [TestMethod]
        [TestCategory("Tokenize")]
        [ExpectedException(typeof(ArgumentNullException), "Tokenize should throw ArgumentNullException if TextReader is null")]
        public void Tokenize_Should_Throw_ArgumentNullException_If_Reader_Is_Null() {
            TokenizeTest(null, Enumerable.Empty<string>(),"");
        }


        #endregion Tokenize tests

        #region TraversalTree mocks

        class MockNode<T> : ITreeNode<T> {
            public T Data { set; get; }
            public IEnumerable<ITreeNode<T>> Children { set; get; }
        }

        class IntNode : MockNode<int> { }

        private static int MaxNodeCount = 100000;

        private IntNode CreateDeepTree() {
            IntNode root = new IntNode() { Data = MaxNodeCount };
            for (int i = MaxNodeCount-1; i > 0; i--) {
                root = new IntNode() { Data = i, Children = new[] { root } };
            }
            return root;
        }

        private IntNode CreateWideTree() {
            IntNode[] children = new IntNode[MaxNodeCount-1];
            IntNode root = new IntNode() { Data = 1, Children = children };
            for (int i = 2; i <= MaxNodeCount; i++) {
                children[i - 2] = new IntNode() { Data = i };
            }
            return root;
        }

        #endregion TraversalTree mocks

        #region DepthTraversalTree tests
        private void DepthTraversalTreeTest<T>(ITreeNode<T> root, IEnumerable<T> expected ,string assertMessage) {
            var actual = Task.DepthTraversalTree(root);
            Assert.IsTrue(expected.IsEqual(actual), assertMessage);
        }

        [TestMethod()]
        [TestCategory("DepthTraversalTree")]
        public void DepthTraversalTree_Should_Return_Right_Result() {
            DepthTraversalTreeTest(
                new IntNode() { Data = 1, Children = null },
                new[] { 1 },
                "DepthTraversalTree should return the root if root has no children"
                );
            
            DepthTraversalTreeTest(
                new IntNode() { Data = 1, 
                    Children = new[] { 
                        new IntNode() { Data = 12},
                        new IntNode() { Data = 13},
                        new IntNode() { Data = 14},
                    }
                },
                new[] { 1, 12, 13, 14 },
                "DepthTraversalTree should return the right result"
                );

            DepthTraversalTreeTest(
                new IntNode() {
                    Data = 1,
                    Children = new[] { 
                        new IntNode() { Data = 12},
                        new IntNode() { Data = 13 , Children = new[] {
                            new IntNode() { Data = 131 },
                            new IntNode() { Data = 132 },
                            new IntNode() { Data = 133, Children = new[] {
                                new IntNode() { Data=1331 },
                                new IntNode() { Data=1332 },
                            } },
                        }},
                        new IntNode() { Data = 14},
                    }
                },
                new[] { 1, 12, 13, 131, 132, 133, 1331, 1332, 14 },
                "DepthTraversalTree should return the right result"
                );
            
            DepthTraversalTreeTest(CreateDeepTree(), Enumerable.Range(1, MaxNodeCount), "DepthTraversalTree should proceed a deep tree");
            DepthTraversalTreeTest(CreateWideTree(), Enumerable.Range(1, MaxNodeCount), "DepthTraversalTree should proceed a wide tree");
        }

        [TestMethod()]
        [TestCategory("DepthTraversalTree")]
        [ExpectedException(typeof(ArgumentNullException), "DepthTraversalTree should throw ArgumentNullException if root is null")]
        public void DepthTraversalTree_Should_Throw_ArgumentNullException_If_Root_Is_Null() {
            DepthTraversalTreeTest(null, new int[] { }, null);
        }
        #endregion DepthTraversalTree tests

        #region WidthTraversalTree tests
        private void WidthTraversalTreeTest<T>(ITreeNode<T> root, IEnumerable<T> expected, string assertMessage) {
            var actual = Task.WidthTraversalTree(root);
            Assert.IsTrue(expected.IsEqual(actual), assertMessage);
        }

        [TestMethod()]
        [TestCategory("WidthTraversalTree")]
        public void WidthTraversalTree_Should_Return_Right_Result() {
            WidthTraversalTreeTest(
                new IntNode() { Data = 1, Children = null },
                new[] { 1 },
                "WidthTraversalTree should return the root if root has no children"
                );

            WidthTraversalTreeTest(
                new IntNode() {
                    Data = 1,
                    Children = new[] { 
                        new IntNode() { Data = 12},
                        new IntNode() { Data = 13},
                        new IntNode() { Data = 14},
                    }
                },
                new[] { 1, 12, 13, 14 },
                "WidthTraversalTree should return the right result"
                );

            WidthTraversalTreeTest(
                new IntNode() {
                    Data = 1,
                    Children = new[] { 
                        new IntNode() { Data = 12},
                        new IntNode() { Data = 13 , Children = new[] {
                            new IntNode() { Data = 131 },
                            new IntNode() { Data = 132 },
                            new IntNode() { Data = 133, Children = new[] {
                                new IntNode() { Data=1331 },
                                new IntNode() { Data=1332 },
                            } },
                        }},
                        new IntNode() { Data = 14},
                    }
                },
                new[] { 1, 12, 13, 14, 131, 132, 133, 1331, 1332 },
                "WidthTraversalTree should return the right result"
                );


            WidthTraversalTreeTest(CreateDeepTree(), Enumerable.Range(1, MaxNodeCount), "WidthTraversalTree should proceed a deep tree");
            WidthTraversalTreeTest(CreateWideTree(), Enumerable.Range(1, MaxNodeCount), "WidthTraversalTree should proceed a wide tree");
        }

        [TestMethod()]
        [TestCategory("WidthTraversalTree")]
        [ExpectedException(typeof(ArgumentNullException), "WidthTraversalTree should throw ArgumentNullException if root is null")]
        public void WidthTraversalTree_Should_Throw_ArgumentException_If_Root_Is_Null() {
            WidthTraversalTreeTest(null, new int[] { }, null);
        }
        #endregion WidthTraversalTree tests

        #region GenerateAllPermutations tests

        private void GenerateAllPermutationsTest<T>(T[] source, int count, IEnumerable<T[]> expected, string message) {
            IEnumerable<T[]> actual = Task.GenerateAllPermutations(source, count);
            Func<IEnumerable<T[]>,IEnumerable<string>> normalize = x=>x.Select(y=>string.Join(",",y)).OrderBy(y=>y);
            Assert.IsTrue(
                Enumerable.SequenceEqual(normalize(expected), normalize(actual)), 
                message);
        }

        [TestMethod]
        [TestCategory("GenerateAllPermutations")]
        public void GenerateAllPermutations_Should_Generate_Permutations() {
            GenerateAllPermutationsTest(
                new int[] { 1, 2, 3, 4 }, 1,
                new int[][] { new[] { 1 }, new[] { 2 }, new[] { 3 }, new[] { 4 } },
                "GenerateAllPermutations should return list of items if count=1");
            GenerateAllPermutationsTest(
                new int[] { 1, 2, 3, 4 }, 2,
                new int[][] { new[] { 1, 2 }, new[] { 1, 3 }, new[] { 1, 4 }, new[] { 2, 3 }, new[] { 2, 4 }, new[] { 3, 4 } },
                "GenerateAllPermutations should return list of all pairs if count=2");
            GenerateAllPermutationsTest(
                new int[] { 1, 2, 3, 4 }, 3,
                new int[][] { new[] { 1,2,3 }, new[] { 1,2,4 }, new[] { 1,3,4 }, new[] { 2,3,4 } },
                "GenerateAllPermutations should return list of all triples if count=3");
            GenerateAllPermutationsTest(
                new int[] { 1, 2, 3, 4 }, 4,
                new int[][] { new[] { 1, 2, 3, 4 } },
                "GenerateAllPermutations should return source if count=source.Length");

            string charArray = "qwertyuiopasdfghjkl";
            GenerateAllPermutationsTest(
                charArray.ToArray(), 1,
                charArray.Select(x => new char[] { x }),
                "GenerateAllPermutations should return list of items if count=1");
            GenerateAllPermutationsTest(
                charArray.ToArray(), charArray.Length,
                new char[][] { charArray.ToArray() },
                "GenerateAllPermutations should return source if count=source.Length");

            GenerateAllPermutationsTest(
                new int[] { 1, 2, 3, 4 }, 0,
                Enumerable.Empty<int[]>(),
                "GenerateAllPermutations should return an empty enumeration if count=0");
        }

        [TestMethod]
        [TestCategory("GenerateAllPermutations")]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "GenerateAllPermutations should throw an ArgumentOutOfRangeException if count is out of range")]
        public void GenerateAllPermutations_Should_Throw_ArgumentOutOfRangeException_If_Count_Is_Out_Of_Range() {
            GenerateAllPermutationsTest(
                new int[] { 1, 2, 3, 4 }, 5,
                Enumerable.Empty<int[]>(),
                "");
        }

        #endregion GenerateAllPermutation tests

        #region DictionaryExtentions.GetOrBuildValue tests
        [TestMethod]
        [TestCategory("DictionaryExtentions.GetOrBuildValue")]
        public void DictionaryExtentions_GetOrBuildValue_Should_Build_Value_And_Put_It_Into_Dictionary_If_Key_Does_Not_Exist() {
            IDictionary<int, string> cache = new Dictionary<int, string>();
            bool flag = false;
            string expectedValue = "expectedValue";
            int key = 1;
            var actualValue = cache.GetOrBuildValue(key, () => { flag = true; return expectedValue; });
            Assert.IsTrue(flag, "DictionaryExtentions.GetOrBuildValue should invoke builder if key does not exist in dictionary");
            Assert.IsTrue(ReferenceEquals(expectedValue, actualValue), "DictionaryExtentions.GetOrBuildValue should return the builded value if key does not exist in dictionary");
            Assert.IsTrue(ReferenceEquals(expectedValue, cache[key]), "DictionaryExtentions.GetOrBuildValue should put builded value into dictionary if key does not exist");
        }

        [TestMethod]
        [TestCategory("DictionaryExtentions.GetOrBuildValue")]
        public void DictionaryExtentions_GetOrBuildValue_Should_Return_The_Value_From_Dictionary_If_Key_Exists() {
            IDictionary<int, string> cache = new Dictionary<int, string>();
            bool flag = false;
            int key = 1;
            string expectedValue = "expectedValue";
            cache[key] = expectedValue;
            var actualValue = cache.GetOrBuildValue(key, () => { flag = true; return expectedValue; });
            Assert.IsFalse(flag, "DictionaryExtentions.GetOrBuildValue should not invoke builder if key exists in the dictionary");
            Assert.IsTrue(ReferenceEquals(expectedValue, actualValue), "DictionaryExtentions.GetOrBuildValue should return value from dictionary if key exist");
        }

        #endregion DictionaryExtentions.GetOrBuildValue tests
    }

    #region Test utils
    public static class Utils {

        public static bool IsEqual<T>(this IEnumerable<T> data1, IEnumerable<T> data2, Func<T,T,bool> equals = null) {
            bool result = true;
            if (equals == null) equals = (x, y) => x.Equals(y);
            using (IEnumerator<T> enum1 = data1.GetEnumerator(), enum2 = data2.GetEnumerator()) {
                while (true) {
                    bool next1 = enum1.MoveNext();
                    bool next2 = enum2.MoveNext();
                    if (next1 == next2) {
                        if (!next1) break;
                        var val1 = enum1.Current;
                        var val2 = enum2.Current;
                        if (val1 == null ? val2 != null : !equals(val1,val2)) {
                            result = false;
                            break;
                        }
                    } else {
                        result = false;
                        break;
                    }
                }
            }
            return result;
        }

        public static TimeSpan EvaluatePerformance(Action action) {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            action();
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }
    }
    #endregion Test utils
}
