using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

namespace Rebar.Service.ExceptionHandling
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Delegate)]
    public sealed class ExceptionHandlingAttribute : ExceptionFilterAttribute
    {
        private static readonly Lazy<ExceptionManager> ExceptionManager =
            new Lazy<ExceptionManager>(() => new ExceptionPolicyFactory().CreateManager());

        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext == null) return;

            string msg = actionExecutedContext.Exception.Message;
            Exception ex;
            if (actionExecutedContext == null) throw new ArgumentNullException("actionExecutedContext");
            ExceptionManager.Value.HandleException(actionExecutedContext.Exception, "Global Exception Policy", out ex);

            actionExecutedContext.Response = actionExecutedContext.Request.CreateErrorResponse((HttpStatusCode)int.Parse(ex.Message, CultureInfo.InvariantCulture), msg);
        }
    }
}