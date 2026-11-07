using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Nct.Domain.Uow;
using Nct.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Nct.AspNetCore.Filters
{
    public class UowPageFilter : IAsyncPageFilter, ITransientDependency
    {

        public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {

            if (context.HandlerMethod == null || context.ActionDescriptor is not PageActionDescriptor)
            {
                await next();
                return;
            }


            var methodInfo = context.HandlerMethod?.MethodInfo;

            var unitOfWorkAttr = UnitOfWorkHelper.GetUnitOfWorkAttributeOrNull(methodInfo);
            if (unitOfWorkAttr?.Enable == false)
            {
                await next();
                return;
            }


            var unitOfWorkManager = context.HttpContext.RequestServices.GetRequiredService<IUnitOfWorkManager>();

            var options = UnitOfWorkHelper.CreateOptions(unitOfWorkAttr, context.HttpContext.Request.Method);

            using (var uow = unitOfWorkManager.Begin(options))
            {
                var executedContext = await next();

                if (executedContext.Exception == null || executedContext.ExceptionHandled)
                {
                    var result = executedContext.Result;
                    bool needCommit = true;
                    if (result is ObjectResult objResult)
                    {
                        var value = objResult.Value;
                        if (value is RetResult ret && !ret.IsSuccess)
                        {
                            needCommit = false;
                        }
                    }

                    if (needCommit)
                    {
                        uow.Complete();
                    }
                }
            }




        }




        public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            return Task.CompletedTask;
        }
    }
}
