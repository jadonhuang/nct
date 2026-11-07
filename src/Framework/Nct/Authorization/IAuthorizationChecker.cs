using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Nct.Authorization
{
   public interface IAuthorizationChecker
    {
        void Authorize(MethodInfo method, Type type);
    }
}
