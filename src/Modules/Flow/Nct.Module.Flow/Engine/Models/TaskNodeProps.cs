using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Module.Flow.Engine.Models
{

    public class FlowAudit
    {


        public int Type { get; set; }

        public int Percent { get; set; }

        public AuditTimeLimit TimeLimit { get; set; }

        public List<AuditMember> Members { get; set; }


    }

    public class AuditMember
    {

        public int Type { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

    }


    public class AuditTimeLimit
    {

        public bool Enabled { get; set; }

        public int Timeout { get; set; }

        /// <summary>
        /// 1天2小时3分钟
        /// </summary>
        public int Unit { get; set; }

        /// <summary>
        /// 1通知提醒2自动通过3自动拒绝
        /// </summary>
        public int ExpireHandle { get; set; }

        public bool LoopNotify { get; set; }


    }

    public class TaskNodeProps : NodeProps
    {

        public FlowAudit Audit { get; set; }

        public bool FormEdit { get; set; }

    }
}
