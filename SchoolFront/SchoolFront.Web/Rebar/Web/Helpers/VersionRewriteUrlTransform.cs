using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Optimization;

using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace Accenture.Rebar.Web.Infrastructure.Helpers
{
    /// <summary>
    /// This is the code for <c>CssRewriteUrlTransform</c> with the addition of version query parameter for cache busting
    /// </summary>
    public class VersionRewriteUrlTransform : IItemTransform
    {
        /// <summary>
        /// Processes the specified included virtual path.
        /// </summary>
        /// <param name="includedVirtualPath">The included virtual path.</param>
        /// <param name="input">The input.</param>
        /// <returns>The transformed URL.</returns>
        public string Process(string includedVirtualPath, string input)
        {
            if (includedVirtualPath == null) throw new ArgumentNullException("includedVirtualPath");

            var virtualDirectory = VirtualPathUtility.GetDirectory(includedVirtualPath);
            if (string.IsNullOrWhiteSpace(virtualDirectory))
            {
                virtualDirectory = "~/";
            }
            else
            {
                if (virtualDirectory[0] == '/')
                {
                    virtualDirectory = "~" + virtualDirectory;
                }
            }

            return ConvertUrlsToAbsolute(virtualDirectory, input);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Intentionally swallowing exceptions.")]
        internal static string RebaseUrlToAbsolute(string baseUrl, string url)
        {
            if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(baseUrl)) return url;
            if (!baseUrl.EndsWith("/", StringComparison.OrdinalIgnoreCase)) baseUrl = baseUrl + "/";

            var isImage = url.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                          || url.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                          || url.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
                          || url.EndsWith(".gif", StringComparison.OrdinalIgnoreCase);

            if (url.StartsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                return isImage ? VirtualPathUtility.ToAbsolute(StaticFile.Version(url)) : url;
            }

            var result = url;
            try
            {
                result = isImage
                             ? VirtualPathUtility.ToAbsolute(StaticFile.Version(baseUrl + url))
                             : VirtualPathUtility.ToAbsolute(baseUrl + url);
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message, "Warning");
            }

            return result;
        }

        internal static string ConvertUrlsToAbsolute(string baseUrl, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return content;

            return new Regex("url\\(['\"]?(?<url>[^)]+?)['\"]?\\)").Replace(
                content, match => "url(" + RebaseUrlToAbsolute(baseUrl, match.Groups["url"].Value) + ")");
        }
    }
}