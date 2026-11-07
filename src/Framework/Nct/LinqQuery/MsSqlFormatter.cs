using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace Nct.LinqQuery
{
    public class MsSqlFormatter : SqlFormatter
    {
        public override void FormatAggregate(BaseTranslator translator, string mothodName, string columnName)
        {
            switch (mothodName)
            {
                case "Count":
                    translator.Write("COUNT(1)");
                    break;
                case "LongCount":
                    translator.Write("COUNT_BIG(1)");
                    break;

                case "Average":
                    translator.Write($"ISNULL(AVG({columnName}),0)");
                    break;
                default:
                    translator.Write($"ISNULL({mothodName.ToUpper()}({columnName}),0)");
                    break;

            }
        }

        public override string FormatIdentity()
        {
            return $"SELECT {AddParam(0)}=SCOPE_IDENTITY()";
        }

        public override void FormatMember(BaseTranslator translator, MemberExpression m)
        {
            if (m.Member.DeclaringType == typeof(string))
            {
                switch (m.Member.Name)
                {
                    case "Length":
                        translator.Write("LEN(");
                        translator.Visit(m.Expression);
                        translator.Write(")");
                        break;
                }
            }
            else if (m.Member.DeclaringType == typeof(DateTime) || m.Member.DeclaringType == typeof(DateTimeOffset))
            {
                switch (m.Member.Name)
                {
                    case "Day":
                        translator.Write("DAY(");
                        translator.Visit(m.Expression);
                        translator.Write(")");
                        break;
                    case "Month":
                        translator.Write("MONTH(");
                        translator.Visit(m.Expression);
                        translator.Write(")");
                        break;
                    case "Year":
                        translator.Write("YEAR(");
                        translator.Visit(m.Expression);
                        translator.Write(")");
                        break;
                    case "Hour":
                        translator.Write("DATEPART(hour, ");
                        translator.Visit(m.Expression);
                        translator.Write(")");
                        break;
                    case "Minute":
                        translator.Write("DATEPART(minute, ");
                        translator.Visit(m.Expression);
                        translator.Write(")");
                        break; ;
                    case "Second":
                        translator.Write("DATEPART(second, ");
                        translator.Visit(m.Expression);
                        translator.Write(")");
                        break;
                    case "Millisecond":
                        translator.Write("DATEPART(millisecond, ");
                        translator.Visit(m.Expression);
                        translator.Write(")");
                        break;
                    case "DayOfWeek":
                        translator.Write("(DATEPART(weekday, ");
                        translator.Visit(m.Expression);
                        translator.Write(") - 1)");
                        break;
                    case "DayOfYear":
                        translator.Write("(DATEPART(dayofyear, ");
                        translator.Visit(m.Expression);
                        translator.Write(") - 1)");
                        break;
                }
            }

        }

        public override void FormatMethod(BaseTranslator translator, MethodCallExpression m)
        {
            var methodName = m.Method.Name;
            switch (methodName)
            {

                case "ToString":
                    translator.Write("CAST(");
                    translator.Visit(m.Object == null ? m.Arguments[0] : m.Object);
                    translator.Write(" as nvarchar)");
                    break;

                case "ToInt32":
                    translator.Write("CAST(");
                    translator.Visit(m.Arguments[0]);
                    translator.Write(" as int)");
                    break;

                case "ToDecimal":
                    translator.Write("CAST(");
                    translator.Visit(m.Arguments[0]);
                    translator.Write(" as decimal(36,19))");
                    break;

                case "ToDouble":
                    translator.Write("CAST(");
                    translator.Visit(m.Arguments[0]);
                    translator.Write(" as decimal(36,16))");
                    break;

                case "ToDateTime":
                    translator.Write("CAST(");
                    translator.Visit(m.Arguments[0]);
                    translator.Write(" as datetime)");
                    break;


                case "Contains":
                    if (m.Arguments.Count == 2)  //extension method   int[].Contains(m.Name)
                    {
                        translator.Visit(m.Arguments[1]);
                        translator.Write(" IN (");
                        translator.Visit(m.Arguments[0]);
                        translator.Write(")");
                    }
                    else if (m.Arguments.Count == 1 && m.Object.Type.IsGenericType) //list etc  List<T>.Contains(m.Name)
                    {
                        translator.Visit(m.Arguments[0]);
                        translator.Write(" IN (");
                        translator.Visit(m.Object);
                        translator.Write(")");
                    }
                    else
                    {
                        translator.Visit(m.Object);
                        translator.Write(" LIKE ");
                        translator.Write("('%'+ ");
                        translator.Visit(m.Arguments[0]);
                        translator.Write(" + '%')");
                    }
                    break;

                case "StartsWith":
                    translator.Visit(m.Object);
                    translator.Write(" LIKE ");
                    translator.Write("(");
                    translator.Visit(m.Arguments[0]);
                    translator.Write(" + '%')");
                    break;

                case "EndsWith":
                    translator.Visit(m.Object);
                    translator.Write(" LIKE ");
                    translator.Write("('%'+ ");
                    translator.Visit(m.Arguments[0]);
                    translator.Write(")");
                    break;
                case "IsNullOrEmpty":
                    translator.Write("(");
                    translator.Visit(m.Arguments[0]);
                    translator.Write(" IS NULL OR ");
                    translator.Visit(m.Arguments[0]);
                    translator.Write("='')");
                    break;

                case "Substring":
                    translator.Write("SUBSTRING(");
                    translator.Visit(m.Object);
                    translator.Write(", ");
                    translator.Visit(m.Arguments[0]);
                    translator.Write(" + 1, ");
                    if (m.Arguments.Count == 2)
                    {
                        translator.Visit(m.Arguments[1]);
                    }
                    else
                    {
                        translator.Write("8000");
                    }
                    translator.Write(")");
                    break;

                case "Replace":
                    translator.Write("REPLACE(");
                    translator.Visit(m.Object);
                    translator.Write(", ");
                    translator.Visit(m.Arguments[0]);
                    translator.Write(", ");
                    translator.Visit(m.Arguments[1]);
                    translator.Write(")");
                    break;
                case "IndexOf":
                    translator.Write("(CHARINDEX(");
                    translator.Visit(m.Arguments[0]);
                    translator.Write(", ");
                    translator.Visit(m.Object);
                    if (m.Arguments.Count == 2 && m.Arguments[1].Type == typeof(int))
                    {
                        translator.Write(", ");
                        translator.Visit(m.Arguments[1]);
                        translator.Write(" + 1");
                    }
                    translator.Write(") - 1)");
                    break;
            }
        }

        public override string FormatSelectSql(SqlInfo sqlInfo)
        {
            StringBuilder sb = new StringBuilder();

            string table = sqlInfo.Join == null ? sqlInfo.Table : sqlInfo.Join;

            string lockMode = sqlInfo.LockMode != null ? $"WITH({sqlInfo.LockMode}) " : "";

            sb.Append("SELECT ");

           
            bool hasSelect = true;
            if (sqlInfo.Select == null)
            {
                hasSelect = false;
                var newLine = sqlInfo.SkipNum.HasValue ? "" : Environment.NewLine;
                var tbAlias = GetOrSetTableNameAlias(sqlInfo.EntityType);
                var fields = EntityCache.GetOrAdd(sqlInfo.EntityType).Fields.Select(m => $"{newLine}{tbAlias}.{ FormatColumnName(m.Name)}");
                sqlInfo.Select = string.Join(",", fields);
            }

            if (sqlInfo.SkipNum.HasValue)
            {
                if (string.IsNullOrWhiteSpace(sqlInfo.Order)) throw new ArgumentException("must has orderby");

                var rowIndex = "__ROWINDEX";

                var newLine = hasSelect ? "" : Environment.NewLine;
                string resultFields = Regex.Replace(sqlInfo.Select, "t[\\d]+\\.", $"{newLine}tb.");

                if (sqlInfo.IsDistinct)
                {
                    resultFields = $"DISTINCT {resultFields}";
                }

                sb.Append($"{sqlInfo.Select}, ROW_NUMBER() OVER({sqlInfo.Order}) {rowIndex}");
                sb.Append($" FROM {table} {lockMode}{sqlInfo.Where}");
                sb.Insert(0, $"SELECT {resultFields} FROM (");
                sb.Append(")tb");
                sb.Append($" WHERE {rowIndex} > {sqlInfo.SkipNum}");
                if (sqlInfo.TopNum > 0)
                {
                    sb.Append($" AND {rowIndex} <= {sqlInfo.SkipNum + sqlInfo.TopNum}");
                }

            }
            else
            {
                if (sqlInfo.IsDistinct)
                {
                    sb.Append("DISTINCT ");
                }

                if (sqlInfo.TopNum.HasValue)
                {
                    sb.Append($"TOP {sqlInfo.TopNum} ");
                }

                sb.Append($"{sqlInfo.Select} FROM {table} {lockMode}{sqlInfo.Where} {sqlInfo.GroupBy} ");
                if (!sqlInfo.IsAggregate && sqlInfo.Order != null)
                    sb.Append(sqlInfo.Order);
            }

            return sb.ToString();

        }




    }
}
