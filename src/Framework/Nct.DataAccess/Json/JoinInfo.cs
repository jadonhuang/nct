using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.DataAccess.Json
{
    public class JoinInfo
    {

        public JoinType Type { get; set; }

        public string Table { get; set; }

        public string JoinedField { get; set; }

        public string Field { get; set; }

        public Condition Condition { get; set; }    

        public string Tag { get; set; }


    }
}
