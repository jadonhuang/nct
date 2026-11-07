using Nct.DataAccess.Core;
using System;
using System.Collections.Generic;
using System.Text;
using Nct.Ioc;
using Nct.LinqQuery;
using Nct.Domain.Entities;
using Nct.Extentions;
using Nct.Domain.Filters;

namespace Nct.DataAccess
{
    public class DapperDbContext : DbContext
    {

        public DapperDbContext(string connectionString) : base(connectionString)
        {



        }


        protected override Query<T> ApplyFilters<T>(Query<T> query)
        {

            if (typeof(ISoftDelete).IsAssignableFrom(typeof(T)))
            {
                bool enabled = IocManager.Instance.Resolve<IDataFilter<ISoftDelete>>().IsEnabled;
                if (enabled)
                {
                    query = (Query<T>)query.WhereIf(m => ((ISoftDelete)m).IsDeleted.Equals(0));
                }

            }
            return query;

        }


    }
}
