using Nct.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nct.MessageQueue
{
    public class MessageQueueServiceBuilder
    {

        private List<Type> msgTypes = [];

        public static IMessageQueueService MessageQueueService { get; private set; }

        public static MessageQueueServiceBuilder Create()
        {
            return new MessageQueueServiceBuilder();
        }

        public MessageQueueServiceBuilder Register(IReadOnlyList<Type> types)
        {

            msgTypes = types.Where(m => typeof(IMessageData).IsAssignableFrom(m) && m.IsClass && m != typeof(MessageData)).ToList();
            return this;

        }


        public void Build()
        {

            MessageQueueService = IocManager.Instance.Resolve<IMessageQueueService>();

            foreach (var type in msgTypes)
            {
                MessageQueueService.Subscribe(type);
            }

        }




    }
}
