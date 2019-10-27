using System;
using System.Collections.Specialized;
#if NET45
using System.Web;
#elif NETCOREAPP2_1
using Microsoft.AspNetCore.Http;
#elif NETCOREAPP3_0
using Microsoft.AspNetCore.Http;
#endif


namespace SqlSyringe
{
    /// <summary>
    ///     This part of the SQL syringe implements methods that are common among all .NET variants.
    /// </summary>
    /// <devdoc>Common methods.</devdoc>
    public partial class Syringe {
        /// <summary>
        ///     Accepts the options or throws and Exception if not valid.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <exception cref="System.ArgumentNullException">
        ///     options - The Syringe options are mandatory.
        ///     or
        ///     options - The Syringe FromIp option is mandatory.
        /// </exception>
        private void AcceptOptions(InjectionOptions options) {
            _options = options ?? throw new ArgumentNullException(nameof(options), "The Syringe options are mandatory.");

            if (options.FromIp == null) throw new ArgumentNullException(nameof(options), "The Syringe FromIp option is mandatory.");
        }

        /// <summary>
        ///     Determines whether Syringe is applicable to the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        ///     <c>true</c> if [is applicable to] [the specified context]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsApplicableTo(HttpContext context) {
#if NET45
            //Use as System.Web.HttpContext
            return
                context.Request.IsSecureConnection &&
                context.Request.UserHostAddress.Equals(_options.FromIp.ToString()) &&
                context.Request.Path.EndsWith("sql-syringe");
#elif NETCOREAPP2_1
            //Use as Microsoft.AspNetCore.Http.HttpContext
            return
                context.Request.IsHttps &&
                context.Request.HttpContext.Request.HttpContext.Connection.RemoteIpAddress.Equals(_options.FromIp) &&
                context.Request.Path.Value.EndsWith("sql-syringe");
#elif NETCOREAPP3_0
            //Use as Microsoft.AspNetCore.Http.HttpContext
            return
                context.Request.IsHttps &&
                context.Request.HttpContext.Request.HttpContext.Connection.RemoteIpAddress.Equals(_options.FromIp) &&
                context.Request.Path.Value.EndsWith("sql-syringe");
#endif
        }

        private InjectionRequest GetInjectionRequest(HttpContext context)
        {
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
            return new InjectionRequest
            {
                IsQuery = isQuery,
                ConnectionString = connectionString,
                SqlCommand = sqlCommand
            };
        }
    }
}