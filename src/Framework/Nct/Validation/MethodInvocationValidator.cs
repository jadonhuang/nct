using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Nct.Extentions;
using Nct.Helpers;
using Nct.Ioc;

namespace Nct.Validation
{
    public class MethodInvocationValidator : ITransientDependency
    {

        private const int MaxValidateDepth = 2;
        protected MethodInfo Method { get; private set; }
        protected object[] ParameterValues { get; private set; }
        protected ParameterInfo[] Parameters { get; private set; }
        public List<ValidationResult> ValidationErrors { get; } = new List<ValidationResult>();

        protected List<Type> ValidatorTypes { get; private set; }

        public virtual void Initialize(MethodInfo method, object[] parameterValues)
        {
            Check.NotNull(method, nameof(method));
            Check.NotNull(parameterValues, nameof(parameterValues));

            Method = method;
            ParameterValues = parameterValues;
            Parameters = method.GetParameters();
            ValidatorTypes = new List<Type> { typeof(DataAnnotationsValidator), typeof(ValidatableObjectValidator) };
        }

        public void Validate()
        {

            Check.NotNull(Method, nameof(Method));

            if (Parameters.IsNullOrEmpty())
            {
                return;
            }

            if (!Method.IsPublic)
            {
                return;
            }

            if (IsValidationDisabled())
            {
                return;
            }

            if (Parameters.Length != ParameterValues.Length)
            {
                throw new Exception("Parameters length not equal ParameterValues length!");
            }

            //ValidationErrors.Clear();

            if (!ValidationErrors.Any())
            {

                for (var i = 0; i < Parameters.Length; i++)
                {
                    ValidateMethodParameter(Parameters[i], ParameterValues[i]);
                }
            }

            if (ValidationErrors.Any())
            {
                ThrowValidationError();
            }


        }



        protected virtual void ValidateMethodParameter(ParameterInfo parameterInfo, object parameterValue)
        {

            if (parameterValue == null)
            {
                return;
            }

            ValidateObjectRecursively(parameterValue, 1);


        }

        protected virtual void ThrowValidationError()
        {
            var errMessage = string.Join(Environment.NewLine, ValidationErrors.Select(m => m.ErrorMessage));
            throw new BusinessException(errMessage);
        }



        protected virtual void ValidateObjectRecursively(object validatingObject, int currentDepth)
        {

            if (currentDepth > MaxValidateDepth)
            {
                return;
            }

            if (validatingObject == null)
            {
                return;
            }

            if (ReflectionHelper.IsPrimitiveExtendedIncludingNullable(validatingObject.GetType()))
            {
                return;
            }



            if (IsEnumerable(validatingObject))
            {
                foreach (var item in (IEnumerable)validatingObject)
                {
                    ValidateObjectRecursively(item, currentDepth + 1);
                }

                return;
            }


            foreach (var validatorType in ValidatorTypes)
            {
                if (!ShouldValidateUsingValidator(validatingObject, validatorType)) continue;

                var validator = IocManager.Instance.Resolve<IDataValidator>(validatorType);
                var validationResults = validator.Validate(validatingObject);
                ValidationErrors.AddRange(validationResults);
            }


            var properties = TypeDescriptor.GetProperties(validatingObject).Cast<PropertyDescriptor>();
            foreach (var property in properties)
            {               
                ValidateObjectRecursively(property.GetValue(validatingObject), currentDepth + 1);
            }


        }


        protected virtual bool IsValidationDisabled()
        {
            if (Method.IsDefined(typeof(EnableValidationAttribute), true))
            {
                return false;
            }

            return ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault<DisableValidationAttribute>(Method) != null;
        }

        protected virtual bool ShouldValidateUsingValidator(object validatingObject, Type validatorType)
        {
            return true;
        }


        private bool IsEnumerable(object validatingObject)
        {
            return
                validatingObject is IEnumerable &&
                !(validatingObject is IQueryable) &&
                !ReflectionHelper.IsPrimitiveExtendedIncludingNullable(validatingObject.GetType());

        }

    }
}
