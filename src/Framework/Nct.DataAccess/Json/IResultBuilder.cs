using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nct.Ioc;

namespace Nct.DataAccess.Json
{
    public interface IResultBuilder : ITransientDependency
    {

        Task<IDictionary<string, object>> Build(string json, StatementType statementType);

    }
}
