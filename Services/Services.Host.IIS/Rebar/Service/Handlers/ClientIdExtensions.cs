using System;

namespace Rebar.Service.Handlers
{
    /// <summary>
    /// Class ClientIdExtensions
    /// </summary>
    public static class ClientIdExtensions
    {
        /// <summary>
        /// Determines whether the specified value is disabled.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">if set to <c>true</c> [default value].</param>
        /// <returns><c>true</c> if the specified value is disabled; otherwise, <c>false</c>.</returns>
        public static bool IsDisabled(this string value, bool defaultValue = false)
        {
            if (string.IsNullOrWhiteSpace(value)) return defaultValue;
            value = value.Trim();
            return "FALSE".Equals(value, StringComparison.OrdinalIgnoreCase)
                || "DISABLED".Equals(value, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether the specified value is enabled.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">if set to <c>true</c> [default value].</param>
        /// <returns><c>true</c> if the specified value is enabled; otherwise, <c>false</c>.</returns>
        public static bool IsEnabled(this string value, bool defaultValue = true)
        {
            return !IsDisabled(value, !defaultValue);
        }
    }
}