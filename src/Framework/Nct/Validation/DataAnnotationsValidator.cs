using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Validation
{
   public class DataAnnotationsValidator:IDataValidator
    {
        public virtual IReadOnlyList<ValidationResult> Validate(object validatingObject)
        {
            return GetDataAnnotationAttributeErrors(validatingObject);
        }

      
        protected virtual List<ValidationResult> GetDataAnnotationAttributeErrors(object validatingObject)
        {
            var validationErrors = new List<ValidationResult>();
            DataAnnotationsHelper.ValidateObject(validatingObject, validationErrors); 
            return validationErrors;
        }
    }
}
