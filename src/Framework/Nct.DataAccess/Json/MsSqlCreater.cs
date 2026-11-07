using Nct.Extentions;
using Nct.Ioc;
using System.Text;

namespace Nct.DataAccess.Json
{
    public class MsSqlCreater : BaseSqlCreater, ITransientDependency
    {


        protected override string GetIndentitySql()
        {
            return "select id=@@IDENTITY";
        }

        private string GetTableName(JsonTable table)
        {
            return table.Joins.Count != 0 ? table.Table : CreateQuote(table.Table);
        }

        protected override string CreateTopSqlString(JsonTable table, string fields, string where, string groupBy, string having, string orderBy)
        {         
            var sql = $"select top {table.Count} {fields} from {GetTableName(table)} {where} {groupBy} {having} {orderBy}";
            return sql;
          
        }

        protected override string CreatePageSqlString(JsonTable table, string fields, string where, string groupBy, string having, string orderBy)
        {

            table.Count ??= 10;

            var sql = new StringBuilder();

            if (orderBy.IsEmpty()) orderBy = "order by (select 0)";

            var rowIndex = SqlHelper.RowIndexName;

            var start = (table.Page.Value - 1) * table.Count.Value;

            sql.Append($"select * from (select {fields}, row_number() over({orderBy}) {rowIndex}");
            sql.Append($" from {GetTableName(table)} {where} {groupBy} {having}");
            sql.Append(")tb");
            sql.Append($" where {rowIndex} > {start}");
            sql.Append($" and {rowIndex} <= {start + table.Count.Value}");

            return sql.ToString();


        }


    }
}
