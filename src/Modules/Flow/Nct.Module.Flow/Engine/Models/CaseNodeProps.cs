using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Module.Flow.Engine.Models
{
    public class NodeCdt
    {
        public string SqlCdt {  get; set; }

        public string CodeCdt { get; set; }

        public int CdtType { get; set; } = 1;

        public bool IsDefault { get; set; }

        public string Text { get; set; }    

        public string EdgeId { get; set; }
    }

    public class CaseNodeProps : NodeProps
    {
        public List<NodeCdt> Cdts { get; set; }
       

    }

}
