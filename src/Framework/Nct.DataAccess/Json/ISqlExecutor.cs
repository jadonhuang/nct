using Nct.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.DataAccess.Json
{
    public interface ISqlExecutor : ITransientDependency
    {


        Task Begin();
        Task End(Exception ex);
        Task<List<dynamic>> Query(string sql, Dictionary<string, object> parameters);
        Task<int> Execute(string sql, Dictionary<string, object> parameters);

    }
}
