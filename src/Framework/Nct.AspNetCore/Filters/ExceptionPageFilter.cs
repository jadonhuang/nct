using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Nct.Authorization;
using Nct.Domain.Uow;
using Nct.Ioc;
using Nct.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Nct.AspNetCore.Filters
{
    public class ExceptionPageFilter : IAsyncPageFilter, ITransientDependency
    {

        public ILogger Logger { get; set; }

        public ExceptionPageFilter()
        {
            Logger = NullLogger.Instance;

        }


        public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {

            if (context.HandlerMethod == null)
            {
                await next();
                return;
            }

            var executedContext = await next();

            if (executedContext.Exception == null)
            {
                return;
            }

            if (!ActionResultHelper.IsObjectResult(context.HandlerMethod.MethodInfo.ReturnType)) return;

            var retResult = new RetResult();

            Exception exception = executedContext.Exception;

            var statusCode = GetStatusCode(executedContext);

            var errMsg = statusCode == (int)HttpStatusCode.InternalServerError ? "System error" : exception.Message;

            retResult.Error(errMsg, statusCode);

            Logger.Error(exception);
            executedContext.Result = new ObjectResult(retResult);
            executedContext.Exception = null;



        }


        protected virtual int GetStatusCode(PageHandlerExecutedContext context)
        {
            if (context.Exception is AuthorizationException)
            {
                return context.HttpContext.User.Identity.IsAuthenticated
                    ? (int)HttpStatusCode.Forbidden
                    : (int)HttpStatusCode.Unauthorized;
            }

            if (context.Exception is BusinessException)
            {
                return (int)HttpStatusCode.BadRequest;
            }

            return (int)HttpStatusCode.InternalServerError;


        }




        public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            return Task.CompletedTask;
        }
    }
}
