using Nct.Module.Form.FormDefs;
using System.Collections.Generic;

namespace Nct.Module.Form.RuleDefs
{
    public class RuleContext
    {

        public bool CancelSave { get; set; } = false;

        public string FormCode { get; set; }

        public string FormDataId { get; set; }

        public string Table { get; set; }

        public DesignInfo DesignInfo { get; set; }

        public Dictionary<string, object> Row { get; set; }

        public Dictionary<string, List<Dictionary<string, object>>> TableData { get; set; }

        public bool IsAdd { get; set; }





    }
}
