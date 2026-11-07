using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Domain.Uow
{
    public interface IUnitOfWork : IDisposable
    {
        IUnitOfWork Begin(UnitOfWorkOptions options);

        void Complete();

        event EventHandler OnDisposed;

        bool IsDisposed { get; }

        UnitOfWorkOptions Options { get; }

    }
}
