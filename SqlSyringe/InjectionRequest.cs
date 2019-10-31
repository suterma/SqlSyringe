#region copyright

// ----------------------------------------------------------------------------
// Copyright 2019 by Marcel Suter (mail@marcelsuter.ch).
// ----------------------------------------------------------------------------

#endregion

namespace SqlSyringe {
    /// <summary>Data about the injection request.</summary>
    public class InjectionRequest {
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>
        /// The connection string.
        /// </value>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is a query for data.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is a query for data; otherwise, <c>false</c>.
        /// </value>
        public bool IsQuery { get; set; }

        /// <summary>
        /// Gets or sets the SQL command.
        /// </summary>
        /// <value>
        /// The SQL command.
        /// </value>
        public string SqlCommand { get; set; }
    }
}