using System;
using System.Diagnostics;
using System.Linq;

namespace Rebar.Web
{
    /// <summary>
    /// Extension methods for REBAR web.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Adds .fiddler to the host name if Fiddler is running.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>
        /// The updated URI.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2234:PassSystemUriObjectsInsteadOfStrings", Justification = "Intentional.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "0#", Justification = "Intentional.")]
        public static string AddFiddler(this string url)
        {
            return new UriBuilder(url).AddFiddler().Uri.ToString();
        }

        /// <summary>Adds the .fiddler domain suffix to the host name of the Uri if the Fiddler.exe process is running.
        /// </summary>
        /// <param name="uriBuilder">The URI builder containing the URI to append the host name to</param>
        /// <returns><paramref name="uriBuilder"/> with the <see cref="UriBuilder.Host"/> property having .fiddler appended to it, only if the host was local.</returns>
        /// <exception cref="System.ArgumentNullException">when <paramref name="uriBuilder"/> is <c>null</c></exception>
        public static UriBuilder AddFiddler(this UriBuilder uriBuilder)
        {
            if (uriBuilder == null) throw new ArgumentNullException("uriBuilder");
            if (!uriBuilder.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase)) return uriBuilder;
            var processess = Process.GetProcessesByName("Fiddler");
            if (processess.Length > 0)
            {
                uriBuilder.Host = uriBuilder.Host + ".fiddler";
            }

            return uriBuilder;
        }

        /// <summary>Finds the parent uri using the parentSegment as a stop.</summary>
        /// <param name="fullUri">A full uri</param>
        /// <param name="parentSegment">The uri prefix used as a stop.</param>
        /// <returns>A UriBuilder with the path of the <paramref name="fullUri"/> truncated up to and including the <paramref name="parentSegment"/>.</returns>
        /// <exception cref="System.ArgumentNullException">when <paramref name="fullUri"/> is <c>null</c>.</exception>
        /// <remarks>Path will always end in /</remarks>
        public static UriBuilder FindParentUri(this Uri fullUri, string parentSegment)
        {
            if (fullUri == null) throw new ArgumentNullException("fullUri");
            string path = string.Empty;

            if (!string.IsNullOrEmpty(parentSegment))
            {
                string uriPrefixWithTrailingSlash = parentSegment + "/";

                path =
                  fullUri.Segments

                  // Get all segments up to the parentSegment
                  .TakeWhile(
                    x =>
                      !(parentSegment.Equals(x, StringComparison.OrdinalIgnoreCase)
                        || (x.EndsWith("/", StringComparison.Ordinal) && uriPrefixWithTrailingSlash.Equals(x, StringComparison.OrdinalIgnoreCase))))

                  // Concatenate the segments back together
                  .Aggregate(path, (p, x) => p + x);

                // Add back the parentSegment
                path += parentSegment;
            }

            // Ensure path always ends in a slash
            if (!path.EndsWith("/", StringComparison.Ordinal))
            {
                path += "/";
            }

            return new UriBuilder(fullUri) { Path = path };
        }
    }
}
