using Microsoft.Diagnostics.Tracing;
using Rebar.Telemetry;
using System;
using System.Collections.Generic;
using System.Text;

[assembly: WebActivatorEx.PreApplicationStartMethod(
    typeof(SchoolFront.Web.ETW.Rebar.ETWLogging.RebarEventSource), "PreStart")]

namespace SchoolFront.Web.ETW.Rebar.ETWLogging
{
    [EventSource(Name = "Accenture-Rebar-EventSource")] //"{CompanyName}-{AppName}-{Component}?
    public sealed class RebarEventSource : EventSource
    {
        public static readonly RebarEventSource Log = new RebarEventSource();
        
        // Use keywords to filter events
        public class Keywords
        {           
            public const EventKeywords _trace = (EventKeywords)1;
            public const EventKeywords _failure = (EventKeywords)2;
            public const EventKeywords _archTrace = (EventKeywords)4;
            public const EventKeywords _server_requestTelemetry = (EventKeywords)8;
            public const EventKeywords _db_CommandTelemetry = (EventKeywords)16;
        }
                
        private const int ServiceRequestTraceId = 1;
        private const int ArchEventId = 2;
        private const int FailureId = 3;
        private const int server_requestTelemetryId = 4;
        private const int db_CommandTelemetryId = 5;

        //Event to log server request telemetry
        [Event(server_requestTelemetryId, Version = 1, Keywords = Keywords._server_requestTelemetry, Level = EventLevel.Informational)]
        public void ServerRequestTelemetry(string request_url, string client_id, DateTime start_time, string result, string duration, DateTime end_time )
        {
            if (IsEnabled()) WriteEvent(server_requestTelemetryId, request_url, client_id, start_time, result, duration, end_time); 
        }

        //Event to log db command telemetry
        [Event(db_CommandTelemetryId, Version = 1, Keywords = Keywords._db_CommandTelemetry, Level = EventLevel.Informational)]
        public void DbCommandTelemetry(string request_url, string client_id, DateTime start_time, string db_statement_id, string db_statement_type, string result, string duration, DateTime end_time, int db_record_count)
        {
            if (IsEnabled()) WriteEvent(db_CommandTelemetryId, request_url, client_id, start_time, db_statement_id, db_statement_type, result, duration, end_time, db_record_count);
        }
    
        [Event(ServiceRequestTraceId, Version = 1, Keywords = Keywords._trace, Level = EventLevel.Informational)]
        public void ServiceRequestTrace(string category, string level, string message, string operation, string Operator, string request, string requestMethod, string requestUri, string kind, string exception, string severity)
        {
            if (IsEnabled()) WriteEvent(ServiceRequestTraceId, category, level, message, operation, Operator, request, requestMethod, requestUri, kind, exception, severity);
        }

        //Event to log exception/fauilure
        [Event(FailureId, Version = 1, Message = "Failure Message: {3}", Channel = EventChannel.Admin, Keywords = Keywords._failure, Level = EventLevel.Error)]
        public void Failure(string HelpLink, int HResult, string InnerException, string Message, string Source, string StackTrace, string TargetSite)
        {
            if (IsEnabled()) WriteEvent(FailureId, HelpLink, HResult, InnerException, Message, Source, StackTrace, TargetSite);
        }
        
        [Event(ArchEventId, Version = 1, Keywords = Keywords._archTrace, Level = EventLevel.Informational)]
        public void ArchTraceLog(string message)
        {
            if (IsEnabled()) WriteEvent(ArchEventId, message);
        }

		[NonEvent]
        public static void ETWTelemetryEvent(IDictionary<string, object> _event)
        {
            string eventsource = _event["name"].ToString();
            switch (eventsource)
            {
                case "db_command":
                    using (new EventContextScope())
                    {
                        if (!(_event.ContainsKey("db_record_count")))
                        { _event["db_record_count"] = 0; }

                        Log.DbCommandTelemetry(_event["request_url"].ToString(), (string)_event["client_id"],
                            ((DateTime)_event["start_time"]), _event["db_statement_id"].ToString(),
                            (string)_event["db_statement_type"], (string)_event["result"], _event["duration"].ToString(),
                            (DateTime)_event["end_time"], (int)_event["db_record_count"]);                        
                        
                    }
                    break;
                case "server_request":
                    using (new EventContextScope())
                    {
                        Log.ServerRequestTelemetry(_event["request_url"].ToString(), (string)_event["client_id"],
                            (DateTime)_event["start_time"], (string)_event["result"],
                            _event["duration"].ToString(), (DateTime)_event["end_time"]);
                    }
                    break;
                default:
                    break;
            }
        }

		[NonEvent]
        public static void PreStart()
        {
            TelemetryManager.SetLogger(x => RebarEventSource.ETWTelemetryEvent(x)); 
        }
    }
}
