using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Localization;
using Nct.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Nct.AspNetCore.Validation
{
    public class CustomValidateAttributeAdapterProvider : IValidationAttributeAdapterProvider
    {
        private readonly IValidationAttributeAdapterProvider baseProvider =
       new ValidationAttributeAdapterProvider();

        public IAttributeAdapter GetAttributeAdapter(ValidationAttribute attribute, IStringLocalizer stringLocalizer)
        {
            if (attribute is CustomValidateAttribute customValidateAttribute)
            {
                return new CustomValidateAttributeAdapter(customValidateAttribute, stringLocalizer);
            }
            else if (attribute is RequiredIfAttribute requiredIfAttribute)
            {
                return new RequiredIfAttributeAdapter(requiredIfAttribute, stringLocalizer);
            }

            return baseProvider.GetAttributeAdapter(attribute, stringLocalizer);
        }
    }
}
