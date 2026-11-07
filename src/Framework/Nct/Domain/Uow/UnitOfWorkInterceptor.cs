using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Nct.Async;
using Nct.Ioc;

namespace Nct.Domain.Uow
{
    public class UnitOfWorkInterceptor : InternalAsyncInterceptor, ITransientDependency
    {
        private IUnitOfWorkManager _uowManager;
        public UnitOfWorkInterceptor(IUnitOfWorkManager uowManager)
        {
            _uowManager = uowManager;

        }

        private static MethodInfo GetMethodInfo(IInvocation invocation)
        {
            MethodInfo method;
            try
            {
                method = invocation.MethodInvocationTarget;
            }
            catch
            {
                method = invocation.GetConcreteMethod();
            }

            return method;
        }


        public override void InterceptSynchronous(IInvocation invocation)
        {

            var method = GetMethodInfo(invocation);

            if (!UnitOfWorkHelper.IsUnitOfWorkMethod(method, out var unitOfWorkAttr))
            {
                invocation.Proceed();
                return;

            }

            var options = UnitOfWorkHelper.CreateOptions(unitOfWorkAttr, method.Name);

            using (var uow = _uowManager.Begin(options))
            {
                invocation.Proceed();
                uow.Complete();
            }


        }

        protected override async Task InternalInterceptAsynchronous(IInvocation invocation)
        {

            var proceedInfo = invocation.CaptureProceedInfo();
            var method = GetMethodInfo(invocation);

            if (!UnitOfWorkHelper.IsUnitOfWorkMethod(method, out var unitOfWorkAttr))
            {
                proceedInfo.Invoke();
                var task = (Task)invocation.ReturnValue;
                await task;
                return;

            }

            var options = UnitOfWorkHelper.CreateOptions(unitOfWorkAttr, method.Name);

            using (var uow = _uowManager.Begin(options))
            {
                proceedInfo.Invoke();
                var task = (Task)invocation.ReturnValue;
                await task;
                uow.Complete();
            }
        }

        protected override async Task<TResult> InternalInterceptAsynchronous<TResult>(IInvocation invocation)
        {

            var proceedInfo = invocation.CaptureProceedInfo();
            var method = GetMethodInfo(invocation);

            if (!UnitOfWorkHelper.IsUnitOfWorkMethod(method, out var unitOfWorkAttr))
            {
                proceedInfo.Invoke();
                var taskResult = (Task<TResult>)invocation.ReturnValue;
                var result = await taskResult;
                return result;

            }

          var options = UnitOfWorkHelper.CreateOptions(unitOfWorkAttr, method.Name);

            using (var uow = _uowManager.Begin(options))
            {
                proceedInfo.Invoke();
                var taskResult = (Task<TResult>)invocation.ReturnValue;
                var result = await taskResult;
                uow.Complete();
                return result;
            }
        }


    }
}
