using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Authorization
{
    public class NullRoleChecker : IRoleChecker
    {
       

        public bool Authorize(string[] roles)
        {
            return true;
        }

        public static IRoleChecker Instance { get; } = new NullRoleChecker();

    }
}
