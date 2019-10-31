#region copyright

// ----------------------------------------------------------------------------
// Copyright 2019 by Marcel Suter (mail@marcelsuter.ch).
// ----------------------------------------------------------------------------

#endregion

#if (NETCOREAPP2_1 || NETCOREAPP3_0)
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SqlSyringe {
    /// <summary>The Syringe middleware</summary>
    /// <devdoc>
    ///     This part implements the variant for .NET Core 2.1 and .NET Core 3.0
    /// </devdoc>
    public partial class Syringe {
        /// <summary>The next delegate/middleware</summary>
        private readonly RequestDelegate _next;

        /// <summary>The options</summary>
        private InjectionOptions _options;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Syringe" /> class.
        /// </summary>
        /// <param name="next">The next.</param>
        /// <param name="options">The options.</param>
        public Syringe(RequestDelegate next, InjectionOptions options) {
            _next = next;
            AcceptOptions(options);
        }

        /// <summary>Invokes the creation asynchronously.</summary>
        /// <param name="context">The context.</param>
        public async Task InvokeAsync(HttpContext context) {
            if (IsApplicableTo(context)) {
                ApplyTo(context);
            } else {
                // Call the next delegate/middleware in the pipeline
                await _next(context);
            }
        }

        /// <summary>Determines, whether this is a GET request.</summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private bool IsGetRequest(HttpContext context) {
            return context.Request.Method == HttpMethods.Get;
        }

        /// <summary>Determines, whether this is a POST request.</summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private bool IsPostRequest(HttpContext context) {
            return context.Request.Method == HttpMethods.Post;
        }

        /// <summary>Writes a content to the context's response.</summary>
        /// <param name="context"></param>
        /// <param name="responseContent"></param>
        private void ResponseWrite(HttpContext context, string responseContent) {
            context.Response.WriteAsync(responseContent);
        }
    }
}
#endif