using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.DataAccess.DynamicQuery
{
   public class Constants
    {

        public static class SqlOperator
        {
            public const  string Equal = "=";

            public const string NotEqual = "<>";

            public const string GreaterThan = ">";

            public const string GreaterThanOrEqual = ">=";

            public const string LessThan = "<";

            public const string LessThanOrEqual = "<=";

            public const string Like = "LIKE";

            public const string NotLike = "NOT LIKE";

            public const string In = "IN";

            public const string NotIn = "NOT IN";

            public const string AndAlso = "AND";

            public const string OrElse = "OR";

            public const string Add = "+";

            public const string Subtract = "-";

            public const string Multiply = "*";

            public const string Divide = "/";

            public const string IsNull = "IS NULL";

            public const string IsNotNull = "IS NOT NULL";

        }


        public static class LinqFunction
        {
            public const string Max = "Max";

            public const string Min = "Min";

            public const string  Average = "Average";

            public const string Sum = "Sum";

            public const string OrderByDesc = "OrderByDescending";

            public const string OrderBy = "OrderBy";

            public const string FirstOrDefault = "FirstOrDefault";

            public const string Take = "Take";

            public const string Count = "Count";

            public const string Skip = "Skip";

            public const string GetItem = "get_Item";

            public const string Contains = "Contains";

            public const string StartsWith = "StartsWith";

            public const string EndsWith = "EndsWith";

            public const string ToList = "ToList";

            public const string Where = "Where";

            public const string Not = "Not";

            public const string Delete = "Delete";

            public const string DeleteRows = "DeleteRows";

            public const string Update = "Update";

            public const string Insert = "Insert";

            public const string From = "From";

            public const string Fields = "Fields";

            public const string Replace = "Replace";

            public const string WithAudit = "WithAudit";

        }


        public static class SqlKeyword
        {
            public const string OrderBy = "ORDER BY";

            public const string Asc = "ASC";

            public const string Desc = "DESC";

            public const string Max = "MAX";

            public const string Min = "MIN";

            public const string Avarage = "AVG";

            public const string Sum = "SUM";

            public const string Select = "SELECT";

            public const string Update = "UPDATE";

            public const string Delete = "DELETE";

            public const string Count = "COUNT";

            public const string From = "FROM";

            public const string Where = "WHERE";

            public const string Top = "TOP";

            public const string Insert = "INSERT";

            public const string Replace = "REPLACE";

            public const string RowNumber = "ROW_NUMBER()";

            public const string Over = "OVER";





        }


       


    }
}
