using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Async
{
    public abstract class InternalAsyncInterceptor : IAsyncInterceptor
    {
        public virtual void InterceptAsynchronous(IInvocation invocation)
        {
            invocation.ReturnValue = InternalInterceptAsynchronous(invocation);
        }

        public virtual void InterceptAsynchronous<TResult>(IInvocation invocation)
        {
            invocation.ReturnValue = InternalInterceptAsynchronous<TResult>(invocation);
        }

        public abstract void InterceptSynchronous(IInvocation invocation);

        protected abstract Task InternalInterceptAsynchronous(IInvocation invocation);

        protected abstract Task<TResult> InternalInterceptAsynchronous<TResult>(IInvocation invocation);
    }
}
