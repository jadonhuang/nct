using Nct.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.DataAccess.Core
{
    public interface IDbCommandInterceptor
    {
        void Executing(DbCommandInfo dbCommandInfo);

        void Executed(DbCommandInfo dbCommandInfo);
    }
}
