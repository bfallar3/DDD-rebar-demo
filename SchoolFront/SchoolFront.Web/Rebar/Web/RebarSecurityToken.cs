using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using Accenture.Security.Eso.Token;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Accenture.Rebar.Web
{
    /// <summary>
    /// REBAR management of the ESO jwt token
    /// </summary>
    public static class RebarSecurityToken
    {
        private static readonly Dictionary<JwtHashAlgorithm, Func<byte[], byte[], byte[]>> HashAlgorithms = new Dictionary<JwtHashAlgorithm, Func<byte[], byte[], byte[]>>();

        private enum JwtHashAlgorithm
        {
            RS256,
            HS384,
            HS512
        }

        /// <summary>
        /// Gets a token from the appropriate issuer based on the config
        /// </summary>
        /// <param name="sid">The tag id for the config. This is used to find appropriate config section using "accenture.security.eso.token." + sid.</param>
        /// <param name="name">Service Identifier</param>
        /// <returns>A token as a string</returns>
        public static string GetToken(string sid, string name)
        {
            var uniqueId = sid + name;

            var existingToken = HttpRuntime.Cache.Get(uniqueId) as RebarToken;

            if (existingToken != null && !existingToken.IsExpired) return existingToken.JwtToken;

            Task<RebarToken> tokenTask = Task.Run<RebarToken>(() => GetESOToken(sid, name));

            Task.WhenAll(new Task<RebarToken>[] { tokenTask }).Wait();

            var token = tokenTask.Result;
            HttpRuntime.Cache.Insert(uniqueId, token, null, token.TokenExpiration, TimeSpan.Zero);

            return token.JwtToken;
        }

        private static async Task<RebarToken> GetESOToken(string sid, string name)
        {
            var uniqueid = sid + name;
            var configuration = ConfigurationManager.GetSection("accenture.security.eso.token." + sid) as NameValueCollection;
            var jwtEndpoint = configuration["Services:Endpoints:Jwt"];
            var identifer = configuration["Services:Identifier"];
            
            var esoTokenInvoker = new Services(jwtEndpoint, identifer);
          
            var bootstrapCtx = ClaimsPrincipal.Current.Identities.First().BootstrapContext as BootstrapContext;
            
            string jwt;

            if (bootstrapCtx.SecurityToken != null)
            {
                jwt = await esoTokenInvoker.SamlToJwtAsync(bootstrapCtx.SecurityToken, uniqueid);
            }
            else
            {
                jwt = await esoTokenInvoker.SamlToJwtAsync(bootstrapCtx.Token, uniqueid);
            }

            // Need to figure out how to get jwt token expiration 
            return new RebarToken { JwtToken = jwt, TokenExpiration = GetTokenExpirationDate(jwt) };
        }

        private static DateTime GetTokenExpirationDate(string token)
        {
            var jsonToken = Decode(token, string.Empty, false);
            var data = JsonConvert.DeserializeObject<JwtToken>(jsonToken);
            return FromUnixTime(data.ExpirationTime);
        }

        private static DateTime FromUnixTime(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)", Justification = "Reviewed.")]
        private static string Decode(string token, string key, bool verify)
        {
            var parts = token.Split('.');
            var header = parts[0];
            var payload = parts[1];
            byte[] crypto = Base64UrlDecode(parts[2]);

            var headerJson = Encoding.UTF8.GetString(Base64UrlDecode(header));
            var headerData = JObject.Parse(headerJson);
            var payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(payload));
            var payloadData = JObject.Parse(payloadJson);

            if (verify)
            {
                var bytesToSign = Encoding.UTF8.GetBytes(string.Concat(header, ".", payload));
                var keyBytes = Encoding.UTF8.GetBytes(key);
                var algorithm = (string)headerData["alg"];

                var signature = HashAlgorithms[GetHashAlgorithm(algorithm)](keyBytes, bytesToSign);
                var decodedCrypto = Convert.ToBase64String(crypto);
                var decodedSignature = Convert.ToBase64String(signature);

                if (decodedCrypto != decodedSignature)
                {
                    throw new InvalidOperationException(string.Format("Invalid signature. Expected {0} got {1}", decodedCrypto, decodedSignature));
                }
            }

            return payloadData.ToString();
        }
      
        private static byte[] Base64UrlDecode(string input)
        {
            var output = input;
            output = output.Replace('-', '+'); // 62nd char of encoding
            output = output.Replace('_', '/'); // 63rd char of encoding

            // Pad with trailing '='s
            switch (output.Length % 4)
            {
                case 0:
                    // No pad chars in this case
                    break;
                case 2:
                    // Two pad chars
                    output += "==";
                    break;
                case 3:
                    // One pad char
                    output += "="; 
                    break;
                default:
                    throw new InvalidOperationException("Illegal base64url string!");
            }

            // Standard base64 decoder
            var converted = Convert.FromBase64String(output); 
            return converted;
        }

        private static JwtHashAlgorithm GetHashAlgorithm(string algorithm)
        {
            switch (algorithm)
            {
                case "RS256": return JwtHashAlgorithm.RS256;
                case "HS384": return JwtHashAlgorithm.HS384;
                case "HS512": return JwtHashAlgorithm.HS512;
                default: throw new InvalidOperationException("Algorithm not supported.");
            }
        }
    }
}