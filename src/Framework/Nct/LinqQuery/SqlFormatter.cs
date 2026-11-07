using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;

namespace Nct.LinqQuery
{
    public abstract class SqlFormatter
    {

        private Dictionary<Type, string> tableAlias = [];

        public Dictionary<string, object> Parameters { get; } = [];



        public virtual string OpenQuote { get; } = "[";

        public virtual string CloseQuote { get; } = "]";

        public virtual string ParameterPrefix { get; } = "@p";



        public virtual string AddParam(object value)
        {
            string name = $"{ParameterPrefix}{Parameters.Keys.Count}";
            Parameters[name] = value;
            return name;
        }

        public virtual string FormatColumnName(string name)
        {

            return $"{OpenQuote}{name}{CloseQuote}";

        }


        public virtual string FormatQualifiedColumnName(Type type, string name)
        {
            var entityObj = EntityCache.GetOrAdd(type);
            var colName = entityObj.Fields.First(m => m.Name == name).ColumnName;
            return $"{GetOrSetTableNameAlias(type)}.{FormatColumnName(colName)}";

        }


        public virtual string GetOrSetTableNameAlias(Type type)
        {


            if (!tableAlias.ContainsKey(type))
            {
                tableAlias[type] = "t" + tableAlias.Keys.Count;

            }

            return tableAlias[type];

        }


        public virtual string FormatTableName(Type type)
        {

            return $"{OpenQuote}{EntityCache.GetOrAdd(type).TableName}{CloseQuote}";

        }




        public virtual string FormatTableNameAsAlias(Type type)
        {

            return $"{FormatTableName(type)} AS {GetOrSetTableNameAlias(type)}";

        }



        public abstract void FormatMethod(BaseTranslator translator, MethodCallExpression expression);


        public abstract void FormatMember(BaseTranslator translator, MemberExpression expression);


        public abstract void FormatAggregate(BaseTranslator translator, string mothodName, string columnName);



        public virtual string FormatInsertSql(SqlInfo sqlInfo)
        {

            Parameters.Clear();
            StringBuilder sbCols = new StringBuilder();
            StringBuilder sbParams = new StringBuilder();
            sbCols.AppendLine();
            sbParams.AppendLine();
            var entityType = sqlInfo.Entity.GetType();
            var entity = EntityCache.GetOrAdd(entityType);


            foreach (var field in entity.Fields)
            {

                if (field.IsIdentity) continue;

                var value = entityType.GetProperty(field.Name).GetValue(sqlInfo.Entity, null);

                sbCols.AppendLine($" {FormatColumnName(field.ColumnName)},");
                sbParams.AppendLine($" {AddParam(value)},");


            }

            var tailLen = Environment.NewLine.Length + 1;

            return $"INSERT INTO {FormatColumnName(entity.TableName)}({sbCols.ToString().Substring(0, sbCols.Length - tailLen)}) VALUES ({sbParams.ToString().Substring(0, sbParams.Length - tailLen)})";


        }

        public virtual string FormatDeleteSql(SqlInfo sqlInfo)
        {

            string table = sqlInfo.Join != null ? sqlInfo.Join : sqlInfo.Table;


            if (sqlInfo.Entity != null)
            {
                sqlInfo.Where = null;
                sqlInfo.Parameters.Clear();
                var entityType = sqlInfo.Entity.GetType();
                var entity = EntityCache.GetOrAdd(entityType);

                foreach (var field in entity.Fields)
                {
                    if (field.IsKey)
                    {
                        if (sqlInfo.Where != null)
                            sqlInfo.Where += " AND ";
                        else sqlInfo.Where = " WHERE ";


                        var value = entityType.GetProperty(field.Name).GetValue(sqlInfo.Entity, null);
                        sqlInfo.Where += $"{FormatQualifiedColumnName(entityType, field.ColumnName)} = {AddParam(value)}";


                    }

                }


            }

            return $"DELETE {sqlInfo.TableAlias} FROM {table} {sqlInfo.Where}";
        }

        public abstract string FormatSelectSql(SqlInfo sqlInfo);


        public virtual string FormatUpdateSql(SqlInfo sqlInfo)
        {


            if (sqlInfo.Entity != null)
            {
                StringBuilder sbCols = new StringBuilder();
                StringBuilder sbWhere = new StringBuilder();
                sbCols.AppendLine();

                var entityType = sqlInfo.Entity.GetType();
                var entity = EntityCache.GetOrAdd(entityType);
                sqlInfo.Where = null;
                sqlInfo.Parameters.Clear();

                foreach (var field in entity.Fields)
                {


                    var value = entityType.GetProperty(field.Name).GetValue(sqlInfo.Entity, null);

                    if (field.IsKey)
                    {

                        if (sbWhere.Length > 0)
                            sbWhere.Append(" AND ");

                        sbWhere.Append($"{FormatColumnName(field.ColumnName)} = {AddParam(value)}");
                    }
                    else
                    {
                        if (!field.IsIdentity)
                        {
                            sbCols.AppendLine($"  {FormatColumnName(field.ColumnName)} = {AddParam(value)},");
                        }
                    }


                }


                var tailLen = Environment.NewLine.Length + 1;

                return $"UPDATE {FormatColumnName(entity.TableName)} SET {sbCols.ToString().Substring(0, sbCols.Length - tailLen)}{Environment.NewLine}WHERE {sbWhere}";

            }


            return $"UPDATE {sqlInfo.TableAlias} SET {sqlInfo.Select} FROM {(sqlInfo.Join == null ? sqlInfo.Table : sqlInfo.Join)} {sqlInfo.Where}";


        }

        public virtual string FormatSql(SqlInfo sqlInfo)
        {

            switch (sqlInfo.Type)
            {
                case SqlType.Delete:
                    return FormatDeleteSql(sqlInfo);
                case SqlType.Update:
                    return FormatUpdateSql(sqlInfo);
                case SqlType.Insert:
                    return FormatInsertSql(sqlInfo);
                default:
                    return FormatSelectSql(sqlInfo);

            }

        }

        public abstract string FormatIdentity();




    }
}
