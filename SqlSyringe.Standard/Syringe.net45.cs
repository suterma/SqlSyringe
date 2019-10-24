#if NET45
using System;
using System.Collections.Specialized;
using System.Data;
using System.Web;

namespace SqlSyringe.Standard {
    /// <summary>The Syringe HTTP Module</summary>
    /// <devdoc>This part implements the variant for .NET 4.5</devdoc>
    public partial class Syringe : IHttpModule
    {
        private InjectionOptions _options;

        public Syringe(InjectionOptions options)
        {
            AcceptOptions(options);
        }

        public void Dispose()
        {
        }

        public void Init(HttpApplication application)
        {
            application.BeginRequest += (new EventHandler(this.Application_BeginRequest));
        }

        private void Application_BeginRequest(Object source, EventArgs e)
        {
            HttpContext context = ((HttpApplication)source).Context;

            //Handle the context, as Syringe, if applicable
            Treat(context);
        }




        /// <summary>
        ///     Invokes the treatment of the request, if applicable.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Treat(HttpContext context)
        {
            if (IsApplicableTo(context))
            {
                if (context.Request.RequestType == "GET")
                {
                    //serve the empty form
                    string responseContent = Rendering.GetResourceText("SqlSyringe.Standard.SyringeIndex.html");
                    ResponseWrite(context, responseContent);

                }
                else if (context.Request.RequestType == "POST")
                {
                    try
                    {
                        if (string.IsNullOrEmpty(context.Request.ContentType)) throw new ArgumentException("HTTP request form has no content type.");

                        //get the form content
                        NameValueCollection form = context.Request.Form;
                        string connectionString = form["connectionstring"];
                        string sqlCommand = form["sqlcommand"];
                        bool isQuery = form["querytype"].ToString().Equals("isquery");

                        //Apply the input
                        if (isQuery)
                        {
                            //Read and serve data
                            DataTable data = new Needle().Retrieve(connectionString, sqlCommand);
                            string htmlData = Rendering.GetHtmlTableFrom(data);
                            ResponseWrite(context, Rendering.GetContentWith(htmlData));
                        }
                        else
                        {
                            //Execute and serve row count
                            int affectedRowCount = new Needle().Inject(connectionString, sqlCommand);
                            ResponseWrite(context, Rendering.GetContentWith($"Number of Rows affected: {affectedRowCount}"));
                        }
                    }
                    catch (Exception ex)
                    {
                        //serve the output with the Exception message
                        string responseContent = Rendering.GetResourceText("SqlSyringe.Standard.SyringeResult.html");
                        responseContent = responseContent.Replace("{{OUTPUT}}", ex.Message);
                        ResponseWrite(context, responseContent);
                    }
                }
            }
        }

        private void ResponseWrite(HttpContext context, string responseContent)
        {
            context.Response.Clear();
            context.Response.Write(responseContent);
            context.Response.End();
        }
    }
}
#endif