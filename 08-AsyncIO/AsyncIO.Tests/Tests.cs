using System;
using System.Linq;
using AsyncIO.Tests.Extentions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Collections.Generic;

namespace AsyncIO.Tests
{
    [TestClass]
    public class Tests
    {
        private string[] sites = { 
           "google", "msdn",  "facebook", "linkedin", "twitter",
           "bing",   "yahoo", "youtube",  "baidu",    "amazon"
        };


        private IEnumerable<Uri> GetTestUris() {
            return sites.Select(x => new Uri(string.Format(@"http://{0}.com", x)));
        }

        [TestMethod]
        [TestCategory("GetUrlContent")]
        public void GetUrlContent_Should_Return_Content()
        {
            TestContent(AsyncIO.Tasks.GetUrlContent);
        }


        [TestMethod]
        [TestCategory("GetUrlContent")]
        public void GetUrlContent_Should_Be_Synchronous()
        {
            Action<Uri> action = (uri) => (new[] { uri }).GetUrlContent().ToArray();
            Check_Is_Action_Asynchronous(action, false);
        }


        [TestMethod]
        [TestCategory("GetUrlContentAsync")]
        public void GetUrlContentAsync_Should_Return_Content()
        {
            TestContent(x => x.GetUrlContentAsync(3));
        }

        [TestMethod]
        [TestCategory("GetUrlContentAsync")]
        public void GetUrlContentAsync_Should_Run_Expected_Count_Of_Concurrent_Streams()
        {
            foreach(var expectedConcurrentStreams in new int[] { 3, 6 }) {
                UnitTestsTraceListener.IsActive = true;
                try
                {
                    GetTestUris().GetUrlContentAsync(expectedConcurrentStreams).ToArray();

                    Assert.IsTrue(UnitTestsTraceListener.MaxConcurrentStreamsCount <= expectedConcurrentStreams,
                                  string.Format("Max concurrent streams should be less then {0}, actual : {1}",
                                                expectedConcurrentStreams,
                                                UnitTestsTraceListener.MaxConcurrentStreamsCount));

                    Assert.IsTrue(UnitTestsTraceListener.MaxConcurrentStreamsCount > 1,
                                   string.Format("Max concurrent streams should be more then 1, actual : {0}",
                                                 UnitTestsTraceListener.MaxConcurrentStreamsCount));

                    Trace.WriteLine(string.Format("Actual max concurrent requests (max {0}): {1}",
                                                  expectedConcurrentStreams,
                                                  UnitTestsTraceListener.MaxConcurrentStreamsCount));
                } finally {
                    UnitTestsTraceListener.IsActive = false;
                }
            }
        }


        [TestMethod]
        [TestCategory("GetUrlContentAsync")]
        public void GetUrlContentAsync_Should_Run_Asynchronous()
        {
            Action<Uri> action = (uri) => (new[] {uri}).GetUrlContentAsync(2).ToArray();
            Check_Is_Action_Asynchronous(action, true);
        }




        [TestMethod]
        [TestCategory("GetUrlMD5")]
        public void GetUrlMD5_Should_Return_CorrectValue()
        {
            var actual = new Uri(@"ftp://ftp.byfly.by/test/100kb.txt").GetMD5Async().Result;
            Assert.AreEqual("869c2d2bacc13741416c6303a3a92282", actual, true);
        }


        [TestMethod]
        [TestCategory("GetUrlMD5")]
        public void GetUrlMD5_Should_Run_Asynchronous()
        {
            Check_Is_Action_Asynchronous(GetMD5Wrapper, true);
        }

        // Wrapper to allow using async as well as no async signature for GetMD5Async mrthod
        private void GetMD5Wrapper(Uri uri)
        {
            var result = uri.GetMD5Async().Result;
        }


        #region Private Methods 

        private void TestContent(Func<IEnumerable<Uri>, IEnumerable<string>> func)
        {
            var sw = new Stopwatch();
            sw.Start();
            var actual = func(GetTestUris()).ToArray();
            sw.Stop();
            Trace.WriteLine("Time : " + sw.Elapsed.ToString());
            Assert.IsTrue(actual
                .Zip(sites, (content, site) => content.IndexOf(site, StringComparison.InvariantCultureIgnoreCase) > 0)
                .All(x=>x));
        }

        private void Check_Is_Action_Asynchronous(Action<Uri> action, bool shouldbeAsync)
        {
            UnitTestsTraceListener.IsActive = true;
            try {
                const string uri = "http://www.msdn.com/";
                
                action(new Uri(uri));

                var actual = UnitTestsTraceListener.GetRequest(uri);

                Assert.IsTrue(actual.IsAsync == shouldbeAsync,       "Request should be {0}!",             shouldbeAsync ? "asynchronous" : "synchronous");
                Assert.IsTrue(actual.IsStreamAsync == shouldbeAsync, "Downloading streams should be {0}!", shouldbeAsync ? "asynchronous" : "synchronous");

            } finally {
                UnitTestsTraceListener.IsActive = false;
            }
        }


        #endregion Private Methods

    }
}
