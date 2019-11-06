#region copyright

// ----------------------------------------------------------------------------
// Copyright 2019 by Marcel Suter (mail@marcelsuter.ch).
// ----------------------------------------------------------------------------

#endregion

using System;
using System.Linq;
using System.Net;

namespace SqlSyringe {
    /// <summary>Options for injecting SQL commands</summary>
    public class InjectionOptions {
        /// <summary>
        ///     Gets or sets connection string to access the target database.
        /// </summary>
        /// <remarks>
        ///     If not provided here, the user must provide it in the injection request.
        /// </remarks>
        /// <value>The database connection string.</value>
        public string ConnectionString { get; set; }

        /// <summary>
        ///     Gets or sets the required origin IP for the access check.
        /// </summary>
        /// <remarks>Default ist IPv6 localhost (::1)</remarks>
        /// <value>From ip.</value>
        public IPAddress FromIp { get; set; } = IPAddress.Parse("::1");

        /// <summary>
        ///     Determines whether the connection string is provided.
        /// </summary>
        /// <remarks>
        ///     If not provided here, the user must provide it in the injection request.
        /// </remarks>
        /// <value>Whether the connection string is provided here.</value>
        public bool HasConnectionString => !string.IsNullOrEmpty(ConnectionString);

        /// <summary>
        ///     Gets or sets the URL slug, which triggers SQL Syringe.
        /// </summary>
        /// <remarks>Default is "/sql-syringe"</remarks>
        /// <value>The URL slug.</value>
        public string UrlSlug { get; set; } = "/sql-syringe";

#if NET45
        /// <summary>
        /// Gets or sets the name of the user(s). If set, authorization requires an authenticated user with any of the given name(s).
        /// </summary>
        /// <remarks>Multiple user names can be separated by comma.</remarks>
        /// <value>
        /// The name of the user(s).
        /// </value>
        /// <devdoc>Only provided for .NET 4.5, because with .NET Core, user-defined access checks are possible.</devdoc>
        public string UserName { get; set; }

        /// <summary>
        /// Gets the name of the user(s), parsed from UserName.
        /// </summary>
        /// <devdoc>Only provided for .NET 4.5, because with .NET Core, user-defined access checks are possible.</devdoc>
        public string[] UserNames => UserName == null ? new string[0] :  UserName.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        /// <summary>
        /// Gets or sets the role. If set, authorization requires an authenticated user with this role.
        /// </summary>
        /// <value>
        /// The role.
        /// </value>
        /// <devdoc>Only provided for .NET 4.5, because with .NET Core, user-defined access checks are possible.</devdoc>
        public string Role { get; set; }

        /// <summary>
        ///     Determines whether the options require user authentication.
        /// </summary>
        /// <devdoc>Only provided for .NET 4.5, because with .NET Core, user-defined access checks are possible.</devdoc>
        public bool IsUserAuthenticationRequired => UserNames.Any() || !string.IsNullOrEmpty(Role);

#endif
    }
}