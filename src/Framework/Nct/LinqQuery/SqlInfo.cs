using System;
using System.Collections.Generic;
using System.Text;

namespace Nct.LinqQuery
{
   public class SqlInfo
    {

        public string Order { get; set; }

        public string Select { get; set; }

        public string Where { get; set; }

        public string Join { get; set; }

        public string GroupBy { get; set; }

        public string Having { get; set; }

        public string Table { get; set; }

        public string TableAlias { get; set; }

        public SqlType Type { get; set; } = SqlType.Select;

        public int? TopNum { get; set; }

        public int? SkipNum { get; set; }


        public bool IsDistinct { get; set; }


        public object Entity { get; set; }


        public Type EntityType { get; set; }

        public bool IsAggregate { get; set; }
        public Dictionary<string, object> Parameters { get; set; } 

        public string LockMode { get; set; }


    }
}
