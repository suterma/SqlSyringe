using System.Data;
using System.IO;
using System.Reflection;

namespace SqlSyringe {
    /// <summary>
    /// Implements rendering functions for the HTML output.
    /// </summary>
    public static class Rendering {
        /// <summary>
        ///     Gets the data as a HTML table with header and body for the columns and data rows.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static string GetHtmlTableFrom(DataTable data) {
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
        public static string GetContentWith(string message) {
            string responseContent = GetResourceText("SqlSyringe.SyringeResult.html");
            responseContent = responseContent.Replace("{{OUTPUT}}", message);
            return responseContent;
        }

        /// <summary>
        ///     Gets the resource text.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns></returns>
        public static string GetResourceText(string resourceName) {
            Assembly assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                // ReSharper disable once AssignNullToNotNullAttribute because the resource is always provided with the assembly
            using (StreamReader reader = new StreamReader(stream)) {
                return reader.ReadToEnd();
            }
        }
    }
}