using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Nct.Ioc;

namespace Nct.Event
{

   public class EventBus
    {

        public static IMediator Default
        {
            get
            {
                return IocManager.Instance.Resolve<IMediator>();
            }

        }

   
    }



}
