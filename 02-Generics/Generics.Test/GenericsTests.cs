using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Task.Generics.Test {

    [TestClass]
    public class GenericsTest {

        #region ListConverter.ConvertToString tests
        public void ListConverter_ConvertToString_Test<T>(IEnumerable<T> list, string expected, string assertMessage) {
            var actual = list.ConvertToString();
            Assert.AreEqual(expected, actual, assertMessage);
        }

        [TestMethod]
        [TestCategory("ListConverter.ConvertToString")]
        public void ListConverter_ConvertToString_Should_Convert_List_To_String() {
            ListConverter_ConvertToString_Test(
                   new[] {1,2,3,4,5},
                   "1,2,3,4,5",
                   "ListConverter.ConvertToString should convert int list to string"
                );
            ListConverter_ConvertToString_Test(
                   new[] { '1', '2', '3', '4', '5' },
                   "1,2,3,4,5",
                   "ListConverter.ConvertToString should convert char list to string"
                );
            ListConverter_ConvertToString_Test(
                   new[] { true, true, false, false },
                   "True,True,False,False",
                   "ListConverter.ConvertToString should convert bool list to string"
                );
            ListConverter_ConvertToString_Test(
                   new[] { ConsoleColor.Black, ConsoleColor.Blue, ConsoleColor.Cyan },
                   "Black,Blue,Cyan",
                   "ListConverter.ConvertToString should convert enum list to string"
                );
            ListConverter_ConvertToString_Test(
                   new[] { new TimeSpan(1,0,0), new TimeSpan(0,0,30) },
                   "01:00:00,00:00:30",
                   "ListConverter.ConvertToString should convert TimeSpan to string"
            );

        }
        #endregion ListConverter.ConvertToString tests

        #region ListConverter.ConvertToList tests
        public void ListConverter_ConvertToList_Test<T>(string list, IEnumerable<T> expected, string assertMessage) {
            var actual = list.ConvertToList<T>();
            Assert.IsTrue(expected.SequenceEqual(actual), assertMessage);
        }

        [TestMethod]
        [TestCategory("ListConverter.ConvertToList")]
        public void ListConverter_ConvertToList_Should_Convert_String_To_List() {
            ListConverter_ConvertToList_Test(
                   "1,2,3,4,5",
                   new[] { 1, 2, 3, 4, 5 },
                   "ListConverter.ConvertToList<int> should convert string to int list"
                );
            ListConverter_ConvertToList_Test(
                   "1,2,3,4,5",
                   new[] { '1', '2', '3', '4', '5' },
                   "ListConverter.ConvertToList<char> should convert string to char list"
                );
            ListConverter_ConvertToList_Test(
                   "True,True,False,False",
                   new[] { true, true, false, false },
                   "ListConverter.ConvertToList<bool> should convert string to bool list"
                );
            ListConverter_ConvertToList_Test(
                   "Black,Blue,Cyan",
                   new[] { ConsoleColor.Black, ConsoleColor.Blue, ConsoleColor.Cyan },
                   "ListConverter.ConvertToList<ConsoleColor> should convert string to enum list"
                );
            ListConverter_ConvertToList_Test(
                   "1:00:00,0:00:30",
                   new[] { new TimeSpan(1, 0, 0), new TimeSpan(0, 0, 30) },
                   "ListConverter.ConvertToList<TimeSpan> should convert string to TimeSpan list"
                );
        }
        #endregion ListConverter.ConvertToString tests

        #region ArrayExtentions.SwapArrayElements tests
        public void ArrayExtentions_SwapArrayElements_Test<T>(T[] data, int index1, int index2, T[] expected, string assertMessage) {
            data.SwapArrayElements(index1, index2);
            Assert.IsTrue(expected.SequenceEqual(data), assertMessage);
        }

        [TestMethod]
        [TestCategory("ArrayExtentions.SwapArrayElements")]
        public void ArrayExtentions_SwapArrayElements_Should_Swap_Array_Elemets() {
            ArrayExtentions_SwapArrayElements_Test(
                   new[] { 1, 2, 3, 4, 5 }, 0, 1,
                   new[] { 2, 1, 3, 4, 5 },
                   "ArrayExtentions.SwapArrayElements should swap the specified array elements"
                );
            ArrayExtentions_SwapArrayElements_Test(
                   new[] { 1, 2, 3, 4, 5 }, 1, 0,
                   new[] { 2, 1, 3, 4, 5 },
                   "ArrayExtentions.SwapArrayElements should swap the specified array elements"
                );
            ArrayExtentions_SwapArrayElements_Test(
                   new[] { "1", "2", "3", "4", "5" }, 1, 1,
                   new[] { "1", "2", "3", "4", "5" },
                   "ArrayExtentions.SwapArrayElements should not swap if index1==index2"
                );
        }

        [TestMethod]
        [TestCategory("ArrayExtentions.SwapArrayElements")]
        [ExpectedException(typeof(IndexOutOfRangeException), 
            "ArrayExtentions.SwapArrayElements should throw the IndexOutOfRangeException if index1 or index2 is out of range")]
        public void ArrayExtentions_SwapArrayElements_Should_Throw_IndexOutOfRangeException_If_Indexes_Are_Out_Of_Range() {
            ArrayExtentions_SwapArrayElements_Test(
                   new[] { 1, 2, 3, 4, 5 }, 0, 10,
                   new[] { 2, 1, 3, 4, 5 },
                   string.Empty
                );
        }

        #endregion ArrayExtentions.SwapArrayElements tests

        #region ArrayExtentions.SortTupleArray tests
        public void ArrayExtentions_SortTupleArray_Test(Tuple<int,int,int>[] data, int colIndex, bool desc, Tuple<int,int,int>[] expected, string assertMessage) {
            data.SortTupleArray(colIndex, desc);
            Assert.IsTrue(expected.SequenceEqual(data), assertMessage);
        }

        [TestMethod]
        [TestCategory("ArrayExtentions.SortTupleArray")]
        public void ArrayExtentions_SortTupleArray_Should_Sort_Array() {
            ArrayExtentions_SortTupleArray_Test(
                   new[] { Tuple.Create(3,8,8), Tuple.Create(2,9,9), Tuple.Create(1,10,10) }, 0, true,
                   new[] { Tuple.Create(1,10,10), Tuple.Create(2,9,9), Tuple.Create(3,8,8) },
                   "ArrayExtentions.SortTupleArray should sort array by sortedColumn=0 and ascending=true"
                );
            ArrayExtentions_SortTupleArray_Test(
                   new[] { Tuple.Create(1, 12, 12), Tuple.Create(2, 11, 11), Tuple.Create(3, 10, 10) }, 0, false,
                   new[] { Tuple.Create(3, 10, 10), Tuple.Create(2, 11, 11), Tuple.Create(1, 12, 12) },
                   "ArrayExtentions.SortTupleArray should sort array by sortedColumn=0 and ascending=false"
                );
            ArrayExtentions_SortTupleArray_Test(
                   new[] { Tuple.Create(8, 3, 8), Tuple.Create(9, 2, 9), Tuple.Create(10, 1, 10) }, 1, true,
                   new[] { Tuple.Create(10, 1, 10), Tuple.Create(9, 2, 9), Tuple.Create(8, 3, 8) },
                   "ArrayExtentions.SortTupleArray should sort array  by sortedColumn=1 and ascending=true"
                );
            ArrayExtentions_SortTupleArray_Test(
                   new[] { Tuple.Create(12, 1, 12), Tuple.Create(11, 2, 11), Tuple.Create(10, 3, 10) }, 1, false,
                   new[] { Tuple.Create(10, 3, 10), Tuple.Create(11, 2, 11), Tuple.Create(12, 1, 12) },
                   "ArrayExtentions.SortTupleArray should sort array  by sortedColumn=1 and ascending=fasle"
                );
            ArrayExtentions_SortTupleArray_Test(
                   new[] { Tuple.Create(8, 8, 3), Tuple.Create(9, 9, 2), Tuple.Create(10, 10, 1) }, 2, true,
                   new[] { Tuple.Create(10, 10, 1), Tuple.Create(9, 9, 2), Tuple.Create(8, 8, 3) },
                   "ArrayExtentions.SortTupleArray should sort array  by sortedColumn=2 and ascending=true"
                );
            ArrayExtentions_SortTupleArray_Test(
                   new[] { Tuple.Create(12, 12, 1), Tuple.Create(11, 11, 2), Tuple.Create(10, 10, 3) }, 2, false,
                   new[] { Tuple.Create(10, 10, 3), Tuple.Create(11, 11, 2), Tuple.Create(12, 12, 1) },
                   "ArrayExtentions.SortTupleArray should sort array  by sortedColumn=2 and ascending=false"
                );

        }

        [TestMethod]
        [TestCategory("ArrayExtentions.SortTupleArray")]
        [ExpectedException(typeof(IndexOutOfRangeException),
            "ArrayExtentions.SortTupleArray should throw the IndexOutOfRangeException sortedColumn is out of range")]
        public void ArrayExtentions_SortTupleArray_Should_Throw_IndexOutOfRangeException_If_sortedColumn_Is_Out_Of_Range() {
            ArrayExtentions_SortTupleArray_Test(
                   new[] { Tuple.Create(1, 1, 1) }, 5, true,
                   new[] { Tuple.Create(1, 1, 1) },
                   "ArrayExtentions.SortTupleArray should sort array  by sortedColumn=2 and descending=true"
                );
        }

        #endregion ArrayExtentions.SortTupleArray tests

        #region Singleton<T> tests

        private const int SingletonIntValue = 100;
        private const string SingletonStringValue = "Test";

        public class MockSingleton {
            public int IntProperty { get; set; }
            public string StringProperty { get; set; }
            public MockSingleton() {
                IntProperty = SingletonIntValue;
                StringProperty = SingletonStringValue;
            }
        }

        [TestMethod]
        [TestCategory("Singleton<T>")]
        public void Singleton_Should_Create_Only_One_Instance() {
            var instance1 = Singleton<MockSingleton>.Instance;
            var instance2 = Singleton<MockSingleton>.Instance;
            instance1.IntProperty++;
            Assert.AreEqual(instance1.IntProperty, instance2.IntProperty, "Two instances of singleton reference to different objects");
        }

        [TestMethod]
        [TestCategory("Singleton<T>")]
        public void Singleton_Should_Be_Thread_Safe() {
			int threadsCount = 10;
			EventWaitHandle threadsFinished = new EventWaitHandle(false, EventResetMode.AutoReset);
			MockSingleton[] singletons = new MockSingleton[threadsCount];
			Thread[] threads = new Thread[threadsCount];
			Barrier startBarrier = new Barrier(threadsCount);
			Barrier finishBarrier = new Barrier(threadsCount, (barrier) => threadsFinished.Set());
			for (int i = 0; i < threadsCount; i++) {
				threads[i] = new Thread((idx) => {
					startBarrier.SignalAndWait(); // Wait for creating all other threads
					try {
						singletons[(int)idx] = Singleton<MockSingleton>.Instance;
					} catch { }
					finishBarrier.SignalAndWait(); // Wait for finishing all other threads
				});
				threads[i].Start(i);
			}
			threadsFinished.WaitOne();
			var inst = Singleton<MockSingleton>.Instance;
			inst.IntProperty++;
			Assert.IsTrue(singletons.All(x => x.IntProperty == inst.IntProperty), "Singleton<T> shoud inplement thread safe initialization");
        }

        #endregion Singleton<T> tests

        #region FunctionExtention.TimeoutSafeInvoke tests
        [TestMethod]
        [TestCategory("FunctionExtention.TimeoutSafeInvoke")]
        public void FunctionExtention_TimeoutSafeInvoke_Should_Invoke_Specified_Function() {
            int magicInt = 1234;
            Func<int> f = () => magicInt;
            Assert.AreEqual(magicInt, f.TimeoutSafeInvoke(), "FunctionExtention.TimeoutSafeInvoke should invoke the specified function");
        }

        [TestMethod]
        [TestCategory("FunctionExtention.TimeoutSafeInvoke")]
        public void FunctionExtention_TimeoutSafeInvoke_Should_Catch_Up_To_2_Timeouts() {
            StringWriter sw = new StringWriter();
            TraceListener listener = new TextWriterTraceListener(sw);
            Trace.Listeners.Add(listener);
            int magicInt = 1234;
            int tries = 0;
            Func<int> f = () => {
				if (tries++ < 2)
					throw new WebException("The operation has timed out", WebExceptionStatus.Timeout);
				else
					return magicInt;
            };
            Assert.AreEqual(magicInt, f.TimeoutSafeInvoke(), "FunctionExtention.TimeoutSafeInvoke should invoke the specified function");
            Trace.Listeners.Remove(listener);
            listener.Flush();
            sw.Close();
            string trace = sw.ToString();
            Assert.IsTrue(trace.Contains("System.Net.WebException"), "FunctionExtention.TimeoutSafeInvoke should log a WebException to trace log");
        }

        [TestMethod]
        [TestCategory("FunctionExtention.TimeoutSafeInvoke")]
        [ExpectedException(typeof(WebException))]
        public void FunctionExtention_TimeoutSafeInvoke_Should_Raise_TimeoutException_If_Number_Of_Timeouts_More_Then_2() {
            int magicInt = 1234;
            int tries = 0;
            Func<int> f = () => {
				if (tries++ < 3)
					throw new WebException("The operation has timed out", WebExceptionStatus.Timeout);
				else
					return magicInt;
            };
            f.TimeoutSafeInvoke();
        }

        #endregion FunctionExtention.TimeoutSafeInvoke tests

        #region FunctionExtention.CombinePredicates tests
        [TestMethod]
        [TestCategory("FunctionExtention.CombinePredicates")]
        public void FunctionExtention_CombinePredicates_Should_Combine_String_Predicates() {
            var result = FunctionExtentions.CombinePredicates(new Predicate<string>[] {
                    x=> !string.IsNullOrEmpty(x),
                    x=> x.StartsWith("START"),
                    x=> x.EndsWith("END"),
                    x=> x.Contains("#")
                });
            Assert.IsTrue(result("START # END"));
            Assert.IsTrue(result("START######END"));
            Assert.IsTrue(result("START 1111#1111 END"));
            Assert.IsFalse(result("START END"));
            Assert.IsFalse(result(string.Empty));
            Assert.IsFalse(result("START #"));
        }

        [TestMethod]
        [TestCategory("FunctionExtention.CombinePredicates")]
        public void FunctionExtention_CombinePredicates_Should_Combine_Int_Predicates() {
            var result = FunctionExtentions.CombinePredicates(new Predicate<int>[] {
                    x=> x>-10,
                    x=> x<10,
                    x=> x!=0,
                    x=> x!=1
                });
            Assert.IsTrue(result(2));
            Assert.IsTrue(result(-5));
            Assert.IsTrue(result(9));
            Assert.IsFalse(result(-20));
            Assert.IsFalse(result(0));
            Assert.IsFalse(result(1));
        }

        [TestMethod]
        [TestCategory("FunctionExtention.CombinePredicates")]
        public void FunctionExtention_CombinePredicates_Should_Combine_Single_Predicates() {
            var result = FunctionExtentions.CombinePredicates(new Predicate<int>[] {
                    x=> x>0,
                });
            Assert.IsTrue(result(2));
            Assert.IsTrue(result(5));
            Assert.IsTrue(result(1000));
            Assert.IsFalse(result(-20));
            Assert.IsFalse(result(0));
            Assert.IsFalse(result(-1000));
        }

        #endregion FunctionExtention.CombinePredicates tests

    }

}


