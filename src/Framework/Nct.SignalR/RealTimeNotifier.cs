using Microsoft.AspNetCore.SignalR;
using Nct.Ioc;
using Nct.Logging;
using System;

namespace Nct.SignalR
{


    public interface IRealTimeNotifier
    {

        void SendNotifications(params UserNotification[] userNotifications);
    }

    public class RealTimeNotifier : IRealTimeNotifier, ITransientDependency
    {

        public static void Send(params UserNotification[] userNotifications)
        {
            IocManager.Instance.Using<IRealTimeNotifier>(nf =>
            {
                nf.SendNotifications(userNotifications);
            });

        }

        private readonly IHubContext<CommonHub> _hubContext;

        public RealTimeNotifier(IHubContext<CommonHub> hubContext)
        {
            _hubContext = hubContext;
        }


        public void SendNotifications(params UserNotification[] userNotifications)
        {

            foreach (var userNotification in userNotifications)
            {
                try
                {

                    var onlineClients = OnlineClientManager.Instance.GetAllByUserId(userNotification.UserId);

                    foreach (var onlineClient in onlineClients)
                    {

                        var signalRClient = _hubContext.Clients.Client(onlineClient.ConnectionId);
                        if (signalRClient == null)
                        {
                            continue;
                        }
                        userNotification.SessionId = onlineClient.ConnectionId;
                        signalRClient.SendAsync("getNotification", userNotification);
                    }
                }
                catch (Exception ex)
                {
                    IocManager.Instance.Resolve<ILogger>().Error(ex);
                }
            }


        }

    }
}
