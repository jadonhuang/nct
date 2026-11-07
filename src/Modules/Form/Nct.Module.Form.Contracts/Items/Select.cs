using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Module.Form.Items
{
    public class Select : DataSourceItem
    {
        public bool MultiSelect { get; set; }

        public int RenderType { get; set; }

        public object DefaultValue { get; set; }

        public bool CheckStrictly { get; set; }

        public string ParentField { get; set; }


    }
}
