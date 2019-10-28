#region copyright

// ----------------------------------------------------------------------------
// Copyright 2019 by Marcel Suter (mail@marcelsuter.ch).
// ----------------------------------------------------------------------------

#endregion

using System.Net;

namespace SqlSyringe {
    /// <summary>Options for injecting SQL commands</summary>
    public class InjectionOptions {
        /// <summary>
        ///     Gets or sets the required origin IP for the access check.
        /// </summary>
        /// <value>From ip.</value>
        public IPAddress FromIp { get; set; }

        /// <summary>
        /// Gets or sets the URL slug, which triggers SQL Syringe.
        /// </summary>
        /// <remarks>Default is "/sql-syringe"</remarks>
        /// <value>
        /// The URL slug.
        /// </value>
        public string UrlSlug { get; set; } = "/sql-syringe";

        /// <summary>
        /// Gets or sets connection string to access the target database.
        /// </summary>
        /// <remarks>If not provided here, the user must provide it in the injection request.</remarks>
        /// <value>
        /// The database connection string.
        /// </value>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Determines whether the connection string is provided.
        /// </summary>
        /// <remarks>If not provided here, the user must provide it in the injection request.</remarks>
        /// <value>
        /// Whether the connection string is provided here.
        /// </value>
        public bool HasConnectionString => !string.IsNullOrEmpty(ConnectionString);
    }
}