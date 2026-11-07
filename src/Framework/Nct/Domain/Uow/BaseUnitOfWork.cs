using System;

namespace Nct.Domain.Uow
{
    public abstract class BaseUnitOfWork : IUnitOfWork
    {


        public event EventHandler OnDisposed;

        private bool isDisposed = false;

        public bool IsDisposed => isDisposed;


        public UnitOfWorkOptions Options { get; private set; }

        public virtual IUnitOfWork Begin(UnitOfWorkOptions options)
        {
            Options = options;
            return this;
        }

        public virtual void Complete()
        {
           
        }

        public virtual void Dispose()
        {
            if (isDisposed) return;

            isDisposed = true;
            if (OnDisposed != null)
            {
                OnDisposed.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
