using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reflection.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reflection.Tests
{
    [TestClass]
    public class CodeGenerationTests {

        [TestMethod]
        [TestCategory("Code Generation")]
        public void GetVectorMultiplyFunction_Returns_Function_For_Int() {
            var first = new int[] { 1, 2, 3 };
            var second = new int[] { 2, 2, 2 };
            var expected = 1*2 + 2*2 + 3*2;

            var func = CodeGeneration.GetVectorMultiplyFunction<int>();
            var actual = func(first, second);
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        [TestCategory("Code Generation")]
        public void GetVectorMultiplyFunction_Returns_Function_For_Long() {
            var first = new long[] { 1L, 2L, 3L };
            var second = new long[] { 2L, 2L, 2L };
            var expected = 1L * 2L + 2L * 2L + 3L * 2L;

            var func = CodeGeneration.GetVectorMultiplyFunction<long>();
            var actual = func(first, second);
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        [TestCategory("Code Generation")]
        public void GetVectorMultiplyFunction_Returns_Function_For_Double() {
            var first = new double[] { 1D, 2D, 3D };
            var second = new double[] { 2D, 2D, 2D };
            var expected = 1D * 2D + 2D * 2D + 3D * 2D;

            var func = CodeGeneration.GetVectorMultiplyFunction<double>();
            var actual = func(first, second);
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        [TestCategory("Code Generation Performance")]
        public void CodeGeneration_PerformanceTest() {
            var sw = new Stopwatch();
            sw.Start();
            var func = CodeGeneration.GetVectorMultiplyFunction<int>();
            sw.Stop();

            Console.WriteLine("Generating & Compiling method time : {0} ms ({1} ticks)", sw.ElapsedMilliseconds, sw.ElapsedTicks);

            const int TrialCount = 10000;
            var first = Enumerable.Range(0, 100).ToArray();
            var second = Enumerable.Range(0, 100).ToArray();

            // Cold start for JIT-compiling 
            func(first, second);
            CodeGeneration.MultuplyVectors(first, second);

            sw.Reset();
            sw.Start();
            for (int i = 0; i < TrialCount; i++)
                func(first, second);
            sw.Stop();
            Console.WriteLine("Generated code : {0} ms ({1} ticks)", sw.ElapsedMilliseconds, sw.ElapsedTicks);


            sw.Reset();
            sw.Start();
            for (int i = 0; i < TrialCount; i++)
                CodeGeneration.MultuplyVectors(first, second);
            sw.Stop();
            Console.WriteLine("Static code   : {0} ms ({1} ticks)", sw.ElapsedMilliseconds, sw.ElapsedTicks);
        }


    }
}
