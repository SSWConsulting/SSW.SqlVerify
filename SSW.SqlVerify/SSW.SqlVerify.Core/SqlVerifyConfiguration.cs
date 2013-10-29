using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSW.SqlVerify.Core
{
    public class SqlVerifyConfiguration : ISqlVerifyConfiguration
    {


        public SqlVerifyConfiguration()
        {
        }


        public SqlVerifyConfiguration(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }


        private IDbConnectionFactory _connectionFactory;
        public virtual IDbConnectionFactory ConnectionFactory
        {
            get { return _connectionFactory; }
            set { _connectionFactory = value; }
        }

        private string _metaDataTableName;
        public virtual string MetaDataTableName
        {
            get { return _metaDataTableName ?? "SSWSqlVerifyMetaData"; }
            set { _metaDataTableName = value; }
        }



    }
}
