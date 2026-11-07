using Nct.AspNetCore.Authorization;
using Nct.AspNetCore.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Nct.AspNetCore.Validation;
using Nct.AspNetCore.DynamicApi;

namespace Nct.AspNetCore
{
   public static class MvcOptionsExtensions
    {

        public static void AddNct(this MvcOptions options, IServiceCollection services)
        {
            options.Conventions.Add(new AppServiceConvention(services));
            AddFilters(options);
        }


        private static void AddFilters(MvcOptions options)
        {
            options.Filters.AddService(typeof(AuthorizationFilter));
            options.Filters.AddService(typeof(ExceptionFilter));
            options.Filters.AddService(typeof(ExceptionPageFilter));
            options.Filters.AddService(typeof(ValidateActionFilter));
            options.Filters.AddService(typeof(ResultWrapFilter));
            options.Filters.AddService(typeof(UowActionFilter));
            options.Filters.AddService(typeof(UowPageFilter));

        }
    }
}
