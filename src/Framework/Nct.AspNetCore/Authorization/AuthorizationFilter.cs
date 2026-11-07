using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nct.Authorization;
using Nct.Ioc;
using Nct.Session;
using System;

namespace Nct.AspNetCore.Authorization
{


    public class AuthorizationFilter : IAuthorizationFilter,ITransientDependency
    {

        public bool AllowMultiple => false;

        private IAuthorizationChecker _authorizationChecker;

        private ISession _session;

        public AuthorizationFilter(IAuthorizationChecker authorizationChecker, ISession session)
        {

            _authorizationChecker = authorizationChecker;
            _session = session;
        }





        public void OnAuthorization(AuthorizationFilterContext context)
        {
                  

            if (!context.ActionDescriptor.IsControllerAction())
            {
                return;
            }

            var methodInfo = context.ActionDescriptor.GetMethodInfo();
      
            try
            {
                _authorizationChecker.Authorize(methodInfo, methodInfo.DeclaringType);

            }
            catch (AuthorizationException ex)
            {

                if (ActionResultHelper.IsObjectResult(context.ActionDescriptor.GetMethodInfo().ReturnType))
                {
                    var statusCode = _session.IsAuthenticated ? (int)System.Net.HttpStatusCode.Forbidden : (int)System.Net.HttpStatusCode.Unauthorized;

                    context.Result = new ObjectResult(new RetResult().Error(ex.Message, statusCode))
                    {
                        StatusCode = statusCode
                    };
                }
                else
                {
                    context.Result = new ChallengeResult();
                }

            }

            catch (Exception ex)
            {

                if (ActionResultHelper.IsObjectResult(context.ActionDescriptor.GetMethodInfo().ReturnType))
                {
                    context.Result = new ObjectResult(new RetResult().Error(ex.Message))
                    {
                        StatusCode = 500
                    };
                }
                else
                {
                    context.Result = new StatusCodeResult((int)System.Net.HttpStatusCode.InternalServerError);
                }
            }





        }
    }



}
