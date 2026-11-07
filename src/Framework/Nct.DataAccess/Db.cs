using Nct.Application;
using Nct.Ioc;
using System.Collections.Generic;
using System.Data;

namespace Nct.DataAccess
{
    public static class Db
    {


        public static List<T> Query<T>(string sql, object param = null, int? commandTimeout = null)
        {

            List<T> list = default;
            IocManager.Instance.Using<IDbContextProvider>(ctx =>
            {
                list = ctx.GetDbContext<DapperDbContext>().Database.Query<T>(sql, param, commandTimeout);

            });

            return list;

        }


        public static int Execute(string sql, object param = null, int? commandTimeout = null)
        {

            var num = 0;
            IocManager.Instance.Using<IDbContextProvider>(ctx =>
            {
                num = ctx.GetDbContext<DapperDbContext>().Database.Execute(sql, param, commandTimeout);

            });

            return num;


        }




        public static T ExecuteScalar<T>(string sql, object param = null, int? commandTimeout = null)
        {

            T result = default;
            IocManager.Instance.Using<IDbContextProvider>(ctx =>
            {
                result = ctx.GetDbContext<DapperDbContext>().Database.ExecuteScalar<T>(sql, param, commandTimeout);

            });

            return result;
        }



        public static PageTableResult QueryTable(string sql, IPageQuery pageQuery, object param = null, int? commandTimeout = null)
        {

            PageTableResult result = default;
            IocManager.Instance.Using<IDbContextProvider>(ctx =>
            {
                result = ctx.GetDbContext<DapperDbContext>().Database.QueryTable(sql, pageQuery, param, commandTimeout);

            });

            return result;

        }



        public static DataTable QueryTable(string sql, object param = null, int? commandTimeout = null)
        {

            DataTable result = default;
            IocManager.Instance.Using<IDbContextProvider>(ctx =>
            {
                result = ctx.GetDbContext<DapperDbContext>().Database.QueryTable(sql, param, commandTimeout);

            });

            return result;

        }




    }
}
