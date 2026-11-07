using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Module.Flow.Engine.Models
{



    public class VariableTag
    {
        public string Name { get; set; }
        public string NodeId { get; set; }     
        public string Raw { get; set; }
       

    }


    public class VariableValue
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public object Value { get; set; }


    }


    public class Variable
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }

    }
}
