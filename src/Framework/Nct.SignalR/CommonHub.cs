using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using Microsoft.AspNetCore.SignalR;
using Nct.Ioc;
using Nct.Session;


namespace Nct.SignalR
{
    public class CommonHub : Hub
    {

    
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync(); 
            var client = CreateClientForCurrentConnection();
            OnlineClientManager.Instance.Add(client);
        }



        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);

            try
            {
                OnlineClientManager.Instance.Remove(Context.ConnectionId);
            }
            catch
            {
              
            }

        }

      


        private OnlineClient CreateClientForCurrentConnection()
        {

            var session = IocManager.Instance.Resolve<ISession>();

            return new OnlineClient { ConnectionId = Context.ConnectionId, UserId = session.UserId,UserName=session.UserName };

        }

    }
}
