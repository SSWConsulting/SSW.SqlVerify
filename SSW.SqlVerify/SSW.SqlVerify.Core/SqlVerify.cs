using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace SSW.SqlVerify.Core
{
    public class SqlVerify : ISqlVerify
    {

        ISqlVerifyConfiguration _configuration;


        public SqlVerify(
            ISqlVerifyConfiguration configuration
            )
        {
            _configuration = configuration;
        }


        public ISqlVerifyConfiguration Configuration 
        {
            get { return _configuration; }
        }



        public void UpdateDb()
        {
            string hash = buildSchemaHash();
            saveHash(hash);
        }

        



        public bool VerifyDb()
        {
            return buildSchemaHash() == lastSchemaHash();
        }


        private string lastSchemaHash()
        {
            using (var conn = Configuration.ConnectionFactory.GetConnection())
            {
                conn.Open();
                var command = conn.CreateCommand();
                command.CommandText = "Select top 1 * from [" + Configuration.MetaDataTableName +"]  order by Date desc";

                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return reader["Hash"].ToString();
                }
                else return null;
            }
        }



        private void saveHash(string hash)
        {
            InitMetaData();

            using (var conn = Configuration.ConnectionFactory.GetConnection())
            {
                conn.Open();
                var command = conn.CreateCommand();
                command.CommandText = string.Format("insert into {0} ([Hash], [Date]) values (@hash, GetDate())", Configuration.MetaDataTableName);
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@hash";
                parameter.Value = buildSchemaHash();
                command.Parameters.Add(parameter);

                command.ExecuteNonQuery();
            }

        }



        private void InitMetaData()
        {
            using (var conn = Configuration.ConnectionFactory.GetConnection())
            {
                conn.Open();
                // check that METADATA TABLE exists
                var command = conn.CreateCommand();
                command.CommandText = "select count(table_name) from information_schema.tables where table_name='" + Configuration.MetaDataTableName + "';";
                int count = (int)command.ExecuteScalar();

                if (count == 0)
                {
                    var createTableCommand = conn.CreateCommand();
                    createTableCommand.CommandText =
                        string.Format(
                            "Create table {0} ( Id integer primary key identity(1,1), [Hash] NVARCHAR(200), [Date] DateTime); ", Configuration.MetaDataTableName);
                    createTableCommand.ExecuteNonQuery();
                }

            }
        }


        public string buildSchemaHash()
        {
            var sb = new System.Text.StringBuilder();
            using (var conn = _configuration.ConnectionFactory.GetConnection())
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
                        if (reader.IsDBNull(1))
                        {
                            sb.Append("null");
                        }
                        else
                        {
                            sb.Append(reader[i].ToString());
                        }
                        sb.Append(",");
                    }
                    
                }
            }

            return GetHashString(sb.ToString());
        }

    
        public static byte[] GetHash(string inputString)
        {
            HashAlgorithm algorithm = MD5.Create();  // SHA1.Create()
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }




    }
}
