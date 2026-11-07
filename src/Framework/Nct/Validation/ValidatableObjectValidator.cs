using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Validation
{
    public class ValidatableObjectValidator : IDataValidator
    {
        public IReadOnlyList<ValidationResult> Validate(object validatingObject)
        {
            var validationErrors = new List<ValidationResult>();

            if (validatingObject is IValidatableObject  validatableObject)
            {
                var validationResults = validatableObject.Validate(new ValidationContext(validatableObject));

                if (validationResults != null)
                {
                    foreach (ValidationResult result in validationResults)
                    {
                        if (result != ValidationResult.Success)
                        {
                            validationErrors.Add(result);
                        }
                    }
                }
;
            }

            return validationErrors;
        }
    }
}
