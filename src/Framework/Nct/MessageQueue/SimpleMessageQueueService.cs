using Nct.Event;
using Nct.Helpers;
using Nct.Ioc;
using Nct.Logging;
using System;
using System.Threading.Tasks;

namespace Nct.MessageQueue
{
    public class SimpleMessageQueueService : IMessageQueueService, ISingletonDependency
    {

        private IMessageStore _messageStore;

        private readonly ILogger _logger;

        public SimpleMessageQueueService(ILogger logger)
        {

            _logger = logger;
        }


        private void InitMessageStore(Type type)
        {

            Type messageStoreType;

            var messageStoreTypeAttr = ReflectionHelper.GetSingleAttributeOrDefault<MessageStoreTypeAttribute>(type);
            if (messageStoreTypeAttr != null)
            {
                messageStoreType = messageStoreTypeAttr.Type;

            }
            else
            {
                messageStoreType = typeof(IMessageStore);
            }

            _messageStore = IocManager.Instance.Resolve<IMessageStore>(messageStoreType);

            _messageStore.Init(type);

        }

        public void Publish(IMessageData data)
        {
            _messageStore.Push(data);
        }

        public void Publish(IMessageData data, int delay)
        {
            Task.Factory.StartNew(async () =>
            {
                await Task.Delay(delay);
                Publish(data);
            });

        }

        public void Subscribe(Type type)
        {

            InitMessageStore(type);

            var messageName = type.FullName;

            Task.Factory.StartNew(async () =>
            {

                while (true)
                {

                    IMessageData messageData = null;

                    try
                    {

                        messageData = _messageStore.Pop(type);
                        if (messageData != null)
                        {
                            _logger.Debug($"执行消息：{messageName} ");
                            await EventBus.Default.Publish(messageData);
                            _logger.Debug($"执行消息：{messageName} 成功{Environment.NewLine}");
                        }

                    }
                    catch (Exception ex)
                    {
                        _logger.Debug($"执行消息：{messageName} 失败 {ex.Message}{Environment.NewLine}");
                        if (ex is not BusinessException)
                        {

                            _logger.Error($"执行消息：{messageName} 失败 {ex}{Environment.NewLine}");

                            await Task.Delay(1000);
                            _messageStore.Push(messageData);

                        }
                    }

                }


            });

        }



    }
}
