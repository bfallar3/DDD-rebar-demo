using Newtonsoft.Json;

namespace Accenture.Rebar.Web
{
    /// <summary>
    /// POCO class for JwtToken
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Jwt", Justification = "Reviewed.")]
    public class JwtToken
    {
        /// <summary>
        /// Gets or sets the value for the token issuer
        /// </summary>
        [JsonProperty(PropertyName = "Iss")]
        public string Issuer { get; set; }

        /// <summary>
        /// Gets or sets Token Audience
        /// </summary>
        [JsonProperty(PropertyName = "Aud")]
        public string Audience { get; set; }

        /// <summary>
        /// Gets or sets the value for Not Before
        /// </summary>
        [JsonProperty(PropertyName = "Nbf")]
        public string NotBefore { get; set; }

        /// <summary>
        /// Gets or sets the Expiration date of the token
        /// </summary>
        [JsonProperty(PropertyName = "Exp")]
        public long ExpirationTime { get; set; }
    }
}