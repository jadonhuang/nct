using Nct.LinqQuery;
using System;
using System.Data.Common;
using System.Data.SqlClient;

namespace Nct.DataAccess.Core
{
    public class DbContext : IDisposable
    {


        public Database Database { get; }


        public virtual Query<T> Table<T>()
        {

            var query = new Query<T>(new DbQueryProvider(this));
            return ApplyFilters(query);

        }

        protected virtual Query<T> ApplyFilters<T>(Query<T> query)
        {

            return query;
        }


        public virtual void Dispose()
        {
            if (Database.Connection.State == System.Data.ConnectionState.Open)
                Database.Connection.Close();
        }

        public DbContext(string connectionString)
        {

            DbConnection conn = new SqlConnection(connectionString);
            Database = new Database(conn);

        }


        public DbContext(DbConnection existingConnection)
        {

            Database = new Database(existingConnection);

        }

    }
}
