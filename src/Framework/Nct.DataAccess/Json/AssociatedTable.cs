using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.DataAccess.Json
{

    public class AssociatedTable
    {

        public Condition Condition { get; set; }
        public string Tag { get; set; }

        public string Table { get; set; }

        public string AssociatedField { get; set; }

        public string Field { get; set; }

        public object Value { get; set; }

        public string ParentKey { get; set; }


    }

}
