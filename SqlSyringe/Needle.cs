using System.Data;
using System.Data.SqlClient;

namespace SqlSyringe
{
    /// <summary>
    ///     The injection needle, to apply SQL commands.
    /// </summary>
    public class Needle
    {
        /// <summary>
        /// The connection string
        /// </summary>
        private string _connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="Needle"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string to use for later applications.</param>
        public Needle(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        ///     Retrieve data using the specified Query selectCommand, using the connection string.
        /// </summary>
        /// <param name="selectCommand">The select command.</param>
        /// <returns>
        ///     The data retrieved.
        /// </returns>
        public DataTable Retrieve(string selectCommand)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand
                {
                    CommandText = selectCommand,
                    CommandType = CommandType.Text,
                    Connection = sqlConnection
                })
                {
                    sqlConnection.Open();

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
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
        /// <param name="sqlCommand">The SQL command.</param>
        /// <returns>The number of rows affected. </returns>
        public int Inject(string sqlCommand)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand
                {
                    CommandText = sqlCommand,
                    CommandType = CommandType.Text,
                    Connection = sqlConnection
                })
                {
                    sqlConnection.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }
    }
}