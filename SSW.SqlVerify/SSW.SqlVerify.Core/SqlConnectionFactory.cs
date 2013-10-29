using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace SSW.SqlVerify.Core
{
    public class SqlConnectionFactory
    {

        private string _connectionString;

        public SqlConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }


        public System.Data.IDbConnection GetConnection()
        {
            return new System.Data.SqlClient.SqlConnection(_connectionString);   
        }

    }
}
