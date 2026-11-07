using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Module.Form.Items
{
    public class Tab:BaseItem
    {

        public List<TabPage> Tabs { get; set; }

        public string Style { get; set; }

        public string TabPosition { get; set; }

    }

    public class TabPage
    {

        public string Title { get; set; }

        public string Icon { get; set; }

        public List<BaseItem> Items { get; set; }


    }
}
