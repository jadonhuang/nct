using Nct.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.DataAccess.Json
{
    public interface ISqlCreater 
    {

        SqlInfo CreateSql(JsonTable table, StatementType statementType);

        List<JsonTable> Tables { get; set; }
    }
}
