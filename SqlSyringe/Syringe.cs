using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SqlSyringe
{
    /// <summary>The Syringe middleware</summary>
    public class Syringe
    {
        /// <summary>
        ///     The next delegate/middleware
        /// </summary>
        private readonly RequestDelegate _next;

        /// <summary>
        ///     The options
        /// </summary>
        private readonly InjectionOptions _options;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Syringe" /> class.
        /// </summary>
        /// <param name="next">The next.</param>
        /// <param name="options">The options.</param>
        public Syringe(RequestDelegate next, InjectionOptions options)
        {
            _next = next;
            _options = options;
        }

        /// <summary>
        /// Invokes the creation asynchronously.
        /// </summary>
        /// <param name="context">The context.</param>
        public async Task InvokeAsync(HttpContext context)
        {
            if (IsApplicableTo(context))
            {
                if (context.Request.Method == HttpMethods.Get)
                {
                    //serve the empty form
                    string responseContent = GetResourceText("SqlSyringe.SyringeCaution.html");
                    await context.Response.WriteAsync(responseContent);
                }
                else if (context.Request.Method == HttpMethods.Post)
                {
                    try
                    {
                        //apply the form content

                        throw new ApplicationException("*test by masu");

                        //serve the output with the result

                    }
                    catch (Exception ex)
                    {
                        //serve the output with the Exception message
                        string responseContent = GetResourceText("SqlSyringe.SyringeResult.html");
                        responseContent = responseContent.Replace("{{OUTPUT}}", ex.Message);
                        await context.Response.WriteAsync(responseContent);
                    }
                }
            }
            else
            // Call the next delegate/middleware in the pipeline
            {
                await _next(context);
            }
        }

        /// <summary>
        /// Gets the resource text.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns></returns>
        private string GetResourceText(string resourceName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        ///     Determines whether Syringe is applicable to the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        ///     <c>true</c> if [is applicable to] [the specified context]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsApplicableTo(HttpContext context)
        {
            return
                context.Request.IsHttps &&
                context.Request.HttpContext.Request.HttpContext.Connection.RemoteIpAddress.Equals(_options.FromIp) &&
                context.Request.Path.Value.EndsWith("sql-syringe");
        }
    }
}