using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.MessageQueue
{
  public static  class MessageSender
    {
        public static void Send(IMessageData message)
        {
            MessageQueueServiceBuilder.MessageQueueService.Publish(message);
           
        }

        public static void Send(IMessageData message, int delay)
        {
            MessageQueueServiceBuilder.MessageQueueService.Publish(message, delay);
           
        }

    }
}
