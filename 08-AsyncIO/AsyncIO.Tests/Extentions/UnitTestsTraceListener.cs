using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace AsyncIO.Tests.Extentions
{

    /// <summary>
    /// Idea and source code of TraceListener2 are from 
    /// http://msdn.microsoft.com/en-us/magazine/cc300790.aspx
    /// </summary>
    public abstract class TraceListener2 : TraceListener
    {
        protected TraceListener2(string name) : base(name) { }

        protected abstract void TraceEventCore(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message);

        protected virtual string FormatData(object[] data)
        {
            return string.Join("|", data.Select(x => x.ToString()));
        }

        protected void TraceDataCore(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
        {
            if (Filter != null && !Filter.ShouldTrace(eventCache, source, eventType, id, null, null, null, data))
                return;
            TraceEventCore(eventCache, source, eventType, id, FormatData(data));
        }

        public sealed override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            if (Filter != null && !Filter.ShouldTrace(eventCache, source, eventType, id, message, null, null, null))
                return;
            TraceEventCore(eventCache, source, eventType, id, message);
        }

        public sealed override void Write(string message)
        {
            if (Filter != null && !Filter.ShouldTrace(null, "Trace", TraceEventType.Information, 0, message, null, null, null))
                return;
            TraceEventCore(null, "Trace", TraceEventType.Information, 0, message);
        }

        public sealed override int GetHashCode() { return base.GetHashCode(); }

        public sealed override string Name
        {
            get { return base.Name; }
            set { base.Name = value; }
        }

        #region Other Members
        protected void TraceTransferCore(TraceEventCache eventCache, string source, int id, string message, Guid relatedActivityId)
        {
            base.TraceTransfer(eventCache, source, id, message, relatedActivityId);
        }
        public sealed override void WriteLine(string message)
        {
            Write(message);
        }
        public sealed override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            TraceEventCore(eventCache, source, eventType, id, string.Format(format, args));
        }
        public sealed override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id)
        {
            TraceEventCore(eventCache, source, eventType, id, null); //TODO: is null valid?
        }
        public sealed override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            TraceDataCore(eventCache, source, eventType, id, data);
        }
        public sealed override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
        {
            TraceDataCore(eventCache, source, eventType, id, data);
        }
        public sealed override void TraceTransfer(TraceEventCache eventCache, string source, int id, string message, Guid relatedActivityId)
        {
            TraceTransferCore(eventCache, source, id, message, relatedActivityId);
        }

        public sealed override bool Equals(object obj)
        {
            return this == obj;
        }

        public sealed override object InitializeLifetimeService()              { return base.InitializeLifetimeService(); }
        public sealed override string ToString()                               { return base.ToString(); }
        public sealed override System.Runtime.Remoting.ObjRef CreateObjRef(Type requestedType) { return base.CreateObjRef(requestedType); }
        public sealed override void Write(object o)                            { base.Write(o); }
        public sealed override void Write(object o, string category)           { base.Write(o, category); }
        public sealed override void Write(string message, string category)     { base.Write(message, category); }
        protected sealed override void WriteIndent()                           { base.WriteIndent(); }
        public sealed override void WriteLine(object o)                        { base.WriteLine(o); }
        public sealed override void WriteLine(object o, string category)       { base.WriteLine(o, category); }
        public sealed override void WriteLine(string message, string category) { base.WriteLine(message, category); }
        public sealed override void Fail(string message)                       { base.Fail(message); }
        public sealed override void Fail(string message, string detailMessage) { base.Fail(message, detailMessage); }

        public sealed override void Close() {
            Dispose(true);
        }
        #endregion
    }


    /// <summary>
    /// TraceListener for tracking System.Net trace log
    /// </summary>
    public class UnitTestsTraceListener : TraceListener2
    {

        private static readonly Dictionary<Regex, Action<Match>> stateMachine = new Dictionary<Regex, Action<Match>> { 
            { 
                new Regex(@"\[(?<tid>\d+)\] HttpWebRequest\#(?<id>\d+)\:\:HttpWebRequest\((?<url>\S+)\#(-?)(\d+)\)"),
                m => GetHttpRequest(m.IntValue("id")).Url = m.StringValue("url")
            },
            { 
                new Regex(@"\[(?<tid>\d+)\] Associating HttpWebRequest\#(?<id>\d+) with ConnectStream\#(?<cid>\d+)"),
                m => AddHttpStream(m.IntValue("id"), m.IntValue("cid"))
            },
            { 
                new Regex(@"\[(?<tid>\d+)\] HttpWebRequest\#(?<id>\d+)\:\:BeginGetResponse\(\)"),
                m => GetHttpRequest(m.IntValue("id")).IsAsync = true
            },
            { 
                new Regex(@"\[(?<tid>\d+)\] HttpWebRequest\#(?<id>\d+)\:\:GetResponse\(\)"),
                m => GetHttpRequest(m.IntValue("id")).IsAsync = false
            },
            { 
                new Regex(@"\[(?<tid>\d+)\] ConnectStream\#(?<cid>\d+)\:\:BeginRead\(\)"),
                m => GetHttpStream(m.IntValue("cid")).IsAsync = true
            },
            { 
                new Regex(@"\[(?<tid>\d+)\] ConnectStream\#(?<cid>\d+)\:\:Read\(\)"),
                m => GetHttpStream(m.IntValue("cid")).IsAsync = false
            },
            { 
                new Regex(@"\[(?<tid>\d+)\] ConnectStream\#(?<cid>\d+) - Sending headers"),
                m => StreamClosed(m.IntValue("cid"))
            },
            { 
                new Regex(@"\[(?<tid>\d+)\] ConnectStream\#(?<cid>\d+)\:\:Close\(\)"),
                m => StreamClosed(m.IntValue("cid"))
            },
            { 
                new Regex(@"\[(?<tid>\d+)\] HttpWebRequest\#(?<id>\d+)\:\:\(\) - Error code"),
                m => CloseAllStreams(m.IntValue("id"))
            }

        };

        private static readonly ConcurrentDictionary<int, HttpRequesDetail> requestsIndex = new ConcurrentDictionary<int, HttpRequesDetail>();
        private static readonly ConcurrentDictionary<int, StreamDetail> streamIndex = new ConcurrentDictionary<int, StreamDetail>();
        private static volatile int concurrentStreamsCount = 0;
        private static object lockObject = new Object();

        public UnitTestsTraceListener() : base("UnitTestTraceListener") { }

        protected override void TraceEventCore(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            if (!IsActive) return;

            foreach (var state in stateMachine)
            {
                var match = state.Key.Match(message);
                if (!match.Success) continue;

                state.Value(match);
                break;
            }
        }

        private static HttpRequesDetail GetHttpRequest(int id)
        {
            return requestsIndex.GetOrAdd(id, (x) => new HttpRequesDetail() { Id = id, Streams = new List<StreamDetail>() });
        }

        private static StreamDetail GetHttpStream(int id)
        {
            return streamIndex[id];
        }

        private static void AddHttpStream(int id, int streamId)
        {
            var result = streamIndex.GetOrAdd(streamId, 
                (x) => { 
                         StreamsCountChanged(+1); 
                         return new StreamDetail() {Id = streamId, IsClosed = false};
                        });

            var request = GetHttpRequest(id);
            request.Streams.Add(result);
        }

        private static void CloseAllStreams(int id)
        {
            var request = GetHttpRequest(id);
            lock (lockObject)
            {
                foreach (var stream in request.Streams.Where(x=>!x.IsClosed))
                {
                    stream.IsClosed = true;
                    concurrentStreamsCount--;
                }
            }
        }

        private static void StreamClosed(int id)
        {
            var stream = GetHttpStream(id);
            if (!stream.IsClosed)
            {
                stream.IsClosed = true;
                StreamsCountChanged(-1);
            }
        }

        private static void StreamsCountChanged(int increment)
        {
            lock (lockObject)
            {
                concurrentStreamsCount += increment;
                if (concurrentStreamsCount > MaxConcurrentStreamsCount)
                {
                    MaxConcurrentStreamsCount = concurrentStreamsCount;
                }
            }
        }

        private static void ClearData()
        {
            requestsIndex.Clear();
            streamIndex.Clear();
            concurrentStreamsCount = 0;
        }

        private static bool isActive = false;
        public static bool IsActive
        {
            set { isActive = true; ClearData(); }
            get { return isActive; }
        }

        public static int MaxConcurrentStreamsCount { private set; get; }

        public static HttpRequesDetail GetRequest(string uri)
        {
            return requestsIndex.FirstOrDefault(x => x.Value.Url.Equals(uri, StringComparison.InvariantCultureIgnoreCase)).Value;
        }
    }

    public class HttpRequesDetail
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public bool? IsAsync { get; set; }
        public List<StreamDetail> Streams { get; set; }

        public bool IsStreamAsync {
            get { return Streams.All(x => x.IsAsync != false); }
        }
    }

    public class StreamDetail
    {
        public int Id { get; set; }
        public bool? IsAsync { get; set; }
        public bool IsClosed { get; set; }
    }

    public static class MatchExtentions
    {
        public static int IntValue(this Match match, string group)
        {
            return int.Parse(match.StringValue(group));
        }

        public static string StringValue(this Match match, string group)
        {
            return match.Groups[group].Value;
        }
    }

}
