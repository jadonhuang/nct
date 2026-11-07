using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Authorization
{
    public class NullPermissionChecker : IPermissionChecker
    {
        public bool Authorize(string[] permissions)
        {
            return true;
        }

        public static IPermissionChecker Instance { get; } = new NullPermissionChecker();


    }
}
