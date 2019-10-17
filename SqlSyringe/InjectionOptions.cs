using System.Net;

namespace SqlSyringe {
    /// <summary>
    ///     Option for injecting SQL commands
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