#region copyright

// ----------------------------------------------------------------------------
// Copyright 2019 by Marcel Suter (mail@marcelsuter.ch).
// ----------------------------------------------------------------------------

#endregion

using System.Data;
using System.Diagnostics;
#if NET45
using System;
using System.Web;

namespace SqlSyringe {
    /// <summary>The SQL Syringe HTTP Module.</summary>
    /// <remarks>
    ///     Register this Module into the ASP.NET HTTP request pipeline.
    /// </remarks>
    /// <devdoc>
    ///     This part implements the variant for .NET 4.5, as a HTTP Module.
    /// </devdoc>
    public partial class Syringe : IHttpModule {
        private InjectionOptions _options;

        public Syringe(InjectionOptions options) {
            AcceptOptions(options);
        }

        public void Dispose() { }

        public void Init(HttpApplication application) {
            Trace.WriteLine("Initializing the SQL Syringe .NET 4.5 module...");
            application.BeginRequest += Application_BeginRequest;
            Trace.WriteLine("SQL Syringe .NET 4.5 module initialization done.");
        }

        /// <summary>
        ///     Invokes the treatment of the request, if applicable.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Treat(HttpContext context) {
            if (IsApplicableTo(context)) {
                ApplyTo(context);
            }
        }


        /// <summary>
        /// Determines, whether this is a POST request.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private bool IsPostRequest(HttpContext context)
        {
            return context.Request.RequestType == "POST";
        }

        private void Application_BeginRequest(object source, EventArgs e) {
            HttpContext context = ((HttpApplication) source).Context;

            //Handle the context, if applicable
            Treat(context);
        }

        /// <summary>
        /// Writes a content to the context's response.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="responseContent"></param>
        private void ResponseWrite(HttpContext context, string responseContent) {
            context.Response.Clear();
            context.Response.Write(responseContent);
            context.Response.Flush();
            context.Response.End();
        }
        /// <summary>
        /// Determines, whether this is a GET request.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private bool IsGetRequest(HttpContext context)
        {
            return context.Request.RequestType == "GET";
        }
    }
}
#endif