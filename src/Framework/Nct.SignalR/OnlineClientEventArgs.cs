using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.SignalR
{
   public class OnlineClientEventArgs:EventArgs
    {
        public OnlineClient Client { get; }

        public OnlineClientEventArgs(OnlineClient client)
        {
            Client = client;
        }
    }
}
