using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nct.Domain.Uow;
using Nct.Ioc;
using System.Threading.Tasks;

namespace Nct.AspNetCore.Filters
{
    public class UowActionFilter : IAsyncActionFilter, ITransientDependency
    {

        private IUnitOfWorkManager _uowManager;
        public UowActionFilter(IUnitOfWorkManager uowManager)
        {
            _uowManager = uowManager;

        }


        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            if (!context.ActionDescriptor.IsControllerAction())
            {
                await next();
                return;
            }

            var methodInfo = context.ActionDescriptor.GetMethodInfo();
            if (methodInfo == null)
            {
                await next();
                return;
            }

            var unitOfWorkAttr = UnitOfWorkHelper.GetUnitOfWorkAttributeOrNull(methodInfo);
            if (unitOfWorkAttr?.Enable == false)
            {
                await next();
                return;
            }


            var options = UnitOfWorkHelper.CreateOptions(unitOfWorkAttr, context.HttpContext.Request.Method);

            using (var uow = _uowManager.Begin(options))
            {
                var executedContext = await next();

                if (executedContext.Exception == null || executedContext.ExceptionHandled)
                {

                    var feature = context.HttpContext.Features.Get<IExceptionHandlerFeature>();
                    var error = feature?.Error;
                    if (error != null) return;

                    var result = executedContext.Result;

                    if (result is ObjectResult objResult)
                    {
                        var value = objResult.Value;
                        if (value is RetResult ret && !ret.IsSuccess)
                        {
                            return;
                        }
                    }

                    uow.Complete();


                }
            }



        }





    }
}
