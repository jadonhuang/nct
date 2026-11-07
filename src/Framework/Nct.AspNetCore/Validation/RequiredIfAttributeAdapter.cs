using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;
using Nct.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.AspNetCore.Validation
{
    public class RequiredIfAttributeAdapter : AttributeAdapterBase<RequiredIfAttribute>
    {

        public RequiredIfAttributeAdapter(RequiredIfAttribute attribute,
       IStringLocalizer stringLocalizer)
       : base(attribute, stringLocalizer)
        {

        }


        public override string GetErrorMessage(ModelValidationContextBase validationContext)
        {

            var name = validationContext.ModelMetadata.DisplayName;
            return Attribute.FormatErrorMessage(name);
        }


        public override void AddValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-requiredif", GetErrorMessage(context));
            MergeAttribute(context.Attributes, "data-val-requiredif-isequal", Attribute.IsEqual.ToString().ToLower());
            MergeAttribute(context.Attributes, "data-val-requiredif-dependentvalue", Attribute.DependentValue?.ToString());
            MergeAttribute(context.Attributes, "data-val-requiredif-dependentproperty", Attribute.DependentProperty);
        }


    }
}
