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
    public class CustomValidateAttributeAdapter : AttributeAdapterBase<CustomValidateAttribute>
    {

        public CustomValidateAttributeAdapter(CustomValidateAttribute attribute,
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
            MergeAttribute(context.Attributes, "data-val-regex", GetErrorMessage(context));
            MergeAttribute(context.Attributes, "data-val-regex-pattern", Attribute.GetRegStr());
        }


    }
}
