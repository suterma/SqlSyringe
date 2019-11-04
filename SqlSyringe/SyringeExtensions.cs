#if (NETCOREAPP2_1 || NETCOREAPP3_0)
using Microsoft.AspNetCore.Builder;

using System;
using System.Collections.Generic;
using System.Text;

namespace SqlSyringe
{
    /// <summary>
    /// Static extenstion methods for SqlSyringe
    /// </summary>
    public static class SyringeExtensions
    {

        /// <summary>
        /// Uses the SqlSyringe Middleware with the given options.
        /// </summary>
        /// <param name="app">The app to use SqlSyringe on.</param>
        /// <param name="options">The options to use.</param>
        /// <returns>The app with SqlSyringe applied.</returns>
            public static IApplicationBuilder UseSqlSyringe(this IApplicationBuilder app, InjectionOptions options)
            {
            app.UseMiddleware<Syringe>(options);
            return app;
            }
    }
}
#endif