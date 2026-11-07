using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Nct.Session;

namespace Nct.MessageQueue
{
   public interface IMessageData: INotification
    {

         ISession Session { get; set; }

    }
}
