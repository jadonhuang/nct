using Dapper;
using Nct.Application;
using Nct.Extentions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.DataAccess.Core
{
    public class Database
    {


        public DbTransaction Transaction { get; private set; }

        public DbConnection Connection
        {
            get;
            private set;
        }

        public int? Timeout { get; set; }

        public virtual void UseTransaction(DbTransaction transaction)
        {
            Connection = transaction.Connection;
            Transaction = transaction;
        }

        public virtual DbTransaction BeginTransaction()
        {
            return BeginTransaction(IsolationLevel.ReadCommitted);
        }

        public virtual DbTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            if (Connection.State != ConnectionState.Open)
                Connection.Open();

            Transaction = Connection.BeginTransaction(isolationLevel);
            return Transaction;
        }


        public Database(DbConnection dbConnection)
        {

            Connection = dbConnection;
        }



        public virtual async Task<List<T>> QueryAsync<T>(string sql, object param = null, int? commandTimeout = null)
        {
            var dbCommand = new DbCommandInfo(sql, param);

            DbInterception.Executing(dbCommand);

            try
            {

                return (await Connection.QueryAsync<T>(sql, param, Transaction, commandTimeout)).ToList();

            }
            catch (Exception ex)
            {
                dbCommand.Exception = ex;
                throw;

            }
            finally
            {

                DbInterception.Executed(dbCommand);

            }

        }


        public virtual async Task<int> ExecuteAsync(string sql, object param = null, int? commandTimeout = null)
        {

            var dbCommand = new DbCommandInfo(sql, param);

            DbInterception.Executing(dbCommand);

            try
            {

                return await Connection.ExecuteAsync(sql, param, Transaction, commandTimeout);

            }
            catch (Exception ex)
            {
                dbCommand.Exception = ex;
                throw;

            }
            finally
            {

                DbInterception.Executed(dbCommand);

            }



        }




        public virtual List<T> Query<T>(string sql, object param = null, int? commandTimeout = null)
        {
            var dbCommand = new DbCommandInfo(sql, param);

            DbInterception.Executing(dbCommand);

            try
            {

                return Connection.Query<T>(sql, param, Transaction, commandTimeout: commandTimeout).ToList();

            }
            catch (Exception ex)
            {
                dbCommand.Exception = ex;
                throw;

            }
            finally
            {

                DbInterception.Executed(dbCommand);

            }

        }


        public virtual int Execute(string sql, object param = null, int? commandTimeout = null)
        {

            var dbCommand = new DbCommandInfo(sql, param);

            DbInterception.Executing(dbCommand);

            try
            {

                return Connection.Execute(sql, param, Transaction, commandTimeout);

            }
            catch (Exception ex)
            {
                dbCommand.Exception = ex;
                throw;

            }
            finally
            {

                DbInterception.Executed(dbCommand);

            }
        }


        public virtual async Task<T> ExecuteScalarAsync<T>(string sql, object param = null, int? commandTimeout = null)
        {

            var dbCommand = new DbCommandInfo(sql, param);

            DbInterception.Executing(dbCommand);

            try
            {

                return await Connection.ExecuteScalarAsync<T>(sql, param, Transaction, commandTimeout);

            }
            catch (Exception ex)
            {
                dbCommand.Exception = ex;
                throw;

            }
            finally
            {

                DbInterception.Executed(dbCommand);

            }
        }


        public virtual T ExecuteScalar<T>(string sql, object param = null, int? commandTimeout = null)
        {

            var dbCommand = new DbCommandInfo(sql, param);

            DbInterception.Executing(dbCommand);

            try
            {

                return Connection.ExecuteScalar<T>(sql, param, Transaction, commandTimeout);

            }
            catch (Exception ex)
            {
                dbCommand.Exception = ex;
                throw;

            }
            finally
            {

                DbInterception.Executed(dbCommand);

            }
        }




        public virtual async Task<DataTable> QueryTableAsync(string sql, object param = null, int? commandTimeout = null)
        {
            var dbCommand = new DbCommandInfo(sql, param);

            DbInterception.Executing(dbCommand);
            var dt = new DataTable("dt");
            try
            {

                var reader = await Connection.ExecuteReaderAsync(sql, param, Transaction, commandTimeout);
                dt.Load(reader);
                return dt;
            }
            catch (Exception ex)
            {
                dbCommand.Exception = ex;
                throw;

            }
            finally
            {

                DbInterception.Executed(dbCommand);

            }

        }

        private static StringBuilder SqlPageSql(string strSql, IPageQuery pageQuery)
        {

            var sb = new StringBuilder();
            var pageIndex = pageQuery.PageIndex;
            var pageSize = pageQuery.PageSize;

            int startIndex = (pageIndex - 1) * pageSize;
            int endIndex = startIndex + pageSize;
            string orderBy = pageQuery.OrderBy ?? "(select 0)";
            orderBy = $"order by {orderBy}";

            if (pageSize > 0)
            {
                var rowIndex = SqlHelper.RowIndexName;
                sb.Append("Select * From (Select ROW_NUMBER() Over (" + orderBy + ")");
                sb.Append($" As {rowIndex} , * From ({strSql}) T ) As N Where {rowIndex} > {startIndex} And {rowIndex} <= {endIndex}");
            }
            else
            {
                sb.Append(strSql + " " + orderBy);

            }
            return sb;

        }




        public virtual async Task<PageTableResult> QueryTableAsync(string sql, IPageQuery pageQuery, object param = null, int? commandTimeout = null)
        {


            var pageTableResult = new PageTableResult();
            var pageSql = SqlPageSql(sql, pageQuery).ToString();

            var dt = await QueryTableAsync(pageSql, param);
            pageTableResult.Rows = dt;

            var totalSql = $"select count(1) as num from ({sql}) t";

            int toal = await ExecuteScalarAsync<int>(totalSql, param, commandTimeout);

            pageTableResult.Total = toal;

            return pageTableResult;

        }


        public virtual PageTableResult QueryTable(string sql, IPageQuery pageQuery, object param = null, int? commandTimeout = null)
        {


            var pageTableResult = new PageTableResult();
            var pageSql = SqlPageSql(sql, pageQuery).ToString();

            var dt = QueryTable(pageSql, param);
            pageTableResult.Rows = dt;

            var totalSql = $"select count(1) as num from ({sql}) t";

            int toal = ExecuteScalar<int>(totalSql, param, commandTimeout);

            pageTableResult.Total = toal;

            return pageTableResult;

        }



        public virtual DataTable QueryTable(string sql, object param = null, int? commandTimeout = null)
        {

            var dbCommand = new DbCommandInfo(sql, param);

            DbInterception.Executing(dbCommand);
            var dt = new DataTable("dt");
            try
            {

                var reader = Connection.ExecuteReader(sql, param, Transaction, commandTimeout);
                dt.Load(reader);
                return dt;
            }
            catch (Exception ex)
            {
                dbCommand.Exception = ex;
                throw;

            }
            finally
            {

                DbInterception.Executed(dbCommand);

            }

        }



        public virtual int Insert<T>(IEnumerable<T> list, string targetTable = null, int batchSize = 1000, int? commandTimeout = null)
        {

            DataTable dt = list.ToDataTable();
    
            return Insert(dt, targetTable, batchSize, commandTimeout);

        }

        public virtual int Insert(DataTable dataTable, string targetTable = null, int batchSize = 1000, int? commandTimeout = null)
        {

            targetTable ??= dataTable.TableName;

            ArgumentNullException.ThrowIfNull(nameof(dataTable));

            ArgumentNullException.ThrowIfNull(nameof(targetTable));

            if (dataTable.Rows.Count == 0)
                throw new ArgumentException("dataTable is empty");

            using (var conn = (SqlConnection)Connection)
            {
                conn.Open();
                using (var sqlBulkCopy = new SqlBulkCopy(conn))
                {
                    sqlBulkCopy.DestinationTableName = targetTable;
                    sqlBulkCopy.BatchSize = batchSize;
                    if (commandTimeout.HasValue)
                    {
                        sqlBulkCopy.BulkCopyTimeout = commandTimeout.Value;
                    }

                    sqlBulkCopy.WriteToServer(dataTable);

                }
            }

            return dataTable.Rows.Count;

        }


    }
}
