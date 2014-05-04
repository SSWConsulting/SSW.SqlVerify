namespace SSW.SqlVerify.Core
{
    using System.Data.SqlClient;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Core class to manage hashes
    /// </summary>
    public class SqlVerify : ISqlVerify
    {
        /// <summary>
        /// Configurations object.
        /// </summary>
        private ISqlVerifyConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlVerify" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public SqlVerify(ISqlVerifyConfiguration configuration)
        {
            this._configuration = configuration;
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        public ISqlVerifyConfiguration Configuration 
        {
            get { return this._configuration; }
        }

        /// <summary>
        /// Gets the hash.
        /// </summary>
        /// <param name="inputString">The input string.</param>
        /// <returns>Binary hash of the string.</returns>
        public static byte[] GetHash(string inputString)
        {
            HashAlgorithm algorithm = MD5.Create();  // SHA1.Create()
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        /// <summary>
        /// Gets the hash string.
        /// </summary>
        /// <param name="inputString">The input string.</param>
        /// <returns>Hash of the string.</returns>
        public static string GetHashString(string inputString)
        {
            var sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
            {
                sb.Append(b.ToString("X2"));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Updates the database.
        /// </summary>
        public void UpdateDb()
        {
            this.SaveHash();
        }

        /// <summary>
        /// Verifies the database by comparing last hash with a current DB hash.
        /// </summary>
        /// <returns>True, if hashes are the same, false otherwise</returns>
        public bool VerifyDb()
        {
            if (this.CheckIfExists())
            {
                return this.BuildSchemaHash() == this.LastSchemaHash();
            }

            this.UpdateDb();
            return true;
        }

        /// <summary>
        /// Checks if exists.
        /// </summary>
        /// <returns></returns>
        private bool CheckIfExists()
        {
            using (var conn = this.Configuration.ConnectionFactory.GetConnection())
            {
                conn.Open();
                
                // check that METADATA TABLE exists
                var command = conn.CreateCommand();
                command.CommandText = "SELECT COUNT(table_name) FROM information_schema.tables WHERE table_name='" + this.Configuration.MetaDataTableName + "';";
                var count = (int)command.ExecuteScalar();

                return count > 0;
            }
        }

        /// <summary>
        /// Builds the schema hash.
        /// </summary>
        /// <returns>New computed hash.</returns>
        public string BuildSchemaHash()
        {
            var sb = new StringBuilder();
            using (var conn = this._configuration.ConnectionFactory.GetConnection())
            {
                if (conn.State != System.Data.ConnectionState.Open)
                {
                    conn.Open();
                }

                var command = conn.CreateCommand();
                command.CommandText = "SELECT * FROM INFORMATION_SCHEMA.COLUMNS";

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        sb.Append(reader.IsDBNull(1) ? "NULL" : reader[i].ToString());

                        sb.Append(",");
                    }
                }
            }

            return GetHashString(sb.ToString());
        }

        /// <summary>
        /// Initializes the meta data.
        /// </summary>
        private void InitMetaData()
        {
            using (var conn = this.Configuration.ConnectionFactory.GetConnection())
            {
                conn.Open();
                
                // check that METADATA TABLE exists
                var command = conn.CreateCommand();
                command.CommandText = "SELECT COUNT(table_name) FROM information_schema.tables WHERE table_name='" + this.Configuration.MetaDataTableName + "';";
                var count = (int)command.ExecuteScalar();

                if (count == 0)
                {
                    var createTableCommand = conn.CreateCommand();
                    createTableCommand.CommandText =
                        string.Format(
                            "CREATE TABLE {0} ( Id integer primary key identity(1,1), [Hash] NVARCHAR(200), [Date] DateTime); ", this.Configuration.MetaDataTableName);
                    createTableCommand.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Gets the last schema hash.
        /// </summary>
        /// <returns>Last hash</returns>
        private string LastSchemaHash()
        {
            using (var conn = this.Configuration.ConnectionFactory.GetConnection())
            {
                conn.Open();
                var command = conn.CreateCommand();
                command.CommandText = "SELECT TOP 1 * " + 
                                      "FROM [" + this.Configuration.MetaDataTableName + "] " + 
                                      "ORDER BY Date DESC";

                var reader = command.ExecuteReader();
                try
                {
                    if (reader.Read())
                    {
                        return reader["Hash"].ToString();
                    }
                }
                catch (SqlException)
                {
                    return null;
                }

                return null;
            }
        }

        /// <summary>
        /// Saves the hash.
        /// </summary>
        private void SaveHash()
        {
            this.InitMetaData();

            using (var conn = this.Configuration.ConnectionFactory.GetConnection())
            {
                conn.Open();
                var command = conn.CreateCommand();
                command.CommandText = string.Format("INSERT INTO {0} ([Hash], [Date]) VALUES (@hash, GetDate())", this.Configuration.MetaDataTableName);
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@hash";
                parameter.Value = this.BuildSchemaHash();
                command.Parameters.Add(parameter);

                command.ExecuteNonQuery();
            }
        }
    }
}
