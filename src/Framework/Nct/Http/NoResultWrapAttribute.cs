using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Http
{
    [AttributeUsage(AttributeTargets.Method)]
    public class NoResultWrapAttribute : Attribute
    {

    }
}
