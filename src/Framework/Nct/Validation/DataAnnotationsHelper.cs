using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Nct.Extentions;
using Nct.Ioc;

namespace Nct.Validation
{
    public static class DataAnnotationsHelper
    {

        private readonly static ConcurrentDictionary<Type, string> ErrMsgFormates = new ConcurrentDictionary<Type, string>();


        public static bool ValidateObject(object validatingObject, ICollection<ValidationResult> validationErrors)
        {

            var validateResult = true;
            var properties = TypeDescriptor.GetProperties(validatingObject).Cast<PropertyDescriptor>();
            foreach (var property in properties)
            {

                List<ValidationResult> validationResults = new();

                if (!ValidateProperty(validatingObject, property, validationResults))
                {
                    validateResult = false;

                    foreach (var result in validationResults)
                        validationErrors.Add(result);

                }

            }

            return validateResult;

        }

        private static bool ValidateProperty(object validatingObject, PropertyDescriptor propertyDes, ICollection<ValidationResult> validationErrors)
        {

            var validateResult = true;

            var validationAttributes = propertyDes.Attributes.OfType<ValidationAttribute>().ToList();

            if (validationAttributes.Count == 0)
            {
                return validateResult;
            }

            if (validationAttributes.Count > 1)
            {
                var requireAttr = validationAttributes.FirstOrDefault(m => m is RequiredAttribute || m is RequiredIfAttribute);
                if (requireAttr != null)
                {
                    validationAttributes.Remove(requireAttr);
                    validationAttributes.Insert(0, requireAttr);


                }
            }

            var validationContext = new ValidationContext(validatingObject)
            {
                MemberName = propertyDes.Name
            };

            foreach (var attribute in validationAttributes)
            {

                var result = attribute.GetValidationResult(propertyDes.GetValue(validatingObject), validationContext);
                if (result != null && result != ValidationResult.Success)
                {
                    SetLocalizeErrorMessage(attribute, validationContext, result);
                    validationErrors.Add(result);
                    validateResult = false;
                }
            }

            return validateResult;

        }

        private static void SetLocalizeErrorMessage(ValidationAttribute validationAttr, ValidationContext validationContext, ValidationResult validationResult)
        {

            if (validationAttr.ErrorMessageResourceType != null) return;

            IocManager.Instance.Using<IStringLocalizer>(locals =>
            {

                var displayName = locals.GetString(validationContext.DisplayName);

                var resourceName = validationAttr.ErrorMessageResourceName ?? validationAttr.ErrorMessage;

                if (resourceName == null)
                {
                    var attrType = validationAttr.GetType();
                    if (!ErrMsgFormates.TryGetValue(attrType, out resourceName))
                    {
                        var prop = validationAttr.GetType().GetProperty("ErrorMessageString", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                        resourceName = prop.GetValue(validationAttr) as string;
                        ErrMsgFormates.TryAdd(attrType, resourceName);
                    }

                }

                string errorMessage = validationResult.ErrorMessage;

                if (validationAttr is RequiredAttribute
                || validationAttr is RequiredIfAttribute
                || validationAttr is CustomValidateAttribute
                || validationAttr is EmailAddressAttribute
                || validationAttr is UrlAttribute
                || validationAttr is RegularExpressionAttribute

                )
                {
                    errorMessage = locals.GetString(resourceName, displayName);
                }
                else if (validationAttr is MinLengthAttribute min)
                {
                    errorMessage = locals.GetString(resourceName, displayName, min.Length);
                }
                else if (validationAttr is MaxLengthAttribute max)
                {
                    errorMessage = locals.GetString(resourceName, displayName, max.Length);
                }
                else if (validationAttr is RangeAttribute range)
                {
                    errorMessage = locals.GetString(resourceName, displayName, range.Minimum, range.Maximum);
                }
                else if (validationAttr is StringLengthAttribute  stringLength)
                {
                    if (stringLength.MinimumLength == 0)
                    {
                        errorMessage = locals.GetString(resourceName, displayName, stringLength.MaximumLength);
                    }
                    else
                    {
                        resourceName = "The field {0} must be a string with a minimum length of {2} and a maximum length of {1}.";
                        errorMessage = locals.GetString(resourceName, displayName, stringLength.MaximumLength, stringLength.MinimumLength);
                    }
                       
                }
               
                validationResult.ErrorMessage = errorMessage;

            });


        }



        public static bool ValidateProperty(object validatingObject, string propertyName, ICollection<ValidationResult> validationErrors)
        {

            var property = TypeDescriptor.GetProperties(validatingObject).Cast<PropertyDescriptor>().First(m => m.Name == propertyName);
            return ValidateProperty(validatingObject, property, validationErrors);



        }

    }
}
