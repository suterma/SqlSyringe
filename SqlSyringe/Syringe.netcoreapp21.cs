#if NETCOREAPP2_1
using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;


namespace SqlSyringe
{
    /// <summary>The Syringe middleware</summary>
    /// <devdoc>This part implements the variant for .NET Core 2.1</devdoc>
    public partial class Syringe {

        /// <summary>
        ///     The next delegate/middleware
        /// </summary>
        private readonly RequestDelegate _next;

        /// <summary>
        ///     The options
        /// </summary>
        private  InjectionOptions _options;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Syringe" /> class.
        /// </summary>
        /// <param name="next">The next.</param>
        /// <param name="options">The options.</param>
        public Syringe(RequestDelegate next, InjectionOptions options) {
            _next = next;
            AcceptOptions(options);
        }

        /// <summary>
        ///     Invokes the creation asynchronously.
        /// </summary>
        /// <param name="context">The context.</param>
        public async Task InvokeAsync(HttpContext context) {
            if (IsApplicableTo(context)) {
                if (context.Request.Method == HttpMethods.Get) {
                    //serve the empty form
                    string responseContent = Rendering.GetResourceText("SqlSyringe.SyringeIndex.html");
                    await context.Response.WriteAsync(responseContent);
                }
                else if (context.Request.Method == HttpMethods.Post) {
                    try {
                        Trace.WriteLine("Processing the SQL Syringe query request");
                        if (string.IsNullOrEmpty(context.Request.ContentType)) throw new ArgumentException("HTTP request form has no content type.");

                        InjectionRequest injection = GetInjectionRequest(context);

                        //Apply the input
                        if (injection.IsQuery) {
                            //Read and serve data
                            DataTable data = new Needle().Retrieve(injection.ConnectionString, injection.SqlCommand);
                            string htmlData = Rendering.GetHtmlTableFrom(data);
                            await context.Response.WriteAsync(Rendering.GetContentWith(htmlData));
                        }
                        else {
                            //Execute and serve row count
                            int affectedRowCount = new Needle().Inject(injection.ConnectionString, injection.SqlCommand);
                            await context.Response.WriteAsync(Rendering.GetContentWith($"Number of Rows affected: {affectedRowCount}"));
                        }
                    }
                    catch (Exception ex) {
                        //serve the output with the Exception message
                        string responseContent = Rendering.GetResourceText("SqlSyringe.SyringeResult.html");
                        responseContent = responseContent.Replace("{{OUTPUT}}", ex.Message);
                        await context.Response.WriteAsync(responseContent);
                    }
                }
            }
            else {
                // Call the next delegate/middleware in the pipeline
                await _next(context);
            }
        }


    }
}
#endif 
