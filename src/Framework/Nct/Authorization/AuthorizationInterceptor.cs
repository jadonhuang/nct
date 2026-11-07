using Castle.DynamicProxy;

namespace Nct.Authorization
{
   
    public class AuthorizationInterceptor : IInterceptor
    {
        private readonly IAuthorizationChecker  _authorizationChecker;

        public AuthorizationInterceptor(IAuthorizationChecker authorizationChecker)
        {
            _authorizationChecker = authorizationChecker;
        }

        public void Intercept(IInvocation invocation)
        {
            _authorizationChecker.Authorize(invocation.MethodInvocationTarget, invocation.TargetType);
            invocation.Proceed();
        }
    }
}
