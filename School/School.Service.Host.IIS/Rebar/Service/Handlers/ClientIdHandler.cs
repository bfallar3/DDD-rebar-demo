using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Rebar.Telemetry;

namespace Rebar.Service.Handlers
{
    /// <summary>
    /// Class ClientIdHandler
    /// </summary>
    public class ClientIdHandler : DelegatingHandler
    {
        internal const string ClientIdHeaderName = "x-acc-clientid";
        internal const string LegacyClientIdHeaderName = "ClientId";
        internal const string ClientIdRequiredAppSetting = "Rebar.Server.RequireClientId";
        internal const string AllowLegacyClientIdAppSetting = "Rebar.Server.AllowLegacyClientId";
        internal const string TelemetryClientIdProperty = "client_id";

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientIdHandler"/> class.
        /// </summary>
        public ClientIdHandler()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientIdHandler" /> class with a specific inner handler.
        /// </summary>
        /// <param name="innerHandler">The inner handler which is responsible for processing the HTTP response messages.</param>
        public ClientIdHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
        }

        /// <summary>
        /// Sends an HTTP request to the inner handler to send to the server as an asynchronous operation.
        /// </summary>
        /// <param name="request">The HTTP request message to send to the server.</param>
        /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />. The task object representing the asynchronous operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The framework disposes of this")]
        protected override System.Threading.Tasks.Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            IEnumerable<string> clientId;

            bool hasClientId = request.Headers.TryGetValues(ClientIdHeaderName, out clientId);
            if (!hasClientId && ConfigurationManager.AppSettings[AllowLegacyClientIdAppSetting].IsEnabled())
            {
                hasClientId = request.Headers.TryGetValues(LegacyClientIdHeaderName, out clientId);
            }

            if (hasClientId)
            {
                TelemetryManager.Current.CurrentStep.Items[TelemetryClientIdProperty] = clientId.First();
            }

            if (ConfigurationManager.AppSettings[ClientIdRequiredAppSetting].IsEnabled() && !hasClientId)
            {
                var tcs = new TaskCompletionSource<HttpResponseMessage>(TaskCreationOptions.None);
                var response = request.CreateResponse(HttpStatusCode.BadRequest, "This service requires a valid x-acc-clientid header value which is missing from the request.");
                tcs.SetResult(response);
                return tcs.Task;
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}