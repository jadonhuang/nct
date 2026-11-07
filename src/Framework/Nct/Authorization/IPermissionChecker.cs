using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Authorization
{
   public interface IPermissionChecker
    {

        bool Authorize(params string[] permissions);
    }
}
