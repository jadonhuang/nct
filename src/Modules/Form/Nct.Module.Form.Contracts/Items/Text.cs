using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Module.Form.Items
{
    public class Text : BaseItem
    {
        public int MinLen { get; set; }

        public int MaxLen { get; set; } 

        public string Verify { get; set; }

        public string VerifyInfo { get; set; }

        public string Icon { get; set; }
        public  string DefaultValue { get; set; }


    }
}
