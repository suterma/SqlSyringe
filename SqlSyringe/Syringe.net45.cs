#region copyright

// ----------------------------------------------------------------------------
// Copyright 2019 by Marcel Suter (mail@marcelsuter.ch).
// ----------------------------------------------------------------------------

#endregion

#if NET45
using System;
using System.Diagnostics;
using System.Linq;
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

            //SqlSyringe treats the request only ever after a possibly required authentication was handeled
            application.PostAuthenticateRequest += Application_PostAuthenticateRequest;

            Trace.WriteLine("SQL Syringe .NET 4.5 module initialization done.");
        }

        private void Application_PostAuthenticateRequest(object source, EventArgs e)
        {
            HttpContext context = ((HttpApplication)source).Context;
            //Treat the request, with the possibly applied authentication on the context 
            Treat(context);
        }
        /// <summary>
        /// Determines whether the request complies with the possibly required user-based authorization.
        /// </summary>
        /// <param name="context"></param>
        /// <returns><c>true</c> when the request complies with the requirements; <c>false</c> otherwise.</returns>
        private bool IsUserAuthorized(HttpContext context)
        {
            //Authentication required by SqlSyringe options?
            if (_options.IsUserAuthenticationRequired)
            {
                if (context.Request.IsAuthenticated)
                {
                    //Authorize the user as configured
                    if (
                        (string.IsNullOrEmpty(_options.Role) || context.User.IsInRole(_options.Role)) &&
                        (string.IsNullOrEmpty(_options.UserName) || _options.UserNames.Contains(context.User.Identity.Name))
                            )
                    {
                        return true;
                    }
                }
            }
            else
            {
                //When no user-based authorization is required, treat the request anyway
                return true;
            }
            //In any other cases, leave the request alone.
            return false;
        }

        /// <summary>
        ///     Invokes the treatment of the request, if applicable.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Treat(HttpContext context) {
            if (IsUserAuthorized(context) && IsApplicableTo(context)) {
                ApplyTo(context);
            }
        }



        /// <summary>Determines, whether this is a GET request.</summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private bool IsGetRequest(HttpContext context) {
            return context.Request.RequestType == "GET";
        }

        /// <summary>Determines, whether this is a POST request.</summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private bool IsPostRequest(HttpContext context) {
            return context.Request.RequestType == "POST";
        }

        /// <summary>Writes a content to the context's response.</summary>
        /// <param name="context"></param>
        /// <param name="responseContent"></param>
        private void ResponseWrite(HttpContext context, string responseContent) {
            context.Response.Clear();
            context.Response.Write(responseContent);
            context.Response.Flush();
            context.Response.End();
        }
    }
}
#endif