using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.DataAccess.Json
{

    public class PrivacyCheckResult { 
    
       public bool Success { get; set; }

       public string Message { get; set; }

      

    }
    public interface ISqlJsonPrivacyChecker
    {
        Task<PrivacyCheckResult> Check(JsonTable table, StatementType statementType);

    }
}
