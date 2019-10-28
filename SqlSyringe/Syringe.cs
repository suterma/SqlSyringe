#region copyright

// ----------------------------------------------------------------------------
// Copyright 2019 by Marcel Suter (mail@marcelsuter.ch).
// ----------------------------------------------------------------------------

#endregion

using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
#if NET45
using System.Web;

#elif NETCOREAPP2_1
using Microsoft.AspNetCore.Http;
#elif NETCOREAPP3_0
using Microsoft.AspNetCore.Http;
#endif

namespace SqlSyringe {
    /// <summary>
    ///     This part of the SQL syringe implements methods that are common among all .NET variants.
    /// </summary>
    /// <devdoc>Common methods.</devdoc>
    public partial class Syringe {
        /// <summary>
        ///     Determines whether Syringe is applicable to the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        ///     <c>true</c> if [is applicable to] [the specified context]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsApplicableTo(HttpContext context) {
            Debug.Write($"Determining whether SqlSyringe is applicable to the context: ");
            bool isHttps;
            bool isFromIpMatch;
            bool isPathMatch;

#if NET45

            //Use as System.Web.HttpContext
            isHttps = context.Request.IsSecureConnection;
            IPAddress fromIp = IPAddress.Parse(context.Request.UserHostAddress);
            string path = context.Request.Path;
#elif NETCOREAPP2_1
            //Use as Microsoft.AspNetCore.Http.HttpContext
            isHttps = context.Request.IsHttps;
            IPAddress fromIp = context.Request.HttpContext.Request.HttpContext.Connection.RemoteIpAddress;
            string path = context.Request.Path;
#elif NETCOREAPP3_0
            //Use as Microsoft.AspNetCore.Http.HttpContext
            isHttps = context.Request.IsHttps;
            IPAddress fromIp = context.Request.HttpContext.Request.HttpContext.Connection.RemoteIpAddress;
            string path = context.Request.Path;
#endif
            isFromIpMatch = fromIp.Equals(_options.FromIp);
            isPathMatch = path.EndsWith("sql-syringe");

            Debug.Write($"Request with https: '{isHttps}', from IP: '{fromIp}', to path: '{path}'; ");

            bool isApplicable = isHttps && isFromIpMatch && isPathMatch;

            if (!isApplicable) {
                Debug.Write(" is not applicable because:");

                //Write the reason
                if (!isHttps) {
                    Debug.Write(" Request must be HTTPS.");
                }

                if (!isFromIpMatch) {
                    Debug.Write($" Origin IP '{fromIp}' must match configuration.");
                }

                if (!isPathMatch) {
                    Debug.Write($" Path '{path}' must match configuration.");
                }
            } else {
                Debug.Write($" is applicable.");
            }

            Debug.WriteLine(string.Empty);
            return isApplicable;
        }

        /// <summary>
        ///     Accepts the options or throws and Exception if not valid.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <exception cref="System.ArgumentNullException">
        ///     options - The Syringe options are mandatory. or options - The Syringe FromIp option is mandatory.
        /// </exception>
        private void AcceptOptions(InjectionOptions options) {
            Trace.WriteLine($"Accepting the following options: {options.FromIp}");
            _options = options ?? throw new ArgumentNullException(nameof(options), "The Syringe options are mandatory.");

            if (options.FromIp == null) {
                throw new ArgumentNullException(nameof(options), "The Syringe FromIp option is mandatory.");
            }
        }

        private InjectionRequest GetInjectionRequest(HttpContext context) {
#if NET45
            NameValueCollection form = context.Request.Form;
            string connectionString = form["connectionstring"];
            string sqlCommand = form["sqlcommand"];
            bool isQuery = form["querytype"].Equals("isquery");
#elif NETCOREAPP2_1
            IFormCollection form = context.Request.ReadFormAsync().Result;
            string connectionString = form["connectionstring"];
            string sqlCommand = form["sqlcommand"];
            bool isQuery = form["querytype"].ToString().Equals("isquery");
#elif NETCOREAPP3_0
            IFormCollection form = context.Request.ReadFormAsync().Result;
            string connectionString = form["connectionstring"];
            string sqlCommand = form["sqlcommand"];
            bool isQuery = form["querytype"].ToString().Equals("isquery");
#endif
            var injection = new InjectionRequest {
                IsQuery = isQuery,
                ConnectionString = connectionString,
                SqlCommand = sqlCommand
            };
            Trace.WriteLine($"Injection is query: '{injection.IsQuery}', with command: '{injection.SqlCommand}'");
            return injection;
        }
    }
}