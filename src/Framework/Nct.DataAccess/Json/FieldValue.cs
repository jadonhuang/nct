using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.DataAccess.Json
{
    public class FieldValue
    {
        public object Value { get; set; }

        public string Name { get; set; }

        public string Operator { get; set; }

    }
}
