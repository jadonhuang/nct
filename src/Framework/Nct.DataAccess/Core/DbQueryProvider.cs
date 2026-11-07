using Dapper;
using Nct.Json;
using Nct.LinqQuery;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Nct.DataAccess.Core
{
    public class DbQueryProvider : QueryProvider
    {


        private readonly DbConnection _dbConnection;
        private readonly DbTransaction _dbTransaction;
        private readonly int? _timeOut;

        public DbQueryProvider(DbContext dbContext)
        {
            _dbConnection = dbContext.Database.Connection;
            _dbTransaction = dbContext.Database.Transaction;
            _timeOut = dbContext.Database.Timeout;
        }



        public override object Execute(Expression expression)
        {

            var (sqlFormatter, sqlInfo, sql) = GetSqlInfo(expression);
            var commandInfo = new DbCommandInfo(sql, sqlInfo.Parameters);
            DbInterception.Executing(commandInfo);
            try
            {

                if (sqlInfo.Type == SqlType.Select)
                {

                    var (type, isSingle) = GetReturnTypeInfo(expression);

                    if (isSingle)
                        return _dbConnection.QueryFirstOrDefault(type, sql, param: sqlInfo.Parameters, transaction: _dbTransaction, commandTimeout: _timeOut);

                    return _dbConnection.Query(type, sql, param: sqlInfo.Parameters, transaction: _dbTransaction, commandTimeout: _timeOut);

                }

                if (sqlInfo.Type == SqlType.Insert)
                {
                    return Insert(sqlFormatter, sqlInfo, sql);
                }

                return _dbConnection.Execute(sql, sqlInfo.Parameters, transaction: _dbTransaction, commandTimeout: _timeOut);


            }
            catch (Exception ex)
            {
                commandInfo.Exception = ex;
                throw;

            }
            finally
            {
                DbInterception.Executed(commandInfo);

            }
        }

        public override async Task<TResult> ExecuteAsync<TResult>(Expression expression)
        {

            var (sqlFormatter, sqlInfo, sql) = GetSqlInfo(expression);
            var commandInfo = new DbCommandInfo(sql, sqlInfo.Parameters);
            DbInterception.Executing(commandInfo);

            try
            {
                if (sqlInfo.Type == SqlType.Select)
                {
                    var (type, isSingle) = GetReturnTypeInfo(expression);
                    if (isSingle)
                    {
                        return (TResult)await _dbConnection.QueryFirstOrDefaultAsync(type, sql, param: sqlInfo.Parameters, transaction: _dbTransaction, commandTimeout: _timeOut);
                    }

                    return (TResult)await _dbConnection.QueryAsync(type, sql, param: sqlInfo.Parameters, transaction: _dbTransaction, commandTimeout: _timeOut);

                }

                if (sqlInfo.Type == SqlType.Insert)
                {
                    return (TResult)await InsertAsync(sqlFormatter, sqlInfo, sql);
                }

                return (TResult)(object)await _dbConnection.ExecuteAsync(sql, sqlInfo.Parameters, transaction: _dbTransaction, commandTimeout: _timeOut);
            }
            catch (Exception ex)
            {
                commandInfo.Exception = ex;
                throw;

            }
            finally
            {
                DbInterception.Executed(commandInfo);

            }

        }



        private (SqlFormatter sqlFormatter, SqlInfo sqlInfo, string sql) GetSqlInfo(Expression expression)
        {

            var sqlFormatter = new MsSqlFormatter();
#if DEBUG
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

#endif
            var sqlInfo = new QueryBuilder(expression, sqlFormatter).Build();
            var sql = sqlFormatter.FormatSql(sqlInfo);

#if DEBUG

            Console.WriteLine(sql);
            Console.WriteLine(sqlInfo.Parameters.ToJson());
            stopwatch.Stop();
            Console.WriteLine("linq用时：" + stopwatch.ElapsedMilliseconds + "ms");
            Console.WriteLine();
#endif

            return (sqlFormatter, sqlInfo, sql);


        }


        private (Type resultType, bool isSingle) GetReturnTypeInfo(Expression expression)
        {

            var type = expression.Type;
            bool isSingle = true;

            if (type.IsGenericType)
            {

                var isEnumerableOfT = type.GetGenericTypeDefinition() == typeof(IEnumerable<>) || type.GetInterfaces().Any(ti => ti.IsGenericType
                   && ti.GetGenericTypeDefinition() == typeof(IEnumerable<>));

                isSingle = !isEnumerableOfT;


                type = type.GenericTypeArguments[0];


            }

            return (type, isSingle);

        }

        private (DynamicParameters param, Type entityType, EntityField identityCol, string idParam) GetInsertInfo(SqlFormatter sqlFormatter, SqlInfo sqlInfo, ref string sql)
        {

            var param = new DynamicParameters();
            var entityType = sqlInfo.Entity.GetType();
            var identityCol = EntityCache.GetOrAdd(entityType).Fields.FirstOrDefault(m => m.IsIdentity);
            var idParam = "";
            if (identityCol != null)
            {
                sql += $" ;{sqlFormatter.FormatIdentity()}";
                int index = 0;
                foreach (var key in sqlInfo.Parameters.Keys)
                {

                    if (index++ == sqlInfo.Parameters.Keys.Count - 1)
                    {
                        idParam = key;
                        param.Add(key, dbType: DbType.Int64, direction: ParameterDirection.Output);
                    }
                    else
                    {
                        param.Add(key, sqlInfo.Parameters[key]);
                    }
                }

            }

            return (param, entityType, identityCol, idParam);

        }



        private object Insert(SqlFormatter sqlFormatter, SqlInfo sqlInfo, string sql)
        {

            var (param, entityType, identityCol, idParam) = GetInsertInfo(sqlFormatter, sqlInfo, ref sql);

            if (identityCol != null)
            {
                object affectRows = _dbConnection.Execute(sql, param, transaction: _dbTransaction, commandTimeout: _timeOut);
                var idValue = Convert.ChangeType(param.Get<long>(idParam), identityCol.Type);
                entityType.GetProperty(identityCol.Name).SetValue(sqlInfo.Entity, idValue);
                return affectRows;
            }

            return _dbConnection.Execute(sql, sqlInfo.Parameters, transaction: _dbTransaction, commandTimeout: _timeOut);

        }



        private async Task<object> InsertAsync(SqlFormatter sqlFormatter, SqlInfo sqlInfo, string sql)
        {

            var (param, entityType, identityCol, idParam) = GetInsertInfo(sqlFormatter, sqlInfo, ref sql);

            if (identityCol != null)
            {
                var affectRows = await _dbConnection.ExecuteAsync(sql, param, transaction: _dbTransaction, commandTimeout: _timeOut);
                var idValue = Convert.ChangeType(param.Get<long>(idParam), identityCol.Type);
                entityType.GetProperty(identityCol.Name).SetValue(sqlInfo.Entity, idValue);
                return affectRows;
            }

            return await _dbConnection.ExecuteAsync(sql, sqlInfo.Parameters, transaction: _dbTransaction, commandTimeout: _timeOut);

        }


    }




}
