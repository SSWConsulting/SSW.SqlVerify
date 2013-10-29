using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSW.SqlVerify.Core;

namespace SSW.SqlVerify.Core.SqlCe
{
    public class SqlCeConnectionFactory : IDbConnectionFactory
    {

        private string _connectionString;

        public SqlCeConnectionFactory(string connectionString)
        {
            this._connectionString = connectionString;
        }


        public System.Data.IDbConnection GetConnection()
        {
            return new System.Data.SqlServerCe.SqlCeConnection(_connectionString);   
        }
    }
}
