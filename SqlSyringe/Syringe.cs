using System;
using System.Data;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SqlSyringe {
    /// <summary>The Syringe middleware</summary>
    public class Syringe {
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
        public Syringe(RequestDelegate next, InjectionOptions options) {
            _next = next;
            _options = options ?? throw new ArgumentNullException(nameof(options), "The Syringe options are mandatory.");
            
            if (options.FromIp == null) {
                throw new ArgumentNullException(nameof(options), "The Syringe FromIp option is mandatory.");
            }
        }

        /// <summary>
        ///     Invokes the creation asynchronously.
        /// </summary>
        /// <param name="context">The context.</param>
        public async Task InvokeAsync(HttpContext context) {
            if (IsApplicableTo(context)) {
                if (context.Request.Method == HttpMethods.Get) {
                    //serve the empty form
                    string responseContent = GetResourceText("SqlSyringe.SyringeIndex.html");
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
                            string htmlData = GetHtmlTableFrom(data);
                            await context.Response.WriteAsync(GetContentWith(htmlData));
                        }
                        else {
                            //Execute and serve row count
                            int affectedRowCount = new Needle().Inject(connectionString, sqlCommand);
                            await context.Response.WriteAsync(GetContentWith($"Number of Rows affected: {affectedRowCount}"));
                        }
                    }
                    catch (Exception ex) {
                        //serve the output with the Exception message
                        string responseContent = GetResourceText("SqlSyringe.SyringeResult.html");
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
        ///     Gets the data as a HTML table with header and body for the columns and data rows.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        private string GetHtmlTableFrom(DataTable data) {
            string htmlData = "<table class='table'>";
            //Show the column name and the.NET type as header
            htmlData += "<thead><tr>";
            foreach (DataColumn dataColumn in data.Columns) {
                htmlData += "<th>";
                htmlData += $"{dataColumn.ColumnName}<br/><span class='badge badge-dark'>{dataColumn.DataType}</span>";
                htmlData += "</th>";
            }

            htmlData += "</tr></thead>";

            //Show the data as body
            htmlData += "<tbody>";
            foreach (DataRow row in data.Rows) {
                htmlData += "<tr>";
                foreach (object value in row.ItemArray) {
                    htmlData += "<td>";
                    htmlData += value.ToString();
                    htmlData += "</ td>";
                }

                htmlData += "</ tr>";
            }

            htmlData += "</tbody></table>";
            return htmlData;
        }

        /// <summary>
        ///     Gets the template with the specified message applied.
        /// </summary>
        /// <param name="message">The message.</param>
        private string GetContentWith(string message) {
            string responseContent = GetResourceText("SqlSyringe.SyringeResult.html");
            responseContent = responseContent.Replace("{{OUTPUT}}", message);
            return responseContent;
        }

        /// <summary>
        ///     Gets the resource text.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns></returns>
        private string GetResourceText(string resourceName) {
            Assembly assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                // ReSharper disable once AssignNullToNotNullAttribute because the resource is always provided with the assembly
            using (StreamReader reader = new StreamReader(stream)) {
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
        private bool IsApplicableTo(HttpContext context) {
            return
                context.Request.IsHttps &&
                context.Request.HttpContext.Request.HttpContext.Connection.RemoteIpAddress.Equals(_options.FromIp) &&
                context.Request.Path.Value.EndsWith("sql-syringe");
        }
    }
}