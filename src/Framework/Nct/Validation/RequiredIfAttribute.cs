using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Nct
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredIfAttribute : ValidationAttribute
    {
        private const string DefaultErrMsg = "The {0} field is required.";

        public object DependentValue { get; private set; }
        public string DependentProperty { get; private set; }

        public bool IsEqual { get; set; } = true;

        public RequiredIfAttribute(string dependentProperty, object dependentValue):base(DefaultErrMsg)
        {

            DependentValue = dependentValue;
            DependentProperty = dependentProperty;

        }


        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            var otherProperty = validationContext.ObjectInstance.GetType().GetProperty(DependentProperty);

            var otherPropertyValue = otherProperty.GetValue(validationContext.ObjectInstance, null);

            if ((IsEqual && DependentValue.Equals(otherPropertyValue)) || (!IsEqual && !DependentValue.Equals(otherPropertyValue)))
            {
                if (value == null) return new ValidationResult(null, new string[] { validationContext.MemberName });
            }

            return ValidationResult.Success;
        }




    }

}

