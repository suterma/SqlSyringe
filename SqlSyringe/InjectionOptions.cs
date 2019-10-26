using System.Net;

namespace SqlSyringe
{
    /// <summary>
    ///     Options for injecting SQL commands
    /// </summary>
    public class InjectionOptions
    {
        /// <summary>
        /// Gets or sets the required origin IP for the access check.
        /// </summary>
        /// <value>
        /// From ip.
        /// </value>
        public IPAddress FromIp { get; set; }
    }
}