using EnumerableTask;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Win32;

namespace EnumerableTest
{
    
 
    /// <summary>
    ///This is a test class for TaskTest and is intended
    ///to contain all TaskTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TaskTest {


        #region Test Utils
        private Tuple<DateTime, int> GenData(int year, int month, int day, int data) {
            return Tuple.Create(new DateTime(year, month, day), data);
        }

        private Tuple<DateTime, int,int> GenData2(int year, int month, int day, int data1, int data2) {
            return Tuple.Create(new DateTime(year, month, day), data1, data2);
        }
        #endregion Test Utils


        #region GetUppercaseStrings
        public void GetUppercaseStringsTest(IEnumerable<string> data, IEnumerable<string> expected, string assertMessage) {
            Task target = new Task();
            var actual = target.GetUppercaseStrings(data);
            Assert.IsTrue(expected.IsEqual(actual), assertMessage);
        }

        [TestMethod()] [TestCategory("GetUppercaseStrings")]
        public void GetUppercaseStrings_Should_Return_Uppercased_Strings() {
            GetUppercaseStringsTest(
                new[] { "a", "aa", "aaa", "aaaa", "aaaaa" },
                new[] { "A", "AA", "AAA", "AAAA", "AAAAA" },
                "GetUppercaseStrings should return uppercased source strings");
            GetUppercaseStringsTest(
                new string[] { },
                new string[] { },
                "GetUppercaseStrings should return empty sequnce if source is empty");
            GetUppercaseStringsTest(
                new[] { string.Empty, string.Empty, null, null, "     " },
                new[] { string.Empty, string.Empty, null, null, "     " },
                "GetUppercaseStrings should not transform empty strings and nulls");
        }
        #endregion GetUppercaseStrings

        #region GetStringsLength
        public void GetStringsLengthTest(IEnumerable<string> data, IEnumerable<int> expected, string assertMessage) {
            Task target = new Task(); 
            var actual = target.GetStringsLength(data);
            Assert.IsTrue(expected.IsEqual(actual), assertMessage);
        }

        [TestMethod()] [TestCategory("GetStringsLength")]
        public void GetStringsLength_Should_Return_Strings_Length() {
            GetStringsLengthTest(
                new[] { "a", "aa", "aaa", "aaaa", "aaaaa" },
                new[] { 1, 2, 3, 4, 5 },
                "GetStringsLength should return strings length");
            GetStringsLengthTest(
                new[] { "a", "a", "a", string.Empty, string.Empty },
                new[] { 1, 1, 1, 0, 0 },
                "GetStringsLength should return strings length (empty strings)");
            GetStringsLengthTest(
                new[] { "a", "a", "a", null, null },
                new[] { 1, 1, 1, 0, 0 },
                "GetStringsLength should return strings length (null)");
            GetStringsLengthTest(
                new[] { " ", "  ", "   " },
                new[] { 1, 2, 3 },
                "GetStringsLength should return strings length for whitespace strings (Length(\" \")=1, Length(\"  \")=2)");
        }

        #endregion GetStringLength

        #region GetSquareSequence tests
        public void GetSquareSequenceTest(IEnumerable<int> data, IEnumerable<long> expected, string assertMessage) {
            Task target = new Task(); 
            var actual = target.GetSquareSequence(data);
            Assert.IsTrue(expected.IsEqual(actual), assertMessage);
        }

        [TestMethod()] [TestCategory("GetSquareSequence")]
        public void GetSquareSequence_Should_Return_Square_Items() {
            GetSquareSequenceTest(
                new int[] { 10,   20,  30,  40,   50,   60,   70,   80,   90,   100 },
                new long[] { 100, 400, 900, 1600, 2500, 3600, 4900, 6400, 8100, 10000 },
                "GetSquareSequence should return squares of source sequence"
                );
            GetSquareSequenceTest(
                new int[] { 65536, 65537, 2147483647 },
                new long[] { 4294967296, 4295098369, 4611686014132420609 },
                "GetSquareSequence should return squares of source sequence for large numbers"
                );
            GetSquareSequenceTest(
                new int[] { },
                new long[] { },
                "GetSquareSequence should return empty sequence if source is empty"
                );
        }
        #endregion GetSquareSequence tests

        #region GetMovingSumSequence tests
        public void GetMovingSumSequenceTest(IEnumerable<int> data, IEnumerable<long> expected, string assertMessage) {
            Task target = new Task(); 
            var actual = target.GetMovingSumSequence(data);
            Assert.IsTrue(expected.IsEqual(actual), assertMessage);
        }

        [TestMethod()] [TestCategory("GetMovingSumSequence")]
        public void GetMovingSumSequence_Should_Return_Moving_Sum_Items() {
            GetMovingSumSequenceTest(
                new int[]  { 10, 20, 30,  40,  50,  60,  70,  80,  90, 100 },
                new long[] { 10, 30, 60, 100, 150, 210, 280, 360, 450, 550 },
                "GetMovingSum should return moving sum of source sequence"
                );
            GetMovingSumSequenceTest(
                new int[] { 10, -10, 10, -10, 10, -10 },
                new long[] { 10,  0, 10,   0, 10,   0 },
                "GetSquareSequence should return moving sum of source sequnce"
                );
            GetMovingSumSequenceTest(
                new int[] { },
                new long[] { },
                "GetSquareSequence should return empty sequnce if source is empty"
                );
        }
        #endregion GetMovingSumSequence tests

        #region GetEvenItems tests
        public void GetEvenItemsTest<T>(IEnumerable<T> data, IEnumerable<T> expected, string assertMessage) {
            Task target = new Task(); 
            var actual = target.GetEvenItems<T>(data);
            Assert.IsTrue(expected.IsEqual(actual), assertMessage);
        }

        [TestMethod()] [TestCategory("GetEvenItems")]
        public void GetEvenItems_Should_Return_EvenItems() {
            GetEvenItemsTest(
                new int[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110 },
                new int[] {     20,     40,     60,     80,     100 },
                "EvenItems should return even items"
                );
        }

        [TestMethod()] [TestCategory("GetEvenItems")]
        public void GetEvenItems_Should_Return_Empty_If_Sequence_Length_Is_Less_Than_2() {
            GetEvenItemsTest(
                new int[] { 10 },
                new int[] { },
                "EvenItems should return empty if sequence length < 2"
                );
        }
        #endregion GetEvenItems tests

        #region GetPrefixItems tests
        public void GetPrefixItemsTest(IEnumerable<string> data, string prefix, IEnumerable<string> expected, string assertMessage) {
            Task target = new Task(); 
            var actual = target.GetPrefixItems(data, prefix);
            Assert.IsTrue(expected.IsEqual(actual), assertMessage);
        }

        [TestMethod()] [TestCategory("GetPrefixItems")]
        public void GetPrefixItems_Should_Return_Items_Started_With_Required_Prefix() {
            string[] data = "A horse, a kindom for a horse!".Split(' ');
            string[] nulls = { null, null, null };
            string[] result = new string[] { "horse,", "horse!" };
            GetPrefixItemsTest(data, "horse", result,
                "PrefixItems should return items started with required prefix"
            );
            GetPrefixItemsTest(data, "HORSE", result,
                "PrefixItems should return items started with required prefix (uppercase)"
            );
            GetPrefixItemsTest(data.Concat(nulls), "HORSE", result,
                "PrefixItems should return items started with required prefix (test with nulls)"
            );
            GetPrefixItemsTest(data.Concat(nulls), string.Empty, data,
                "PrefixItems should return all not null items if prefix is empty string"
            );
        }

        [TestMethod()] [TestCategory("GetPrefixItems")]
        public void GetPrefixItems_Should_Return_Empty_Sequence_If_No_Items_With_Required_Prefix() {
            GetPrefixItemsTest(
                "A horse, a kindom for a horse!".Split(' '), 
                "cow",
                new string[] {},
                "EvenItems should return empty sequence if no items started with required prefix"
                );
        }

        [TestMethod()] [TestCategory("GetPrefixItems")] 
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetPrefixItems_Should_Raise_Exception_If_Prefix_Is_Null() {
            GetPrefixItemsTest(new string[] { }, null, new string[] { }, null);
        }

        #endregion GetEvenItems tests

        #region PropagateItemsByPostionIndex tests
        public void PropagateItemsByPostionIndexTest<T>(IEnumerable<T> data, IEnumerable<T> expected, string assertMessage) {
            Task target = new Task();
            var actual = target.PropagateItemsByPositionIndex<T>(data);
            Assert.IsTrue(expected.IsEqual(actual), assertMessage);
        }

        [TestMethod()]
        [TestCategory("PropagateItemsByPostionIndex")]
        public void PropagateItemsByPostionIndex_Should_Return_Right_Results() {
            PropagateItemsByPostionIndexTest(
                new int[] { },
                new int[] { },
                "PropagateItemsByPostionIndex should return empty sequence if source is empty"
                );
            PropagateItemsByPostionIndexTest(
                new int[] { 0 },
                new int[] { 0 },
                "PropagateItemsByPostionIndex should return the same sequence if source has one item only"
                );
            PropagateItemsByPostionIndexTest(
                new int[] { 1, 2, 3, 4, 5, 6 },
                new int[] { 1, 2,2,  3,3,3, 4,4,4,4, 5,5,5,5,5, 6,6,6,6,6,6 },
                "PropagateItemsByPostionIndex should propagate source items required times"
                );
        }
        #endregion GetEvenItems tests

        #region GetUsedChars tests
        public void GetUsedCharsTest(IEnumerable<string> data, IEnumerable<char> expected, string assertMessage) {
            Task target = new Task();
            var actual = target.GetUsedChars(data).OrderBy(x=>x);
            Assert.IsTrue(expected.OrderBy(x=>x).IsEqual(actual), assertMessage);
        }

        [TestMethod()] [TestCategory("GetUsedChars")]
        public void GetUsedChars_Should_Return_List_of_Chars() {
            GetUsedCharsTest(
                new[] {"aa","bbbb","ccccc","dddddd", "    "}, 
                new[] {'a', 'b', 'c', 'd',' '},
                "GetUsedChars should return list of used chars"
            );
            GetUsedCharsTest(
                new[] { "aa", "aaa", "aaaaaa", "aaaa" },
                new[] { 'a' },
                "GetUsedChars should return list of used chars"
            );
            GetUsedCharsTest(
                new[] { " ", "  ", "   ", "   " },
                new[] { ' ' },
                "GetUsedChars should return list of used chars"
            );
            GetUsedCharsTest(
                new[] { string.Empty, null },
                new char[] { },
                "GetUsedChars should correctly process with empty strings and nulls"
            );
            GetUsedCharsTest(
                new string[] {}, 
                new char[] {},
                "GetUsedChars should return empty sequence if source is empty"
            );
        }
        #endregion GetUsedChars tests

        #region GetStringOfSequence tests
        public void GetStringOfSequenceTest<T>(IEnumerable<T> data, string expected, string assertMessage) {
            Task target = new Task();
            var actual = target.GetStringOfSequence(data);
            Assert.IsTrue(expected.Equals(actual), assertMessage);
        }

        [TestMethod()] [TestCategory("GetStringOfSequence")]
        public void GetStringOfSequence_Should_Return_Right_Results() {
            GetStringOfSequenceTest(
                new string[] { "aa", "bb", "cc", " ", null },
                "aa,bb,cc, ,null",
                "GetStringOfSequence should return string for strings sequence"
            );
            GetStringOfSequenceTest(
                new[] { 1, 2, 3, 4 },
                "1,2,3,4",
                "GetStringOfSequence should return string for int sequence"
            );
            GetStringOfSequenceTest(
                new[] { string.Empty, string.Empty, string.Empty },
                ",,",
                "GetStringOfSequence should correctly proceed empty strings"
            );
            GetStringOfSequenceTest(
                new string[] { },
                string.Empty,
                "GetUsedChars should return empty string for empty sequence"
            );
        }

        //private long GetStringOfSequence_PerformanceTicks<T>(IEnumerable<T> data) {
        //    Task target = new Task();
        //    Stopwatch sw = new Stopwatch();
        //    sw.Start();
        //    var actual = target.GetStringOfSequence(data);
        //    sw.Stop();
        //    return sw.ElapsedTicks;
            
        //}

        //[TestMethod()] [TestCategory("GetStringOfSequence")]
        //public void GetStringOfSequence_Should_Have_Good_Performance() {
        //    long ticks_for_10_items = GetStringOfSequence_PerformanceTicks(Enumerable.Range(1,10));
        //    long ticks_for_10000_items = GetStringOfSequence_PerformanceTicks(Enumerable.Range(1, 10000));
        //    Assert.IsTrue(ticks_for_10000_items / ticks_for_10_items < 5, "GetStringOfSequence should be optimized for performance");
        //}
        #endregion GetStringOfSequence tests

        #region Get3TopItems tests
        private void Get3TopItemsTest(IEnumerable<int> data, IEnumerable<int> expected, string assertMessage) {
            Task target = new Task();
            var actual = target.Get3TopItems(data);
            Assert.IsTrue(expected.IsEqual(actual), assertMessage);
        }

        [TestMethod()] [TestCategory("Get3TopItems")]
        public void Get3TopItems_Should_Return_Square_Items() {
            Get3TopItemsTest(
                Enumerable.Range(1,100).ToArray(),
                new int[] { 100, 99, 98 },
                "Get3TopItems should return the 3 highest numbers"
                );
            Get3TopItemsTest(
                new int[] { 1, 2 },
                new int[] { 2, 1 },
                "Get3TopItems should return all items if source sequence consists of <3 items"
                );
            Get3TopItemsTest(
                new int[] { },
                new int[] { },
                "Get3TopItems should return empty sequence if source is empty"
                );
        }
        #endregion Get3TopItems tests

        #region GetCountOfGreaterThen10 tests
        private void GetCountOfGreaterThen10Test(IEnumerable<int> data, int expected, string assertMessage) {
            Task target = new Task();
            var actual = target.GetCountOfGreaterThen10(data);
            Assert.IsTrue(expected==actual, assertMessage);
        }

        [TestMethod()] [TestCategory("GetCountOfGreaterThen10")]
        public void GetCountOfGreaterThen10_Should_Return_Right_Result() {
            GetCountOfGreaterThen10Test(
                Enumerable.Range(-100, 200),
                89,
                "GetCountOfGreaterThen10 should return the right result"
                );
            GetCountOfGreaterThen10Test(
                Enumerable.Range(-100, 100),
                0,
                "GetCountOfGreaterThen10 should return the right result"
                );
            GetCountOfGreaterThen10Test(
                new int[] { },
                0,
                "GetCountOfGreaterThen10 should return 0 if source is empty"
                );
        }
        #endregion GetCountOfGreaterThen10 tests

        #region GetFirstContainsFirst tests
        private void GetFirstContainsFirstTest(IEnumerable<string> data, string expected, string assertMessage) {
            Task target = new Task();
            var actual = target.GetFirstContainsFirst(data);
            Assert.IsTrue(expected == actual, assertMessage);
        }

        [TestMethod()] [TestCategory("GetFirstContainsFirst")]
        public void GetFirstContainsFirst_Should_Return_Right_Result() {
            GetFirstContainsFirstTest(
                new[] { "a", "IT IS FIRST", "first item", "I am really first!" },
                "IT IS FIRST",
                "GetFirstContainsFirst should return the right result"
                );
            GetFirstContainsFirstTest(
                new[] { "a", null, "IT IS FiRSt", "first item", "I am really first!" },
                "IT IS FiRSt",
                "GetFirstContainsFirst should return the right result"
                );
            GetFirstContainsFirstTest(
                new[] { "a", null, "b", "c", "d" },
                null,
                "GetFirstContainsFirst should return the null if no items found"
                );
            GetFirstContainsFirstTest(
                new string[] { },
                null,
                "GetFirstContainsFirst should return null if source is empty"
                );
        }
        #endregion GetFirstContainsFirst tests

        #region GetCountOfStringsWithLengthEqualsTo3 tests
        private void GetCountOfStringsWithLengthEqualsTo3Test(IEnumerable<string> data, int expected, string assertMessage) {
            Task target = new Task();
            var actual = target.GetCountOfStringsWithLengthEqualsTo3(data);
            Assert.IsTrue(expected == actual, assertMessage);
        }

        [TestMethod()] [TestCategory("GetCountOfStringsWithLengthEqualsTo3")]
        public void GetCountOfStringsWithLengthEqualsTo3_Should_Return_Right_Result() {
            GetCountOfStringsWithLengthEqualsTo3Test(
                new[] { "a", "aa", "aaa", "aaaa", null, "" },
                1,
                "GetCountOfStringsWithLengthEqualsTo3 should return the right result"
                );
            GetCountOfStringsWithLengthEqualsTo3Test(
                new[] { "a", "aaa", "bbb", "ccc", "d", "e", null },
                3,
                "GetCountOfStringsWithLengthEqualsTo3 should return the right result"
                );
            GetCountOfStringsWithLengthEqualsTo3Test(
                new[] { "aaa", "aaa", "aaa", "aaa", "aaa", "aaa", null },
                1,
                "GetCountOfStringsWithLengthEqualsTo3 should return the right result (a lot of duplicates test)"
                );
            GetCountOfStringsWithLengthEqualsTo3Test(
                new[] { "a", null, "b", "c", "d" },
                0,
                "GetFirstContainsFirst should return 0 if no items found"
                );
            GetCountOfStringsWithLengthEqualsTo3Test(
                new string[] { },
                0,
                "GetCountOfStringsWithLengthEqualsTo3 should return 0 if source is empty"
                );
        }
        #endregion GetCountOfStringsWithLengthEqualsTo3 tests

        #region GetCountOfStrings tests
        private void GetCountOfStringsTest(IEnumerable<string> data, IEnumerable<Tuple<string,int>> expected, string assertMessage) {
            Task target = new Task();
            var actual = target.GetCountOfStrings(data);
            Assert.IsTrue(expected.IsEqual(actual), assertMessage);
        }

        [TestMethod()] [TestCategory("GetCountOfStrings")]
        public void GetCountOfStrings_Should_Return_Right_Result() {
            GetCountOfStringsTest(
                new[] { "a", "aa", "aaa", "aaaa", null, string.Empty },
                new[] { new Tuple<string, int>("a", 1), new Tuple<string, int>("aa", 1), 
                        new Tuple<string, int>("aaa", 1), new Tuple<string,int>("aaaa",1),
                        new Tuple<string,int>(null, 1), new Tuple<string,int>(string.Empty,1) },
                "GetCountOfStrings should return the right result"
                );
            GetCountOfStringsTest(
                new[] { "a", "b", "a", "b", "a", "b", null, null, null },
                new[] { new Tuple<string, int>("a", 3), new Tuple<string, int>("b", 3), 
                        new Tuple<string, int>(null, 3) },
                "GetCountOfStrings should return the right result"
                );
            GetCountOfStringsTest(
                new[] { "aaa", "aaa", "aaa", "aaa", "aaa", "aaa" },
                new[] { new Tuple<string, int>("aaa", 6) },
                "GetCountOfStrings should return the right result (duplicates test)"
                );
            GetCountOfStringsTest(
                Enumerable.Repeat("a",1000).Concat(Enumerable.Repeat("b",1000)).Concat(Enumerable.Repeat<string>(null,1000)),
                new[] { new Tuple<string, int>("a", 1000), new Tuple<string, int>("b", 1000), new Tuple<string, int>(null, 1000) },
                "GetCountOfStrings should return the right result (duplicates test)"
            );

            GetCountOfStringsTest(
                new string[] {},
                new Tuple<string, int>[] {} ,
                "GetCountOfStrings should return empty sequence if source is empty"
                );
        }
        #endregion GetCountOfStrings tests

        #region GetCountOfStringsWithMaxLenght tests
        private void GetCountOfStringsWithMaxLenghtTest(IEnumerable<string> data, int expected, string assertMessage) {
            Task target = new Task();
            var actual = target.GetCountOfStringsWithMaxLength(data);
            Assert.IsTrue(expected == actual, assertMessage);
        }

        [TestMethod()] [TestCategory("GetCountOfStringsWithMaxLenght")]
        public void GetCountOfStringsWithMaxLenght_Should_Return_Right_Result() {
            GetCountOfStringsWithMaxLenghtTest(
                new[] { "a", "aa", "aaa", "aaaa", null, "" },
                1,
                "GetCountOfStringsWithMaxLenght should return the right result"
                );
            GetCountOfStringsWithMaxLenghtTest(
                new[] { "a", "aaa", "bbb", "ccc", "d", "e", null },
                3,
                "GetCountOfStringsWithMaxLenght should return the right result"
                );
            GetCountOfStringsWithMaxLenghtTest(
                new[] { "aaa", "aaa", "aaa", "aaa", "aaa", "aaa" },
                6,
                "GetCountOfStringsWithMaxLenght should return the right result (a lot of duplicates test)"
                );
            GetCountOfStringsWithMaxLenghtTest(
                new[] { string.Empty, null, null, string.Empty, null },
                5,
                "GetCountOfStringsWithMaxLenght should return 0 if no items found"
                );
            GetCountOfStringsWithMaxLenghtTest(
                new string[] { },
                0,
                "GetCountOfStringsWithMaxLenght should return 0 if source is empty"
                );
        }
        #endregion GetCountOfStringsWithMaxLenght tests


        #region GetSpecificEventEntriesCount tests
        [TestMethod()] [TestCategory("GetSpecificEventEntriesCount")]
        public void GetSpecificEventEntriesCount_Should_Return_Right_Result() {
            int expected = (new EventLog("System", ".")).Entries.Count;
            var task = new Task();
            int actual = task.GetSpecificEventEntriesCount(EventLogEntryType.Error)
                        +task.GetSpecificEventEntriesCount(EventLogEntryType.FailureAudit)
                        +task.GetSpecificEventEntriesCount(EventLogEntryType.Information)
                        +task.GetSpecificEventEntriesCount(EventLogEntryType.SuccessAudit)
                        +task.GetSpecificEventEntriesCount(EventLogEntryType.Warning);
            Assert.IsTrue(expected == actual);
        }
        #endregion GetSpecificEventEntriesCount tests


        #region GetDigitCharsCount tests
        private void GetDigitCharsCountTest(string data, int expected, string assertMessage) {
            Task target = new Task();
            var actual = target.GetDigitCharsCount(data);
            Assert.IsTrue(expected==actual, assertMessage);
        }

        [TestMethod()]
        [TestCategory("GetDigitCharsCount")]
        public void GetDigitCharsCount_Should_Return_Number_Of_Digit_Chars() {
            GetDigitCharsCountTest("aaaa", 0,
                "GetDigitCharsCount should return 0 if data has no digits"
                );
            GetDigitCharsCountTest(string.Empty, 0,
                "GetDigitCharsCount should return 0 if data has no digits"
                );
            GetDigitCharsCountTest("1234", 4,
                "GetDigitCharsCount should return the number of digit chars"
                );
            GetDigitCharsCountTest("A1xB2", 2,
                "GetDigitCharsCount should return the number of digit chars"
                );
        }

        [TestMethod()] [TestCategory("GetDigitCharsCount")]
        [ExpectedException(typeof(ArgumentNullException), "GetDigitCharsCount should throw ArgumentNullException if data is null")]
        public void GetDigitCharsCount_Should_Throw_ArgumentException_If_data_Is_Null() {
            GetDigitCharsCountTest(null, 0, null);
        }
        #endregion GetDigitCharsCount tests


        #region GetIEnumerableTypesNames tests
        private void GetIEnumerableTypesNamesTest(Assembly assembly, string[] expected, string assertMessage) {
            Task target = new Task();
            var actual = target.GetIEnumerableTypesNames(assembly);
            foreach (var item in actual.OrderBy(x => x)) Console.Write(item + ","); Console.WriteLine();
            Assert.IsTrue(expected.IsEqual(actual.OrderBy(x => x)), assertMessage);
        }

        [TestMethod()] [TestCategory("GetIEnumerableTypesNames")]
        public void GetIEnumerableTypesNames_Should_Return_Right_Results() {
            GetIEnumerableTypesNamesTest(
                typeof(string).Assembly, /* mscorlib assembly */ 
               ("ApplicationTrustCollection,Array,ArrayList,AuthorizationRuleCollection,BaseChannelObjectWithProperties,BaseChannelSinkWithProperties,"
               +"BaseChannelWithProperties,BitArray,Collection`1,CollectionBase,CommonAcl,ConcurrentDictionary`2,ConcurrentQueue`1,ConcurrentStack`1,Dictionary`2,"
               +"DictionaryBase,DiscretionaryAcl,Evidence,GenericAcl,Hashtable,ICollection,ICollection`1,IdentityReferenceCollection,IDictionary,IDictionary`2,"
               +"IEnumerable`1,IList,IList`1,IProducerConsumerCollection`1,IResourceReader,KeyCollection,KeyContainerPermissionAccessEntryCollection,KeyedCollection`2,"
               +"List`1,NamedPermissionSet,PermissionSet,Queue,RawAcl,ReadOnlyCollection`1,ReadOnlyCollectionBase,ReadOnlyPermissionSet,ResourceReader,ResourceSet,SortedList,"
               +"Stack,String,SystemAcl,ValueCollection").Split(new[] {','}),
                "GetIEnumerableTypesNames should return right results"
                );
        }

        [TestMethod()]
        [TestCategory("GetIEnumerableTypesNames")]
        [ExpectedException(typeof(ArgumentNullException), "GetIEnumerableTypesNames should throw ArgumentNullException if assembly is null")]
        public void GetIEnumerableTypesNames_Should_Throw_ArgumentNullException_If_assembly_Is_Null() {
            GetIEnumerableTypesNamesTest(null, null, null);
        }
        #endregion GetIEnumerableTypesNames tests

        #region GetQuarterSales tests
        private void GetQuarterSalesTest(IEnumerable<Tuple<DateTime,int>> data, int[] expected, string assertMessage) {
            Task target = new Task();
            var actual = target.GetQuarterSales(data);
            Assert.IsTrue(expected.IsEqual(actual), assertMessage);
        }


        [TestMethod()] [TestCategory("GetQuarterSales")]
        public void GetQuarterSales_Should_Return_Right_Results() {
            GetQuarterSalesTest(
                new[] { GenData(2010, 1, 1, 10), GenData(2010, 2, 2, 10), GenData(2010, 3, 3, 10) }, 
                new [] { 30, 0, 0, 0},
                "GetQuarterSales should return right results"
                );
            GetQuarterSalesTest(
                new[] { GenData(2010, 1, 1, 10), GenData(2010, 4, 4, 10), GenData(2010, 10, 10, 10) },
                new[] { 10, 10, 0, 10 },
                "GetQuarterSales should return right results"
                );
            GetQuarterSalesTest(
                new Tuple<DateTime, int>[] {  },
                new[] { 0, 0, 0, 0 },
                "GetQuarterSales should return { 0,0,0,0 } if sales are empty"
                );
        }
        #endregion GetQuarterSales tests

        #region SortStringsByLengthAndAlphabet tests
        private void SortStringsByLengthAndAlphabetTest(IEnumerable<string> data, IEnumerable<string> expected, string assertMessage) {
            Task target = new Task();
            var actual = target.SortStringsByLengthAndAlphabet(data);
            Assert.IsTrue(expected.IsEqual(actual), assertMessage);
        }

        [TestMethod()]
        [TestCategory("SortStringsByLengthAndAlphabet")]
        public void SortStringsByLengthAndAlphabet_Should_Return_Right_Results() {
            SortStringsByLengthAndAlphabetTest(
                new string[] { },
                new string[] { },
                "SortStringsByLengthAndAlphabet should return empty result if data is empty"
                );
            SortStringsByLengthAndAlphabetTest(
                new[] { "c","b","a" },
                new[] { "a","b","c" },
                "GetQuarterSales should return right results"
                );
            SortStringsByLengthAndAlphabetTest(
                new[] { "c","cc","b","bb","a","aa"},
                new[] { "a","b","c", "aa","bb", "cc"},
                "GetQuarterSales should return right result"
                );
            SortStringsByLengthAndAlphabetTest(
                new[] { "aaaaa", "aaaa", "aaa", "aa", "a" },
                new[] { "a", "aa", "aaa", "aaaa", "aaaaa" },
                "GetQuarterSales should return right result"
                );
        }
        #endregion SortStringsByLengthAndAlphabet tests

        #region GetMissingDigits tests
        private void GetMissingDigitsTest(IEnumerable<string> data, IEnumerable<char> expected, string assertMessage) {
            Task target = new Task();
            var actual = target.GetMissingDigits(data);
            Assert.IsTrue(expected.IsEqual(actual), assertMessage);
        }

        [TestMethod()][TestCategory("GetMissingDigits")]
        public void GetMissingDigits_Should_Return_Right_Results() {
            GetMissingDigitsTest(
                new string[] { },
                "0123456789",
                "GetMissingDigits should return all digits if data is empty"
                );
            GetMissingDigitsTest(
                new[] {"aaa","a1","b","c2","d","e3","f01234"},
                "56789",
                "GetMissingDigits should return right results"
                );
            GetMissingDigitsTest(
                new[] {"a","b","c","9876543210"},
                String.Empty,
                "GetMissingDigits should return right result"
                );
            GetMissingDigitsTest(
                new[] { "aaaaa", "aaaa", "aaa", "aa", "a","67890","67890","67890" },
                "12345",
                "GetMissingDigits should return right result"
                );
        }
        #endregion GetMissingDigits tests

        #region SortDigitNamesByNumericOrder tests
        private void SortDigitNamesByNumericOrderTest(IEnumerable<string> data, IEnumerable<string> expected, string assertMessage) {
            Task target = new Task();
            var actual = target.SortDigitNamesByNumericOrder(data);
            Assert.IsTrue(expected.IsEqual(actual), assertMessage);
        }

        [TestMethod()][TestCategory("SortDigitNamesByNumericOrder")]
        public void SortDigitNamesByNumericOrder_Should_Return_Right_Results() {
            SortDigitNamesByNumericOrderTest(
                new string[] { },
                new string[] { },
                "GetMissingDigits should return empty sequence if data is empty"
                );
            SortDigitNamesByNumericOrderTest(
                new[] { "nine", "one" },
                new[] { "one", "nine" },
                "GetMissingDigits should return right results"
                );
            SortDigitNamesByNumericOrderTest(
                new[] { "one", "two", "three" },
                new[] { "one", "two", "three" },
                "GetMissingDigits should return right result"
                );
            SortDigitNamesByNumericOrderTest(
                new[] {"nine","eight","nine","eight"},
                new[] {"eight","eight","nine","nine"},
                "GetMissingDigits should return right result"
                );
            SortDigitNamesByNumericOrderTest(
                new[] { "one", "one", "one", "zero" },
                new[] { "zero", "one", "one", "one" },
                "GetMissingDigits should return right result"
                );
            SortDigitNamesByNumericOrderTest(
               new[] { "nine", "eight", "seven", "six","five","four","three","two","one","zero" },
               new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" },
               "GetMissingDigits should return right result"
            );
        }
        #endregion SortDigitNamesByNumericOrder tests


        #region CombineNumbersAndFruits tests
        private void CombineNumbersAndFruitsTest(IEnumerable<string> numbers, IEnumerable<string> fruits, IEnumerable<string> expected, string assertMessage) {
            Task target = new Task();
            var actual = target.CombineNumbersAndFruits(numbers, fruits);
            Assert.IsTrue(expected.IsEqual(actual), assertMessage);
        }

        [TestMethod()]
        [TestCategory("CombineNumbersAndFruits")]
        public void CombineNumbersAndFruits_Should_Return_Right_Results() {
            CombineNumbersAndFruitsTest(
                new string[] { "one", "two", "three" },
                new string[] { },
                new string[] { },
                "CombineNumbersAndFruits should return empty sequence if fruits sequence is empty"
                );
            CombineNumbersAndFruitsTest(
                new string[] {  },
                new[] { "apple", "bananas", "pineapples" },
                new string [] { }, 
                "CombineNumbersAndFruits should return empty sequence if numbers sequence is empty"
                );
            CombineNumbersAndFruitsTest(
                new[] { "one", "two", "three" },
                new[] { "apple", "bananas", "pineapples" },
                new[] { "one apple", "two bananas", "three pineapples" },
                "CombineNumbersAndFruits should return right result"
                );
            CombineNumbersAndFruitsTest(
                new[] { "one", "two" },
                new[] { "apple", "bananas", "pineapples" },
                new[] { "one apple", "two bananas" },
                "CombineNumbersAndFruits should return right result"
                );
            CombineNumbersAndFruitsTest(
                new[] { "one", "two", "three" },
                new[] { "apple" },
                new[] { "one apple" },
                "CombineNumbersAndFruits should return right result"
                );
            CombineNumbersAndFruitsTest(
                Enumerable.Repeat("one",100),
                Enumerable.Repeat("apple",100),
                Enumerable.Repeat("one apple",100),
                "CombineNumbersAndFruits should return right result"
            );
        }
        #endregion CombineNumbersAndFruits tests


        #region GetCommonChars tests
        private void GetCommonCharsTest(IEnumerable<string> data, IEnumerable<char> expected, string assertMessage) {
            Task target = new Task();
            var actual = target.GetCommonChars(data);
            Assert.IsTrue(expected.IsEqual(actual), assertMessage);
        }

        [TestMethod()][TestCategory("GetCommonChars")]
        public void GetCommonChars_Should_Return_Right_Results() {
            GetCommonCharsTest(
                new string[] { },
                string.Empty,
                "GetCommonChars should return empty sequence if data is empty"
                );
            GetCommonCharsTest(
                new[] { "ab", "ac", "ad" },
                "a",
                "GetCommonChars should return right results"
                );
            GetCommonCharsTest(
                new[] { "a", "b", "c" },
                string.Empty,
                "GetCommonChars should return right result"
                );
            GetCommonCharsTest(
                new[] { "a", "aa", "aaa", "aaaa" },
                "a",
                "GetCommonChars should return right result"
                );
            GetCommonCharsTest(
                new[] { "ab", "ba", "aabb", "baba" },
                "ab",
                "GetCommonChars should return right result"
                );
            GetCommonCharsTest(
               new[] { string.Empty, "a", "aa", "aaa" },
               string.Empty,
               "GetCommonChars should return right result"
            );
        }
        #endregion GetCommonChars tests


        #region GetStringsOnly tests
        private void GetStringsOnlyTest(object[] data, IEnumerable<string> expected, string assertMessage) {
            Task target = new Task();
            var actual = target.GetStringsOnly(data);
            Assert.IsTrue(expected.IsEqual(actual), assertMessage);
        }

        [TestMethod()]
        [TestCategory("GetStringsOnly")]
        public void GetStringsOnly_Should_Return_Right_Results() {
            GetStringsOnlyTest(
                new object[] { },
                new string[] { },
                "GetStringsOnly should return empty sequence if data is empty"
                );
            GetStringsOnlyTest(
                new object[] { "a", 1, 2, null, "b", true, 4.5, "c" },
                new[] { "a", "b", "c" },
                "GetStringsOnly should return right results"
                );
            GetStringsOnlyTest(
                new object[] { "a", "b", "c" },
                new[] { "a", "b", "c" },
                "GetStringsOnly should return right result"
                );
            GetStringsOnlyTest(
                new object[] { 'a', 1, 2, null, 'b', true, 4.5, 'c' },
                new string[] { },
                "GetStringsOnly should return right result"
                );
            GetStringsOnlyTest(
                new[] { String.Empty, null },
                new[] { String.Empty },
                "GetStringsOnly should return right result"
                );
        }
        #endregion GetStringsOnly tests

        #region GetSumOfAllInts tests
        private void GetSumOfAllIntsTest(object[] data, int expected, string assertMessage) {
            Task target = new Task();
            var actual = target.GetSumOfAllInts(data);
            Assert.AreEqual(expected,actual, assertMessage);
        }

        [TestMethod()]
        [TestCategory("GetSumOfAllInts")]
        public void GetSumOfAllInts_Should_Return_Right_Results() {
            GetSumOfAllIntsTest(
                new object[] { 1, true, "a", "b", false, 1 }, 2,
                "GetSumOfAll should return the sum of all ints"
                );
            GetSumOfAllIntsTest(
                new object[] { true, false }, 0,
                "GetSumOfAll should return zero if data does not contain integers"
                );
            GetSumOfAllIntsTest(
                new object[] { 10, "ten", 10 }, 20,
                "GetSumOfAll should return the sum of all ints"
            );
            GetSumOfAllIntsTest(
                new object[] { }, 0,
                "GetSumOfAll should return zero if array is empty"
                );
        }
        #endregion GetSumOfAllInts tests

        #region GetTotalStringsLength tests
        private void GetTotalStringsLengthTest(string[] data, int expected, string assertMessage) {
            Task target = new Task();
            var actual = target.GetTotalStringsLength(data);
            Assert.AreEqual(expected, actual, assertMessage);
        }

        [TestMethod()][TestCategory("GetTotalStringsLength")]
        public void GetTotalStringsLength_Should_Return_Right_Results() {
            GetTotalStringsLengthTest(
                new string[] { "a", "b", "c","d", "e","f" }, 6,
                "GetTotalStringsLength should return the sum of strings length"
                );
            GetTotalStringsLengthTest(
                new string[] { "a", "aa", "aaa" }, 6,
                "GetTotalStringsLength should return the sum of strings length"
                );
            GetTotalStringsLengthTest(
                new string[] { "1234567890" }, 10,
                "GetTotalStringsLength should return the sum of strings length"
                );
            GetTotalStringsLengthTest(
                new string[] { null, string.Empty, "a" }, 1,
                "GetTotalStringsLength should return the sum of strings length"
                );
            GetTotalStringsLengthTest(
                new string[] { null, string.Empty }, 0,
                "GetTotalStringsLength should return the sum of strings length"
                );
            GetTotalStringsLengthTest(
                new string[] { null, string.Empty }, 0,
                "GetTotalStringsLength should return zero if array is empty"
                );
        }
        #endregion GetTotalStringsLength tests

        #region IsSequenceHasNulls tests
        private void IsSequenceHasNullsTest(string[] data, bool expected, string assertMessage) {
            Task target = new Task();
            var actual = target.IsSequenceHasNulls(data);
            Assert.AreEqual(expected, actual, assertMessage);
        }

        [TestMethod()]
        [TestCategory("IsSequenceHasNulls")]
        public void IsSequenceHasNulls_Should_Return_Right_Results() {
            IsSequenceHasNullsTest(
                new string[] { "a", "b", "c", "d", "e", "f" }, false,
                "IsSequenceHasNulls should return right results"
                );
            IsSequenceHasNullsTest(
                new string[] { "a", "aa", "aaa", null }, true,
                "IsSequenceHasNulls should return right results"
                );
            IsSequenceHasNullsTest(
                new string[] { string.Empty }, false,
                "IsSequenceHasNulls should return right results"
                );
            IsSequenceHasNullsTest(
                new string[] { null, null, null }, true,
                "IsSequenceHasNulls should return right results"
                );
            IsSequenceHasNullsTest(
                new string[] { }, false,
                "IsSequenceHasNulls should return right results"
                );
        }
        #endregion IsSequenceHasNulls tests

        #region IsAllStringsAreUppercase tests
        private void IsAllStringsAreUppercaseTest(IEnumerable<string> data, bool expected, string assertMessage) {
            Task target = new Task();
            var actual = target.IsAllStringsAreUppercase(data);
            Assert.AreEqual(expected, actual, assertMessage);
        }

        [TestMethod()][TestCategory("IsAllStringsAreUppercase")]
        public void IsAllStringsAreUppercase_Should_Return_Right_Results() {
            IsAllStringsAreUppercaseTest(
                new string[] { "A", "B", "C", "D", "E", "F" }, true,
                "IsAllStringsAreUppercase should return right results"
                );
            IsAllStringsAreUppercaseTest(
                new string[] { "A", "AA", "AAAA", "AAAa" }, false,
                "IsAllStringsAreUppercase should return right results"
                );
            IsAllStringsAreUppercaseTest(
                new string[] { string.Empty }, false,
                "IsAllStringsAreUppercase should return right results"
                );
            IsAllStringsAreUppercaseTest(
                new string[] { }, false,
                "IsAllStringsAreUppercase should return false if sequence is empty"
                );
        }
        #endregion IsAllStringsAreUppercase tests

        #region GetFirstNegativeSubsequence tests
        private void GetFirstNegativeSubsequenceTest(IEnumerable<int> data, IEnumerable<int> expected, string assertMessage) {
            Task target = new Task();
            var actual = target.GetFirstNegativeSubsequence(data);
            Assert.IsTrue(expected.IsEqual(actual), assertMessage);
        }

        [TestMethod()]
        [TestCategory("GetFirstNegativeSubsequence")]
        public void GetFirstNegativeSubsequence_Should_Return_Right_Results() {
            GetFirstNegativeSubsequenceTest(
                new int[] { -2, -1, 0, 1, 2 }, 
                new int[] { -2, -1 },
                "GetFirstNegativeSubsequence should return right results"
                );
            GetFirstNegativeSubsequenceTest(
                new int[] { 2, 1, 0, -1, -2 }, 
                new int[] { -1, -2 },
                "IsAllStringsAreUppercase should return right results"
                );
            GetFirstNegativeSubsequenceTest(
                new int[] { 1, 1, 1, -1, -1, -1, 0, 0, 0, -2, -2, -2 }, 
                new int[] { -1, -1, -1 },
                "GetFirstNegativeSubsequence should return right results"
                );
            GetFirstNegativeSubsequenceTest(
                new int[] { -1, 0, -2 },
                new int[] { -1 },
                "GetFirstNegativeSubsequence should return right results"
                );
            GetFirstNegativeSubsequenceTest(
                new int[] { 1, 2, 3 },
                new int[] { },
                "GetFirstNegativeSubsequence should return right results"
                );
            GetFirstNegativeSubsequenceTest(
                new int[] { }, 
                new int[] { },
                "GetFirstNegativeSubsequence should return empty sequence if source sequence is empty"
                );
        }
        #endregion GetFirstNegativeSubsequence tests


        #region AreNumericListsEqual tests
        private void AreNumericListsEqualTest(IEnumerable<int> integers, IEnumerable<double> doubles, bool expected, string assertMessage) {
            Task target = new Task();
            var actual = target.AreNumericListsEqual(integers, doubles);
            Assert.AreEqual(expected, actual, assertMessage);
        }

        [TestMethod()]
        [TestCategory("AreNumericListsEqual")]
        public void AreNumericListsEqual_Should_Return_Right_Results() {
            AreNumericListsEqualTest(
                new int[] { 1, 2, 3 }, 
                new double[] { 1.0, 2.0, 3.0 },
                true,
                "AreNumericListsEqual should return right results"
                );
            AreNumericListsEqualTest(
                new int[] { 1, 2, 3 },
                new double[] { 3.0, 2.0, 1.0 },
                false,
                "AreNumericListsEqual should return right results"
                );
            AreNumericListsEqualTest(
                Enumerable.Range(1,100),
                Enumerable.Range(1,100).Select(x=>(double)x),
                true,
                "AreNumericListsEqual should return right results"
                );
            AreNumericListsEqualTest(
                new int[] { -10 },
                new double[] { -10.0 },
                true,
                "AreNumericListsEqual should return right results"
                );
        }
        #endregion AreNumericListsEqual tests

        #region GetNextVersionFromList tests
        private void GetNextVersionFromListTest(IEnumerable<string> data, string currVersion, string expected, string assertMessage) {
            Task target = new Task();
            var actual = target.GetNextVersionFromList(data, currVersion);
            Assert.AreEqual(expected, actual, assertMessage);
        }

        [TestMethod()]
        [TestCategory("GetNextVersionFromList")]
        public void GetNextVersionFromList_Should_Return_Right_Results() {
            GetNextVersionFromListTest(
                new[] { "1.1", "1.2", "1.5", "2.0" }, "1.2", "1.5",
                "GetNextVersionFromList should return right results"
                );
            GetNextVersionFromListTest(
                new[] { "1.1", "1.2", "1.5", "2.0" }, "1.4", null,
                "GetNextVersionFromList should return null if current version is not in source list"
                );
            GetNextVersionFromListTest(
                new[] { "1.1", "1.2", "1.5", "2.0" }, "2.0", null,
                "GetNextVersionFromList should return null if current version is last"
                );
        }
        #endregion GetNextVersionFromList tests


        #region GetSumOfVectors tests
        private void GetSumOfVectorsTest(IEnumerable<int> v1, IEnumerable<int> v2, IEnumerable<int> expected, string assertMessage) {
            Task target = new Task();
            var actual = target.GetSumOfVectors(v1, v2);
            Assert.IsTrue(expected.IsEqual(actual), assertMessage);
        }

        [TestMethod()]
        [TestCategory("GetSumOfVectors")]
        public void GetSumOfVectors_Should_Return_Right_Results() {
            GetSumOfVectorsTest(
                new[] { 1,   2,  3 },
                new[] { 10, 20, 30 },
                new[] { 11, 22, 33 },
                "GetSumOfVectors should return right results"
                );
            GetSumOfVectorsTest(
                new[] {  1,  1,  1 },
                new[] { -1, -1, -1 },
                new[] {  0,  0,  0 },
                "GetSumOfVectors should return right results"
                );
            GetSumOfVectorsTest(
                Enumerable.Range(1, 100),
                Enumerable.Range(1, 100),
                Enumerable.Range(1, 100).Select(x=>x+x),
                "GetSumOfVectors should return right results"
                );
        }
        #endregion GetSumOfVectors tests


        #region GetProductOfVectors tests
        private void GetProductOfVectorsTest(IEnumerable<int> v1, IEnumerable<int> v2, int expected, string assertMessage) {
            Task target = new Task();
            var actual = target.GetProductOfVectors(v1, v2);
            Assert.AreEqual(expected,actual, assertMessage);
        }

        [TestMethod()]
        [TestCategory("GetProductOfVectors")]
        public void GetProductOfVectors_Should_Return_Right_Results() {
            GetProductOfVectorsTest(
                new[] { 1, 2, 3 },
                new[] { 1, 2, 3 },
                1+2*2+3*3,
                "GetSumOfVectors should return right results"
                );
            GetProductOfVectorsTest(
                new[] { 1, 1, 1 },
                new[] { -1, -1, -1 },
                -3,
                "GetProductOfVectors should return right results"
                );
            GetProductOfVectorsTest(
                new[] { 1, 1, 1 },
                new[] { 0, 0, 0 },
                0,
                "GetProductOfVectors should return right results"
                );
        }
        #endregion GetProductOfVectors tests

        #region GetAllPairs tests
        private void GetAllPairsTest(IEnumerable<string> boys, IEnumerable<string> girls, IEnumerable<string> expected, string assertMessage) {
            Task target = new Task();
            var actual = target.GetAllPairs(boys, girls);
            Assert.IsTrue(expected.IsEqual(actual), assertMessage);
        }

        [TestMethod()]
        [TestCategory("GetAllPairs")]
        public void GetAllPairs_Should_Return_Right_Results() {
            GetAllPairsTest(
                new[] {"John", "Josh", "Jacob" }, 
                new[] {"Ann", "Alice"},
                new[] { "John+Ann", "John+Alice", "Josh+Ann", "Josh+Alice", "Jacob+Ann", "Jacob+Alice" },
                "GetAllPairs should return right results"
                );
            GetAllPairsTest(
                new[] { "John" },
                new[] { "Ann" },
                new[] { "John+Ann" },
                "GetAllPairs should return right results"
                );
            GetAllPairsTest(
                new[] { "John", "Josh", "Jacob" },
                new string[] {  },
                new string[] {  },
                "GetAllPairs should return empty sequence if girls are empty"
            );
            GetAllPairsTest(
                new string[] { },
                new string[] { "Ann", "Alice" },
                new string[] { },
                "GetAllPairs should return empty sequence if boys are empty"
            );
        }
        #endregion GetAllPairs tests


        #region GetAverageOfDoubleValues tests
        private void GetAverageOfDoubleValuesTest(IEnumerable<object> data, double expected, string assertMessage) {
            Task target = new Task();
            var actual = target.GetAverageOfDoubleValues(data);
            Assert.AreEqual(expected, actual, assertMessage);
        }

        [TestMethod()]
        [TestCategory("GetAverageOfDoubleValues")]
        public void GetAverageOfDoubleValues_Should_Return_Right_Results() {
            GetAverageOfDoubleValuesTest(
                new object[] { 1.0, 2.0, "3.0", null, 10 },  1.5,
                "GetAverageOfDoubleValues should return right results"
                );
            GetAverageOfDoubleValuesTest(
                new object[] { 1.0, 2.0, 3.0, 4.0, 5.0 }, 3.0,
                "GetAverageOfDoubleValues should return right results"
                );
            GetAverageOfDoubleValuesTest(
                new object[] { "1.0", "2.0", "3.0", "4.0", "5.0" }, 0.0,
                "GetAverageOfDoubleValues should return right results"
                );
            GetAverageOfDoubleValuesTest(
                new object[] { }, 0.0,
                "GetAverageOfDoubleValues should return 0.0 if sequence is empty"
                );
        }
        #endregion GetAverageOfDoubleValues tests



    }

    #region TestUtils
    public static class Utils {

        public static bool IsEqual<T>(this IEnumerable<T> data1, IEnumerable<T> data2) {
            bool result = true;
            using (IEnumerator<T> enum1 = data1.GetEnumerator(), enum2 = data2.GetEnumerator()) {
                while (true) {
                    bool next1 = enum1.MoveNext();
                    bool next2 = enum2.MoveNext();
                    if (next1 == next2) {
                        if (!next1) break;
                        var val1 = enum1.Current;
                        var val2 = enum2.Current;
                        if (val1==null? val2!=null : !val1.Equals(val2)) {
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

    }
    #endregion TestUtils
}
