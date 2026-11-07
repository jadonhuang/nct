using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nct.Extentions;

namespace Nct
{
    public static class Check
    {
      
        public static T NotNull<T>(T value, string parameterName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            return value;
        }

      
        public static string NotNullOrEmpty(string value, string parameterName)
        {
            if (value.IsEmpty())
            {
                throw new ArgumentException($"{parameterName} is null!", parameterName);
            }

            return value;
        }

      
      
       
    }
}
