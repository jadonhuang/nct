using Nct.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.DataAccess.Log
{
   public class DbLogConfiguration
    {
        public  LogLevel Level {get;set; }

        public int WarnExecuteTime { get; set; }

    }
}
