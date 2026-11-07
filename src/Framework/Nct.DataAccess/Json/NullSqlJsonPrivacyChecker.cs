using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.DataAccess.Json
{
    public class NullSqlJsonPrivacyChecker : ISqlJsonPrivacyChecker
    {
        public Task<PrivacyCheckResult> Check(JsonTable table, StatementType statementType)
        {
            return Task.FromResult(new PrivacyCheckResult { Success = true });
        }

        public static ISqlJsonPrivacyChecker Instance { get; } = new NullSqlJsonPrivacyChecker();
    }
}
