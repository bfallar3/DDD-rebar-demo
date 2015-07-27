using System;

namespace Accenture.Rebar.Web
{
    /// <summary>
    /// REBAR wrapper for the jwt token
    /// </summary>
    public class RebarToken
    {
        /// <summary>
        /// Gets or sets the Expiration date
        /// </summary>
        public DateTime TokenExpiration { get; set; }

        /// <summary>
        /// Gets or sets the jwt token
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Jwt", Justification = "Reviewed.")]
        public string JwtToken { get; set; }

        /// <summary>
        /// Gets a value indicating whether the expiration date has passed
        /// </summary>
        public bool IsExpired
        {
            get
            {
                return this.TokenExpiration < DateTime.Now;
            }
        }
    }
}