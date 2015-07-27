using System;
using System.Diagnostics;
using System.Web;

namespace Services.Service.ETW.Rebar.ETWLogging
{
    /// <summary>
    /// Sets ActivityID for current ETW event
    /// </summary>
    public class EventContextScope: IDisposable
    {
        private bool disposed = false;        
        private readonly Guid prevActivityId;

        public EventContextScope()
        {
            Guid prevActivityId;
            Guid newActivityId = SetActivityId();
            Trace.CorrelationManager.ActivityId = newActivityId;
            RebarEventSource.SetCurrentThreadActivityId(newActivityId, out prevActivityId);
        }

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                RebarEventSource.SetCurrentThreadActivityId(prevActivityId);
                Trace.CorrelationManager.ActivityId = prevActivityId;                
            }
        }

        public Guid SetActivityId()
        {
			try
			{
				var serviceProvider = (IServiceProvider)HttpContext.Current;
				var workerRequest = (HttpWorkerRequest)serviceProvider.GetService(typeof(HttpWorkerRequest));
				var traceId = workerRequest.RequestTraceIdentifier;
				if (traceId == Guid.Empty)
				{
					traceId = Guid.NewGuid();
				}
				return traceId;
			}
			catch
			{ return Guid.NewGuid();}
        }
    }
}