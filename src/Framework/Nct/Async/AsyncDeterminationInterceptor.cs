using Castle.DynamicProxy;

namespace Nct.Async
{
    public class AsyncDeterminationInterceptor<TInterceptor> : AsyncDeterminationInterceptor
        where TInterceptor : IAsyncInterceptor
    {
        public AsyncDeterminationInterceptor(TInterceptor asyncInterceptor) : base(asyncInterceptor)
        {

        }
    }
}