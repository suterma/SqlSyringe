namespace SqlSyringe.Models {
    public class SyringeModel {
        /// <summary>
        ///     Gets or sets the connection string.
        /// </summary>
        /// <value>
        ///     The connection string.
        /// </value>
        public string ConnectionString { get; set; }

        /// <summary>
        ///     Gets or sets the SQL command.
        /// </summary>
        /// <value>
        ///     The SQL command.
        /// </value>
        public string SqlCommand { get; set; }
    }
}