using Nct.DataAccess.DynamicQuery.Extensions;
using System.Linq;

namespace Nct.DataAccess.DynamicQuery
{
    public static class Sql
    {

       
        public static IQueryable<DynamicRow> From(string tableName)
        {
            var source = new DynamicQueryable<DynamicRow>();
           
            return source.From(tableName);

        }

       

    }
}
