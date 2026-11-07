using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Domain.Uow
{
    public interface IUnitOfWorkManager
    {

        IUnitOfWork Begin(UnitOfWorkOptions options);

        IUnitOfWork Begin(int? timeout = null);

        IUnitOfWork Current { get; }

    }
}
