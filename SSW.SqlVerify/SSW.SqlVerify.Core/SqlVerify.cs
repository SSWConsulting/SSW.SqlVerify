using System;
using System.Collections.Generic;
using System.Linq;
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
            throw new NotImplementedException();
        }



        private void saveHash(string hash)
        {
            throw new NotImplementedException();
        }



        private void InitMetaData()
        {

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
