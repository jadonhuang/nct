using Nct.Domain.Uow;
using Nct.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.DataAccess.Json
{
    public class DapperSqlExecutor : ISqlExecutor
    {
        private readonly IDbContextProvider _dbContextProvider;
        public DapperSqlExecutor(IDbContextProvider dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;
            
        }

        public Task Begin()
        {
            return Task.CompletedTask;
        }

        public Task End(Exception ex)
        {
            return Task.CompletedTask;
        }

        public async Task<int> Execute(string sql, Dictionary<string, object> parameters)
        {
#if DEBUG
            Console.WriteLine(sql);
            Console.WriteLine(parameters.ToJson());
            Console.WriteLine();
#endif
            return await _dbContextProvider.GetDbContext<DapperDbContext>().Database.ExecuteAsync(sql, parameters);
        }




      
        public async Task<List<dynamic>> Query(string sql, Dictionary<string, object> parameters)
        {

#if DEBUG
            Console.WriteLine(sql);
            Console.WriteLine(parameters.ToJson());
            Console.WriteLine();
#endif


            var db = _dbContextProvider.GetDbContext<DapperDbContext>().Database;
            if(db.Connection.State!= System.Data.ConnectionState.Open) db.Connection.Open();

            var rows= await db.QueryAsync<dynamic>(sql, parameters);
            HandleResult(rows);
            return rows;

        }


        private void HandleResult(List<dynamic> rows)
        {
            foreach (IDictionary<string, object> row in rows)
            {
                row.Remove(SqlHelper.RowIndexName);
            }
        
        }
    }
}
