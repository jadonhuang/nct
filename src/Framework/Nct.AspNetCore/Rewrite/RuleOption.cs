using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.AspNetCore.Rewrite
{

    public enum ActionType
    {
        Rewrite,
        Redirect

    }
    public class RuleOption
    {
        public string From { get; set; }

        public string To { get; set; }

        public ActionType Type { get; set; }

        public bool AppendQuery { get; set; }

    }
}
