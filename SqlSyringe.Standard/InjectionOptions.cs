using System.Net;

namespace SqlSyringe.Standard {
    /// <summary>
    ///     Options for injecting SQL commands
    /// </summary>
    public class InjectionOptions {
        /// <summary>
        /// Gets or sets from ip.
        /// </summary>
        /// <value>
        /// From ip.
        /// </value>
        public IPAddress FromIp { get; set; }
    }
}