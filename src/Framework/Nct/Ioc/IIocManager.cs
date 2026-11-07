using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nct.Ioc
{
   public interface IIocManager:IIocRegistrar,IIocResolver, IDisposable
    {
        IWindsorContainer Container { get; set; }
    }
}
