using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.DataAccess.Json
{


    public class JsonTable
    {
       
        public string Table { get; set; }

        public string TableAlias { get; set; }

        public List<Field> Fields { get; set; } = new();

        public List<FieldValue> FieldValues { get; set; } = new();

        public List<AssociatedTable> AssociatedTables { get; set; } = new();

        public List<JoinInfo> Joins { get; set; } = new();

        public int? Page { get; set; }

        public int? Count { get; set; }

        public string ParentKey { get; set; }

        public string SelfKey { get; set; }

        public List<JsonCondition> Conditions { get; set; } = new();

        public List<Field> GropuBys { get; set; } = new();

        public Dictionary<string, string> OrderBys { get; set; } = new();

        public string Combine { get; set; }
         
        public int Query { get; set; }

        public StatementType? Statement { get; set; }


    }



}
