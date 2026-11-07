using Nct.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.MessageQueue
{
    public class MessageData : IMessageData
    {
        public virtual ISession Session { get; set; }
    }
}
