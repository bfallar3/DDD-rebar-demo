using System;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Rebar.Web
{
    /// <summary>
    /// Class AuthenticationHandler
    /// </summary>
    public class EmptyAuthenticationHandler : DelegatingHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmptyAuthenticationHandler"/> class.
        /// </summary>
        public EmptyAuthenticationHandler()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmptyAuthenticationHandler" /> class with a specific inner handler.
        /// </summary>
        /// <param name="innerHandler">The inner handler which is responsible for processing the HTTP response messages.</param>
        public EmptyAuthenticationHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
        }

        /// <summary>
        /// Ignores Authentication
        /// </summary>
        /// <param name="request">The HTTP request message to send to the server.</param>
        /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />. The task object representing the asynchronous operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The framework disposes of this")]
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.GetRequestContext().Principal = new GenericPrincipal(new GenericIdentity("LocalDevUser"), new[] { "User" });
            if (HttpContext.Current != null)
            {
                HttpContext.Current.User = request.GetRequestContext().Principal;
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}