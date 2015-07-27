using System.Diagnostics;

namespace Rebar.Tracing
{
    public static class LevelToSeverityMapperExtension
    {
        public static TraceEventType GetSeverity(this System.Web.Http.Tracing.TraceLevel level)
        {
            switch (level)
            {
                case System.Web.Http.Tracing.TraceLevel.Off:
                    return TraceEventType.Suspend;

                case System.Web.Http.Tracing.TraceLevel.Fatal:
                    return TraceEventType.Critical;

                case System.Web.Http.Tracing.TraceLevel.Error:
                    return TraceEventType.Error;

                case System.Web.Http.Tracing.TraceLevel.Debug:
                    return TraceEventType.Verbose;

                case System.Web.Http.Tracing.TraceLevel.Info:
                    return TraceEventType.Information;

                case System.Web.Http.Tracing.TraceLevel.Warn:
                    return TraceEventType.Warning;

                default:
                    return TraceEventType.Warning;
            }
        }
    }
}