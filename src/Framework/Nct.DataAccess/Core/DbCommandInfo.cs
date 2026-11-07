using System;

namespace Nct.DataAccess.Core
{
    public class DbCommandInfo
    {

        public DbCommandInfo(string sql, object param)
        {
            Sql = sql;
            Param = param;
        }


        public string Sql { get; set; }


        public object Param { get; set; }


        public Exception Exception { get; set; }

    }
}
