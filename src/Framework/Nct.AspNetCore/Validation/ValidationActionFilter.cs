using Microsoft.AspNetCore.Mvc.Filters;
using Nct.Ioc;
using System;
using System.Threading.Tasks;

namespace Nct.AspNetCore.Validation
{
    public class ValidateActionFilter : IAsyncActionFilter, ITransientDependency
    {
        private ActionInvocationValidator _invocationValidator;
        public ValidateActionFilter(ActionInvocationValidator invocationValidator)
        {
            _invocationValidator = invocationValidator;
        }


        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            if (!context.ActionDescriptor.IsControllerAction())
            {
                await next();
                return;
            }


            _invocationValidator.Initialize(context);
            try
            {
                _invocationValidator.Validate();

            }
            catch (Exception ex)
            {

                if (ActionResultHelper.IsObjectResult(context.ActionDescriptor.GetMethodInfo().ReturnType))
                    throw;

                context.ModelState.Clear();
                context.ModelState.AddModelError(nameof(ValidateActionFilter), ex.Message);

            }

            await next();
        }
    }
}
