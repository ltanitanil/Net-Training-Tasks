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
            return uris.Select(x => new WebClient().DownloadString(x));
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
            var tasks = new Task<String>[maxConcurrentStreams];
            for (int i = 0; i < maxConcurrentStreams; i++)
            {
                if (!uri.MoveNext())
                {
                    tasks = tasks.Where(x => x != null).ToArray();
                    break;
                }
                tasks[i] = new WebClient().DownloadStringTaskAsync(uri.Current);
            }
            while (tasks.Length > 0)
            {
                int indexOfCompletedTask = Task.WaitAny(tasks);
                yield return tasks[indexOfCompletedTask].Result;
                if (uri.MoveNext())
                    tasks[indexOfCompletedTask] = new WebClient().DownloadStringTaskAsync(uri.Current);
                else
                    tasks = tasks.Where((x, i) => i != indexOfCompletedTask).ToArray();
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
