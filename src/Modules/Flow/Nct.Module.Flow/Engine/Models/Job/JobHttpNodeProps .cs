using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Module.Flow.Engine.Models
{

    public class HttpBody
    {

        public string Type { get; set; }

        public List<Variable> Data { get; set; } = [];


    }


    public class RetryConfig
    {
        public bool Enabled { get; set; }
        public int Interval { get; set; }
        public int MaxRetryTimes { get; set; }

    }


    public class JobHttpNodeProps : NodeProps
    {
        public List<Variable> Headers { get; set; } = [];

        public List<Variable> Params { get; set; } = [];

        public HttpBody Body { get; set; }

        public int? Timeout { get; set; }

        public RetryConfig RetryConfig { get; set; }

        public string Method { get; set; }

        public string Url { get; set; }

        public override List<Variable> GetOutputVariables()
        {
            return [

                new() { Name = "body", Type = Consts.VariableType.String },
                new() { Name = "status_code", Type = Consts.VariableType.Number },
                new() { Name = "headers", Type = Consts.VariableType.ArrayString },
                new() { Name = "file", Type = Consts.VariableType.String },
               
              ];
        }





    }
}
