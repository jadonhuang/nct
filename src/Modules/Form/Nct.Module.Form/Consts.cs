using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Module.Form
{
    public class Consts
    {

        public const string SummaryFields = "_summaryfields";

        public readonly static Dictionary<string, string> SystemFields = new(StringComparer.OrdinalIgnoreCase) { { "Id", "str" }, { "CreateTime", "datetime" }, { "CreatorId", "int" }, { "OrgId", "int" } };
    }
}
