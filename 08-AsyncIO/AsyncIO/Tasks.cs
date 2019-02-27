using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncIO
{
    public static class Tasks
    {
        /// <summary>
        /// Returns the content of required uris.
        /// Method has to use the synchronous way and can be used to compare the performace of sync \ async approaches. 
        /// </summary>
        /// <param name="uris">Sequence of required uri</param>
        /// <returns>The sequence of downloaded url content</returns>
        public static IEnumerable<string> GetUrlContent(this IEnumerable<Uri> uris) 
        {
            return uris.Select(x =>
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(x);
                request.AutomaticDecompression = DecompressionMethods.GZip;

                using (Stream receiveStream = request.GetResponse().GetResponseStream())
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    return readStream.ReadToEnd();
            });
        }



        /// <summary>
        /// Returns the content of required uris.
        /// Method has to use the asynchronous way and can be used to compare the performace of sync \ async approaches. 
        /// 
        /// maxConcurrentStreams parameter should control the maximum of concurrent streams that are running at the same time (throttling). 
        /// </summary>
        /// <param name="uris">Sequence of required uri</param>
        /// <param name="maxConcurrentStreams">Max count of concurrent request streams</param>
        /// <returns>The sequence of downloaded url content</returns>
        public static IEnumerable<string> GetUrlContentAsync(this IEnumerable<Uri> uris, int maxConcurrentStreams)
        {
            var uri = uris.GetEnumerator();
            var task = new List<Task<String>>();
            do
            {
                if (maxConcurrentStreams > 0 && uri.MoveNext())
                {
                    var current = uri.Current;
                    task.Add(Task.Factory.StartNew(() => GetWebResponse(current)));
                    maxConcurrentStreams--;
                }
                else
                {
                    int indexOfCompletedTask = Task.WaitAny(task.ToArray());
                    yield return task[indexOfCompletedTask].Result;
                    task.RemoveAt(indexOfCompletedTask);
                    if (uri.MoveNext())
                    {
                        var current = uri.Current;
                        task.Add(Task.Factory.StartNew(() => GetWebResponse(current)));
                    }
                }
            }
            while (task.Any());
        }

        private static string GetWebResponse(Uri uri)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip;
            using (WebResponse webResp = request.GetResponseAsync().Result)
            {
                using (Stream receiveStream = webResp.GetResponseStream())
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    return readStream.ReadToEnd();
            }
        }


        /// <summary>
        /// Calculates MD5 hash of required resource.
        /// 
        /// Method has to run asynchronous. 
        /// Resource can be any of type: http page, ftp file or local file.
        /// </summary>
        /// <param name="resource">Uri of resource</param>
        /// <returns>MD5 hash</returns>
        public async static Task<string> GetMD5Async(this Uri resource)
        {
            return string.Join("", await new WebClient().DownloadDataTaskAsync(resource)
                .ContinueWith(x => MD5.Create().ComputeHash(x.Result).Select(y => y.ToString("x2"))));
        }

    }



}
