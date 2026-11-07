using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Nct.Helpers;
using Nct.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nct.AspNetCore.Validation.Components
{
    public class DataValidator : ComponentBase, IDisposable
    {

        private const int MaxValidateDepth = 2;

        [CascadingParameter] EditContext CurrentEditContext { get; set; }

        private ValidationMessageStore _validationMessageStore;


        protected override void OnInitialized()
        {
            _validationMessageStore = new ValidationMessageStore(CurrentEditContext);

            if (CurrentEditContext == null)
            {
                throw new InvalidOperationException($"{nameof(DataValidator)} requires a cascading " +
                    $"parameter of type {nameof(EditContext)}. For example, you can use {nameof(DataValidator)} " +
                    $"inside an EditForm.");
            }

            CurrentEditContext.OnValidationRequested += delegate
            {

                _validationMessageStore.Clear();
                ValidateObject(CurrentEditContext.Model, CurrentEditContext, _validationMessageStore, 1);
                CurrentEditContext.NotifyValidationStateChanged();
            };

            CurrentEditContext.OnFieldChanged += (sender, e) =>
            {
                FieldIdentifier fieldIdentifier = e.FieldIdentifier;
                ValidateField(CurrentEditContext, _validationMessageStore, fieldIdentifier);
            };
        }



        private static void ValidateObject(object value, EditContext editContext, ValidationMessageStore messages, int currentDepth)
        {

            if (currentDepth > MaxValidateDepth)
            {
                return;
            }

            if (value == null)
            {
                return;
            }

            if (ReflectionHelper.IsPrimitiveExtendedIncludingNullable(value.GetType()))
            {
                return;
            }


            if (value is IEnumerable<object> enumerable)
            {

                foreach (object item in enumerable)
                {
                    ValidateObject(item, editContext, messages, currentDepth++);

                }
                return;
            }


            List<ValidationResult> list = new();

            DataAnnotationsHelper.ValidateObject(value, list);

            var objectValidator = Ioc.IocManager.Instance.Resolve<ValidatableObjectValidator>();
            var errors = objectValidator.Validate(value);
            list.AddRange(errors);


            foreach (ValidationResult item in list)
            {
                if (!item.MemberNames.Any())
                {

                    FieldIdentifier fieldIdentifier = new FieldIdentifier(value, string.Empty);
                    messages.Add(in fieldIdentifier, item.ErrorMessage);
                    continue;
                }
                foreach (string memberName in item.MemberNames)
                {
                    FieldIdentifier fieldIdentifier = new FieldIdentifier(value, memberName);
                    messages.Add(fieldIdentifier, item.ErrorMessage);
                }
            }



            var properties = TypeDescriptor.GetProperties(value).Cast<PropertyDescriptor>();
            foreach (var property in properties)
            {

                ValidateObject(property.GetValue(value), editContext, messages, currentDepth + 1);
            }


        }





        private static void ValidateField(EditContext editContext, ValidationMessageStore messages, FieldIdentifier fieldIdentifier)
        {
            PropertyInfo property = fieldIdentifier.Model.GetType().GetProperty(fieldIdentifier.FieldName);
            if (property != null)
            {

                List<ValidationResult> list = new List<ValidationResult>();
                bool valid = DataAnnotationsHelper.ValidateProperty(fieldIdentifier.Model, property.Name, list);
                messages.Clear(fieldIdentifier);
                messages.Add(fieldIdentifier, list.Select(result => result.ErrorMessage));

                if (valid)
                {
                    var objectValidator = Ioc.IocManager.Instance.Resolve<ValidatableObjectValidator>();
                    var errors = objectValidator.Validate(fieldIdentifier.Model);
                    messages.Add(fieldIdentifier, errors.Where(result => result.MemberNames != null && result.MemberNames.Any(m => m == property.Name)).Select(result => result.ErrorMessage));
                }

                editContext.NotifyValidationStateChanged();
            }
        }


        protected virtual void Dispose(bool disposing)
        {

        }

        void IDisposable.Dispose()
        {

            Dispose(disposing: true);
        }
    }
}
