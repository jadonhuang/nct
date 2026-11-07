using Microsoft.AspNetCore.Mvc.Filters;
using Nct.Extentions;
using Nct.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nct.AspNetCore.Validation
{
    public class ActionInvocationValidator : MethodInvocationValidator
    {
        private ActionExecutingContext _actionContext;


        public void Initialize(ActionExecutingContext actionContext)
        {
            _actionContext = actionContext;

            var methodInfo = actionContext.ActionDescriptor.GetMethodInfo();

            base.Initialize(methodInfo, GetParamValues(methodInfo));

            //SetDataAnnotationAttributeErrors();

        }

        private object[] GetParamValues(MethodInfo method)
        {

            var parameters = method.GetParameters();
            var parameterValues = new object[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                parameterValues[i] = _actionContext.ActionArguments.GetOrDefault(parameters[i].Name);
            }

            return parameterValues;

        }

        //private void SetDataAnnotationAttributeErrors()
        //{
        //    foreach (var state in _actionContext.ModelState)
        //    {
        //        var key = state.Key;

        //        if (!string.IsNullOrEmpty(key) && (key.EndsWith(".String") || key.Contains(".Nullable`"))) continue;  //not a good solution

        //        foreach (var error in state.Value.Errors)
        //        {
        //            base.ValidationErrors.Add(new ValidationResult(error.ErrorMessage, new[] { state.Key }));
        //        }
        //    }
        //}

        //protected override bool ShouldValidateUsingValidator(object validatingObject, Type validatorType)
        //{
        //    if (validatorType == typeof(DataAnnotationsValidator) || validatorType == typeof(ValidatableObjectValidator)) return false;
        //    return true;
        //}


    }
}
