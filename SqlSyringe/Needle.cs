#region copyright

// ----------------------------------------------------------------------------
// Copyright 2019 by nowhow solutions AG.
// 
// Das vorliegende Dokument, einschliesslich aller seiner Teile, ist
// urheberrechtlich geschützt. Jede Verwertung ist ohne Zustimmung der
// nowhow solutions AG unzulässig. Dies gilt insbesondere für
// Vervielfältigungen, die Einspeicherung und Verarbeitung in elektronischer
// Form. Wird das Dokument einem Kunden im Rahmen der Projektarbeit zur Ansicht
// übergeben ist jede weitere Verteilung durch den Kunden an Dritte untersagt.
// ----------------------------------------------------------------------------

#endregion

using System.Data;
using System.Data.SqlClient;

namespace SqlSyringe {
    /// <summary>
    ///     The injection needle, to get and apply data.
    /// </summary>
    public class Needle {
        /// <summary>
        ///     Retrieve data using the specified Query selectCommand, using the connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="selectCommand">The select command.</param>
        /// <returns>
        ///     The data retrieved.
        /// </returns>
        public DataTable Retrieve(string connectionString, string selectCommand) {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString)) {
                using (SqlCommand cmd = new SqlCommand {
                    CommandText = selectCommand,
                    CommandType = CommandType.Text,
                    Connection = sqlConnection
                }) {
                    sqlConnection.Open();

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd)) {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        /// <summary>
        ///     Injects the specified Non-Query sqlCommand, using the connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="sqlCommand">The SQL command.</param>
        /// <returns>The number of rows affected. </returns>
        public int Inject(string connectionString, string sqlCommand) {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString)) {
                using (SqlCommand cmd = new SqlCommand {
                    CommandText = sqlCommand,
                    CommandType = CommandType.Text,
                    Connection = sqlConnection
                }) {
                    sqlConnection.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }
    }
}