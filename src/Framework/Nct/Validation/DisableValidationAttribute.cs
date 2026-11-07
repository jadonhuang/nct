using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class DisableValidationAttribute : Attribute
    {

    }
}
