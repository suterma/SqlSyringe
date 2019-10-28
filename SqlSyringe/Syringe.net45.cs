﻿using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
#if NET45
using System;
using System.Web;

namespace SqlSyringe {
    /// <summary>The SQL Syringe HTTP Module.</summary>
    /// <remarks>Register this Module into the ASP.NET HTTP request pipeline.</remarks>
    /// <devdoc>This part implements the variant for .NET 4.5, as a HTTP Module.</devdoc>
    public partial class Syringe : IHttpModule {
        private InjectionOptions _options;

        public Syringe(InjectionOptions options) {
            AcceptOptions(options);
        }

        public void Dispose() {
        }

        public void Init(HttpApplication application) {
            Trace.WriteLine("Initializing the SQL Syringe .NET 4.5 module...");
            application.BeginRequest += Application_BeginRequest;
            Trace.WriteLine("SQL Syringe .NET 4.5 module initialization done.");
        }

        private void Application_BeginRequest(object source, EventArgs e) {
            HttpContext context = ((HttpApplication) source).Context;

            //Handle the context, if applicable
            Treat(context);
        }

        /// <summary>
        ///     Invokes the treatment of the request, if applicable.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Treat(HttpContext context) {
            if (IsApplicableTo(context)) {
                if (context.Request.RequestType == "GET") {
                    Trace.WriteLine("Serving the empty SQL Syringe form");
                    string responseContent = Rendering.GetResourceText("SqlSyringe.SyringeIndex.html");
                    ResponseWrite(context, responseContent);
                }
                else if (context.Request.RequestType == "POST") {
                    try
                    {
                        Trace.WriteLine("Processing the SQL Syringe query request");
                        if (string.IsNullOrEmpty(context.Request.ContentType)) throw new ArgumentException("HTTP request form has no content type.");

                        InjectionRequest injection = GetInjectionRequest(context);
                        Needle needle = new Needle(injection.ConnectionString);

                        //Apply the input
                        if (injection.IsQuery) {
                            //Read and serve data
                            DataTable data = needle.Retrieve(injection.SqlCommand);
                            string htmlData = Rendering.GetHtmlTableFrom(data);
                            ResponseWrite(context, Rendering.GetContentWith(htmlData));
                        }
                        else {
                            //Execute and serve row count
                            int affectedRowCount = needle.Inject(injection.SqlCommand);
                            ResponseWrite(context, Rendering.GetContentWith($"Number of Rows affected: {affectedRowCount}"));
                        }
                    }
                    catch (Exception ex) {
                        //serve the output with the Exception message
                        string responseContent = Rendering.GetResourceText("SqlSyringe.SyringeResult.html");
                        responseContent = responseContent.Replace("{{OUTPUT}}", ex.Message);
                        ResponseWrite(context, responseContent);
                    }
                }
            }
        }

        private void ResponseWrite(HttpContext context, string responseContent) {
            context.Response.Clear();
            context.Response.Write(responseContent);
            context.Response.End();
        }
    }
}
#endif