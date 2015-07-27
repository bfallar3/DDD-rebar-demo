using System;
using System.IO;
using System.Security.Cryptography;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;

namespace Accenture.Rebar.Web.Infrastructure.Helpers
{
    /// <summary>
    /// Methods to version static files.
    /// </summary>
    public static class StaticFile
    {
        /// <summary>
        /// Versions the specified relative path.
        /// </summary>
        /// <param name="relativePath">The relative path.</param>
        /// <returns>The versioned relative path.</returns>
        public static string Version(string relativePath)
        {
            if (relativePath == null) return null;

            if (HttpRuntime.Cache[relativePath] == null)
            {
                var absolutePath = HostingEnvironment.MapPath(relativePath);
                if (!File.Exists(absolutePath) && relativePath.StartsWith("/", StringComparison.OrdinalIgnoreCase))
                {
                  absolutePath = HostingEnvironment.MapPath("~" + relativePath);
                }

                if (absolutePath != null && File.Exists(absolutePath))
                {
                    using (var stream = File.OpenRead(absolutePath))
                    using (var sha = new SHA1Managed())
                    {
                        var hash = sha.ComputeHash(stream);
                        var hashString = BitConverter.ToString(hash).Replace("-", string.Empty);

                        if (relativePath.StartsWith("~", StringComparison.OrdinalIgnoreCase))
                        {
                          relativePath = VirtualPathUtility.ToAbsolute(relativePath);
                        }

                        var versionedUrl = relativePath + "?v=" + hashString;

                        using (var cacheDependency = new CacheDependency(absolutePath)) HttpRuntime.Cache.Insert(relativePath, versionedUrl, cacheDependency);
                    }
                }
            }

            return HttpRuntime.Cache[relativePath] as string;
        }
    }
}