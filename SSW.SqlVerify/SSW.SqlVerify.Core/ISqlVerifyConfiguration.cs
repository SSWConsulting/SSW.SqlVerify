using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;


namespace SSW.SqlVerify.Core
{
    public interface ISqlVerifyConfiguration
    {

        IDbConnectionFactory ConnectionFactory { get; }

        String MetaDataTableName { get; }

    }
}
