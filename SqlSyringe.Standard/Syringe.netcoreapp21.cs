using System;
using System.Data;
using System.Threading.Tasks;
#if NETCOREAPP2_1
using Microsoft.AspNetCore.Http;
#endif


namespace SqlSyringe.Standard {
    /// <summary>The Syringe middleware</summary>
    public partial class Syringe {
#if NETCOREAPP2_1
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
                        if (!context.Request.HasFormContentType) throw new ArgumentException("HTTP request form has no content type.");

                        //get the form content
                        IFormCollection form = await context.Request.ReadFormAsync();
                        string connectionString = form["connectionstring"];
                        string sqlCommand = form["sqlcommand"];
                        string isquery = form["isquery"];
                        isquery = form["isnonquery"];
                        isquery = form["querytype"].ToString();
                        bool isQuery = isquery.Equals("isquery");
       

                        //Apply the input
                        if (isQuery) {
                            //Read and serve data
                            DataTable data = new Needle().Retrieve(connectionString, sqlCommand);
                            string htmlData = Rendering.GetHtmlTableFrom(data);
                            await context.Response.WriteAsync(Rendering.GetContentWith(htmlData));
                        }
                        else {
                            //Execute and serve row count
                            int affectedRowCount = new Needle().Inject(connectionString, sqlCommand);
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



        /// <summary>
        ///     Determines whether Syringe is applicable to the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        ///     <c>true</c> if [is applicable to] [the specified context]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsApplicableTo(HttpContext context) {
            return
                context.Request.IsHttps &&
                context.Request.HttpContext.Request.HttpContext.Connection.RemoteIpAddress.Equals(_options.FromIp) &&
                context.Request.Path.Value.EndsWith("sql-syringe");
        }
  
#endif 
    }

}