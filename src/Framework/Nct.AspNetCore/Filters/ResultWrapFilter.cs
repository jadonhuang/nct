using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nct.Helpers;
using Nct.Http;
using Nct.Http.ClientProxy;
using Nct.Ioc;

namespace Nct.AspNetCore.Filters
{
    public class ResultWrapFilter : IResultFilter, ITransientDependency
    {


        public virtual void OnResultExecuted(ResultExecutedContext context)
        {


        }

        public virtual void OnResultExecuting(ResultExecutingContext context)
        {
            if (!context.ActionDescriptor.IsControllerAction())
            {
                return;
            }

            var result = context.Result;
            if (result is ObjectResult objResult)
            {
                var methodInfo = context.ActionDescriptor.GetMethodInfo();
                if (methodInfo == null) return;

                var noResultWrapAttr = ReflectionHelper.GetSingleAttributeOrDefault<NoResultWrapAttribute>(methodInfo);

                if (noResultWrapAttr != null) return;

                if (context.HttpContext.Request.Headers.UserAgent == CilentProxyBase.ClientProxyClientName)
                    return;

                var value = objResult.Value;
                if (value is not RetResult)
                {
                    var ret = new RetResult();
                    if (value != null)
                    {
                        ret.Success(value);
                    }
                    else
                    {
                        ret.Error("value is null");
                    }
                    objResult.Value = ret;
                    objResult.DeclaredType = typeof(RetResult);

                }
            }

        }


    }
}
