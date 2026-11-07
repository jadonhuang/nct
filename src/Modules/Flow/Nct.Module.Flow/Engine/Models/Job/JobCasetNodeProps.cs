using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Nct.Module.Flow.Engine.Models
{


    public class CaseCondition
    {
        public string Left { get; set; }
        public string ComparisonOperator { get; set; }
        public string Right { get; set; }

    }

    public class CaseDef
    {
        public string LogicalOperator { get; set; }
        public string[] EdgeIds { get; set; }
        public List<CaseCondition> Conditions { get; set; } = [];
        public bool IsElse { get; set; } = false;

    }
    public class JobCaseNodeProps : NodeProps
    {

        public List<CaseDef> Cases { get; set; } = [];




    }
}
