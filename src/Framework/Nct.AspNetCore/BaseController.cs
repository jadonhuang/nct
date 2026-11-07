using Microsoft.AspNetCore.Mvc;
using Nct.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.AspNetCore
{
    public class BaseController: Controller, ITransientDependency
    {

    }
}
