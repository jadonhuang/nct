using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nct.Jobs
{
    public class JobInfo
    {
        public bool IsBusy { get; set; }

        public string Name { get; set; }
        
        public JobInterval Interval { get; set; }
     
        public Type JobType { get; set; }

        public string Group { get; set; } = "Default";

        public object Data { get; set; }

        public bool LogIgnore { get; set; } = false;


    }
}