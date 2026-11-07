using Nct.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Nct.Domain.Uow
{
    public class InnerUnitOfWork : BaseUnitOfWork
    {

        public override IUnitOfWork Begin(UnitOfWorkOptions options)
        {
            return base.Begin(options);
        }

    }
}
