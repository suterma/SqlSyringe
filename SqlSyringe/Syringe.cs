#region copyright

// ----------------------------------------------------------------------------
// Copyright 2019 by Marcel Suter (mail@marcelsuter.ch).
// ----------------------------------------------------------------------------

#endregion

using System;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Threading;
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
            isPathMatch = path.Equals(_options.UrlSlug);

            Debug.Write($"Request with https: '{isHttps}', from IP: '{fromIp}', to path: '{path}';");

            bool isApplicable = isHttps && isFromIpMatch && isPathMatch;

            if (!isApplicable) {
                Debug.Write(" not applicable because:");

                //Write the reason
                if (!isHttps) {
                    Debug.Write(" Request must be HTTPS.");
                }

                if (!isFromIpMatch) {
                    Debug.Write($" Origin IP must match configuration.");
                }

                if (!isPathMatch) {
                    Debug.Write($" Path must match configuration.");
                }
            } else {
                Debug.Write($" applicable.");
            }

            Debug.WriteLine(string.Empty);
            return isApplicable;
        }


        private void ApplyTo(HttpContext context)
        {
            if (IsGetRequest(context))
            {
                //serve the empty form
                string responseContent = Rendering.GetResourceText("SqlSyringe.SyringeIndex.html");
                responseContent = responseContent.Replace("{{CONNECTIONSTRING-INPUT-DISPLAY}}", _options.HasConnectionString ? "none" : "block");
                ResponseWrite(context, responseContent);
            }
            else if (IsPostRequest(context))
            {
                try
                {
                    Trace.WriteLine("Processing the SQL Syringe query request");
                    if (string.IsNullOrEmpty(context.Request.ContentType)) throw new ArgumentException("HTTP request form has no content type.");

                    InjectionRequest injection = GetInjectionRequest(context, _options);
                    Needle needle = new Needle(injection.ConnectionString);

                    //Apply the input
                    if (injection.IsQuery)
                    {
                        //Read and serve data
                        DataTable data = needle.Retrieve(injection.SqlCommand);
                        string htmlData = Rendering.GetHtmlTableFrom(data);
                        ResponseWrite(context, Rendering.GetContentWith(htmlData));
                    }
                    else
                    {
                        //Execute and serve row count
                        int affectedRowCount = needle.Inject(injection.SqlCommand);
                        ResponseWrite(context, Rendering.GetContentWith(Rendering.GetContentWith($"Number of Rows affected: {affectedRowCount}")));

                    }
                }
                catch (ThreadAbortException) { 
                    //do swallow these because the exception flow should end here, after one of the ResponseWrite has ended the response.
                }
                catch (Exception ex)
                {
                    //serve the output with the Exception message
                    string responseContent = Rendering.GetResourceText("SqlSyringe.SyringeResult.html");
                    responseContent = responseContent.Replace("{{OUTPUT}}", ex.Message);
                    ResponseWrite(context, responseContent);
                }
            }
        }

        /// <summary>
        ///     Accepts the options or throws and Exception if not valid.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <exception cref="System.ArgumentNullException">
        ///     options - The Syringe options are mandatory. or options - The Syringe FromIp option is mandatory.
        /// </exception>
        private void AcceptOptions(InjectionOptions options) {
            Trace.WriteLine($"Accepting the following options: source IP: {options.FromIp}, URL slug: {options.UrlSlug}, has connection string: {options.HasConnectionString}");
            _options = options ?? throw new ArgumentNullException(nameof(options), "The Syringe options are mandatory.");

            if (options.FromIp == null) {
                throw new ArgumentNullException(nameof(options), "The Syringe FromIp option is mandatory.");
            }
        }

        private InjectionRequest GetInjectionRequest(HttpContext context, InjectionOptions options) {
#if NET45
            NameValueCollection form = context.Request.Form;
            string formConnectionString = form["connectionstring"];
            string sqlCommand = form["sqlcommand"];
            bool isQuery = form["querytype"].Equals("isquery");
#elif NETCOREAPP2_1
            IFormCollection form = context.Request.ReadFormAsync().Result;
            string formConnectionString = form["connectionstring"];
            string sqlCommand = form["sqlcommand"];
            bool isQuery = form["querytype"].ToString().Equals("isquery");
#elif NETCOREAPP3_0
            IFormCollection form = context.Request.ReadFormAsync().Result;
            string formConnectionString = form["connectionstring"];
            string sqlCommand = form["sqlcommand"];
            bool isQuery = form["querytype"].ToString().Equals("isquery");
#endif

            //Choose the connection string source (form overrides, if available)
            string connectionString = string.IsNullOrEmpty(formConnectionString) ? options.ConnectionString : formConnectionString;

            //Create the injection
            InjectionRequest injection = new InjectionRequest {
                IsQuery = isQuery,
                ConnectionString = connectionString,
                SqlCommand = sqlCommand
            };
            Trace.WriteLine($"Injection is query: '{injection.IsQuery}', with command: '{injection.SqlCommand}'");
            return injection;
        }
    }
}