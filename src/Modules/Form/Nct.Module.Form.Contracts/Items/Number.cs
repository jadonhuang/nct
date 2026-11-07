using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Module.Form.Items
{
    public class Number: BaseItem
    {

        public decimal Min { get; set; }

        public decimal Max { get; set; }

        public int Precision { get; set; }

        public decimal? DefaultValue { get; set; }

        public string Suffix { get; set; }


    }
}
