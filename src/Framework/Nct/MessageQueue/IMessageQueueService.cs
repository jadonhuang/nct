using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.MessageQueue
{
    public interface IMessageQueueService
    {


        void Subscribe(Type type);

        void Publish(IMessageData data);

        void Publish(IMessageData data, int delay);

    }
}
