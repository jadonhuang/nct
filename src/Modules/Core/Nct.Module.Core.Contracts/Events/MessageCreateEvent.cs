using MediatR;
using Nct.MessageQueue;
using Nct.Module.Core.Messages;
using Nct.Module.Core.Messages.Dtos;
using System.Threading;

namespace Nct.Module.Core.Events
{
    public class Msg : MessageData
    {


        public string Title { get; set; }


        public string Content { get; set; }


        public int UserId { get; set; }


        public int Type { get; set; }


        public string Props { get; set; }



    }

    public class MessageCreateEvent : INotificationHandler<Msg>
    {
        private readonly IMessageAppService _messageAppService;
        public MessageCreateEvent(IMessageAppService messageAppService)
        {

             _messageAppService = messageAppService;

        }

        public async Task Handle(Msg notification, CancellationToken cancellationToken)
        {

            var dto = new CreateMessageDto
            {
                Title = notification.Title,
                Content = notification.Content,
                UserId = notification.UserId,
                Type = notification.Type,
                Props = notification.Props
            };
            await _messageAppService.Create(dto);


        }
    }
}
