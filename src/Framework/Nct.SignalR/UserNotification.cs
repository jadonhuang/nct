using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.SignalR
{

   [Serializable]
   public class UserNotification
    {
        public int? UserId { get; set; }

        public string SessionId { get; set; }

        public string Type { get; set; }

        public  object Data { get; set; }

    }
}
