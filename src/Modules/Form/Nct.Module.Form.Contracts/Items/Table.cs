using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Module.Form.Items
{
    public class Table:BaseItem
    {

        public List<BaseItem> Items { get; set; }           
        public int Layout { get; set; }

        public int MaxRows { get; set; }

    }
}
