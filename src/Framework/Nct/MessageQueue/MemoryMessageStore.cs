using Nct.Ioc;
using System;
using System.Collections.Concurrent;

namespace Nct.MessageQueue
{
    public class MemoryMessageStore : IMessageStore, ISingletonDependency
    {

        private readonly static ConcurrentDictionary<Type, BlockingCollection<IMessageData>> MessageDataDic = new();


        public virtual void Init(Type msgType)
        {
            MessageDataDic[msgType] = new BlockingCollection<IMessageData>();
        }

        public virtual IMessageData Pop(Type msgType)
        {
            return MessageDataDic[msgType].Take();

        }

        public virtual void Push(IMessageData message)
        {
            MessageDataDic[message.GetType()].Add(message);
        }
    }


}
