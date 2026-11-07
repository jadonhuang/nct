using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.DataAccess.Json
{
    public class SqlInfo
    {
        public string Sql { get; set; }

        public string ExtraSql { get; set; }

        public string IndentityIdSql { get; set; }

        public Dictionary<string, object> Parameters { get; set; } = new();

    }

    public class ConditionInfo
    {
        public string Condition { get; set; }

        public Dictionary<string, object> Parameters { get; set; } = new();

    }

}
