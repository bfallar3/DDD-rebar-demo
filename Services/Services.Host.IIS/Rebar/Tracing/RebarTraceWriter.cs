using System;
using System.Collections.Generic;
using System.Web.Http.Tracing;

using Microsoft.Practices.EnterpriseLibrary.Logging;
using Services.Service.ETW.Rebar.ETWLogging;

namespace Rebar.Tracing
{
    public class RebarTraceWriter : SystemDiagnosticsTraceWriter
    {
        private static RebarEventSource _logger = RebarEventSource.Log;
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

            using (new EventContextScope())
            {
                _logger.ServiceRequestTrace(record.Category ?? string.Empty, Convert.ToString(record.Level) ?? string.Empty, record.Message ?? string.Empty,
                                            record.Operation ?? string.Empty, record.Operator ?? string.Empty,
                                            Convert.ToString(record.Request) ?? string.Empty, Convert.ToString(record.Request.Method) ?? string.Empty, Convert.ToString(record.Request.RequestUri) ?? string.Empty,
                                            Convert.ToString(record.Kind) ?? string.Empty, record.Exception != null
                                                    ? record.Exception.GetBaseException()
                                                            .Message
                                                    : !string.IsNullOrEmpty(
                                                        record.Message)
                                                          ? record.Message
                                                          : string.Empty,
                                            Convert.ToString(record.Level.GetSeverity()) ?? string.Empty);
            }
        }            
    }
}