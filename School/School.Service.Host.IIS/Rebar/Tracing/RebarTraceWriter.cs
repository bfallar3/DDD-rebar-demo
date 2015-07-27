using System;
using System.Collections.Generic;
using System.Web.Http.Tracing;

using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace Rebar.Tracing
{
    public class RebarTraceWriter : SystemDiagnosticsTraceWriter
    {
        public override void Trace(System.Net.Http.HttpRequestMessage request, string category, TraceLevel level, Action<TraceRecord> traceAction)
        {
            if (traceAction == null) throw new ArgumentNullException("traceAction");

            var record = new TraceRecord(request, category, level);
            traceAction(record);
            WriteTrace(record);
        }

        private static void WriteTrace(TraceRecord record)
        {
            if (record == null) return;
            if (record.Request == null) return;
            if (record.Request.RequestUri == null) return;

            var entry = new LogEntry
                            {
                                Categories = new List<string> { "Trace" },
                                TimeStamp = record.Timestamp,
                                ExtendedProperties =
                                    new Dictionary<string, object>
                                        {
                                            { "Category", record.Category ?? string.Empty },
                                            { "Level", record.Level },
                                            { "Message", record.Message ?? string.Empty },
                                            { "Operation", record.Operation ?? string.Empty },
                                            { "Operator", record.Operator ?? string.Empty },
                                            { "Properties", record.Properties ?? new Dictionary<object, object>() },
                                            { "Request", record.Request },
                                            { "Request.Method", record.Request.Method },
                                            {
                                                "RequestURI",
                                                record.Request.RequestUri.ToString()
                                            },
                                            { "Kind", record.Kind },
                                            {
                                                "Exception",
                                                record.Exception != null
                                                    ? record.Exception.GetBaseException()
                                                            .Message
                                                    : !string.IsNullOrEmpty(
                                                        record.Message)
                                                          ? record.Message
                                                          : string.Empty
                                            }
                                        },
                                Severity = record.Level.GetSeverity()
                            };

            Logger.Write(entry);
        }
    }
}