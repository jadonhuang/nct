using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.MessageQueue
{
    public interface IMessageStore
    {

        void Push(IMessageData message);
        IMessageData Pop(Type msgType);
        void Init(Type msgType);

    }
}
