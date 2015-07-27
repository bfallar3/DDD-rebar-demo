using System;
using System.Collections;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

namespace Rebar.Web.Infrastructure.Filters
{
    /// <summary>Applies global exception handling policy to unhandled exceptions.</summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class GlobalExceptionHandlingFilterAttribute : FilterAttribute, IExceptionFilter
    {
        /// <summary>
        /// Sets the filterContext result based on the HttpException
        /// </summary>
        /// <param name="filterContext">Exception Context of the Exception</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)", Justification = "Reviewed. Supression ok here"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Reviewed. Supression ok here")]
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext == null || filterContext.HttpContext == null)
            {
                return;
            }

            var exception = HandleException(filterContext.Exception, new Guid());

            var exceptionManager = new ExceptionPolicyFactory().CreateManager();
            exceptionManager.HandleException(exception, "Global Exception Policy");
            
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
            var httpException = filterContext.Exception as HttpException;
            if (httpException == null)
            {
                filterContext.ExceptionHandled = true;
                return;
            }

            filterContext.Result = new ViewResult { ViewName = string.Format("Http{0}", new Exception("HTTP error", httpException)) };
            filterContext.ExceptionHandled = true;
        }

        /*
         * All the code below here is ported over from ACA.NET.
         * It adds additional contextual information to the exception.
         */

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "This is a global error handler so we want to catch everything.")]
        private static Exception HandleException(Exception exception, Guid handlingInstanceId)
        {
            try
            {
                Contextualize(exception.Data, handlingInstanceId);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            return exception;
        }

        private static void Contextualize(IDictionary exceptionContext, Guid handlingInstanceId)
        {
            AddInstanceId(exceptionContext, handlingInstanceId);
            AddUserContext(exceptionContext);
            AddRequestContext(exceptionContext);
            AddSessionContext(exceptionContext);
        }

        private static void AddInstanceId(IDictionary exceptionContext, Guid handlingInstanceId)
        {
            exceptionContext.Add("Exception.InstanceId", handlingInstanceId.ToString());
        }

        private static void AddRequestContext(IDictionary exceptionContext)
        {
            if (HttpContext.Current == null) return;
            if (exceptionContext == null) return;

            var request = HttpContext.Current.Request;
            
            foreach (string index in request.Params)
            {
                exceptionContext.Add("Request." + index, request.Params[index]);
            }

            exceptionContext.Add("Request.IsLocal", Convert.ToBoolean(request.IsLocal ? 1 : 0));
            exceptionContext.Add("Request.IsSecureConnection", Convert.ToBoolean(request.IsSecureConnection ? 1 : 0));
            exceptionContext.Add("Request.RawUrl", request.RawUrl);
            exceptionContext.Add("Request.RequestType", request.RequestType);
            exceptionContext.Add("Request.UrlReferrer", request.UrlReferrer == null ? "Null" : request.UrlReferrer.ToString());
        }

        private static void AddSessionContext(IDictionary exceptionContext)
        {
            if (HttpContext.Current == null) return;
            if (exceptionContext == null) return;

            var session = HttpContext.Current.Session;

            if (session == null)
            {
                exceptionContext.Add("Session", "Null");
            }
            else
            {
                exceptionContext.Add("Session.IsNewSession", Convert.ToBoolean(session.IsNewSession ? 1 : 0));
                exceptionContext.Add("Session.SessionID", session.SessionID);
                foreach (string index in session.Keys)
                    exceptionContext.Add("Session." + index, session[index].ToString());
            }
        }

        private static void AddUserContext(IDictionary exceptionContext)
        {
            AddPrincipalContext(exceptionContext, Thread.CurrentPrincipal, "Thread");
        }

        private static void AddPrincipalContext(IDictionary exceptionContext, IPrincipal principal, string prefix)
        {
            if (exceptionContext == null)
                throw new ArgumentNullException("exceptionContext");

            if (prefix == null)
                prefix = string.Empty;
            
            if (principal != null)
            {
                exceptionContext.Add(prefix + ".Principal.TypeName", principal.GetType().FullName);
                if (principal.Identity == null)
                {
                    exceptionContext.Add((object)prefix + ".Principal.Identity", (object)"Null");
                }
                else
                {
                    IIdentity identity = principal.Identity;
                    exceptionContext.Add(prefix + ".Principal.Identity.TypeName", identity.GetType().FullName);
                    exceptionContext.Add(prefix + ".Principal.Identity.AuthType", identity.AuthenticationType);
                    exceptionContext.Add(prefix + ".Principal.Identity.IsAuthenticated", Convert.ToBoolean(identity.IsAuthenticated ? 1 : 0));
                    exceptionContext.Add(prefix + ".Principal.Identity.Name", identity.Name);
                }
            }
            else
                exceptionContext.Add(prefix + ".Principal", "Null");
        }
    }
}