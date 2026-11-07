
using Microsoft.AspNetCore.Mvc.Filters;
using System;

using System.Net.Http;

using System.Threading;
using System.Threading.Tasks;
using Nct.Logging;
using Nct.Authorization;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Nct.Ioc;

namespace Nct.AspNetCore.Filters
{

    public class ExceptionFilter : IExceptionFilter, ITransientDependency
    {
        public ILogger Logger { get; set; }

        public ExceptionFilter()
        {
            Logger = NullLogger.Instance;

        }


        public void OnException(ExceptionContext context)
        {

            if (!ActionResultHelper.IsObjectResult(context.ActionDescriptor.GetMethodInfo().ReturnType)) return;

            var retResult = new RetResult();

            Exception exception = context.Exception;

            var statusCode = GetStatusCode(context);

            var errMsg = statusCode == (int)HttpStatusCode.InternalServerError ? "System error" : exception.Message;

            retResult.Error(errMsg, statusCode);

            if (exception is not BusinessException)
            {
                Logger.Error(exception);
            }

            context.Result = new ObjectResult(retResult);
            context.Exception = null;

        }

        protected virtual int GetStatusCode(ExceptionContext context)
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

    }
}