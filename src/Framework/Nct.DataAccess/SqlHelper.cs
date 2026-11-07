using Nct.Extentions;
using Nct.Helpers;

namespace Nct.DataAccess
{
    public static class SqlHelper
    {

        public const string RowIndexName = "__RowIndex";
        private const string ContextExtraWhereKey = "ContextExtraWhere";

        public static string GetContextExtraWhere()
        {

            return CallContext.GetData(ContextExtraWhereKey) as string;

        }

        public static void SetContextExtraWhere(string where)
        {

            CallContext.SetData(ContextExtraWhereKey, where);

        }

        public static void AppendContextExtraWhere(ref string where)
        {

            var extraWhere = GetContextExtraWhere();

            if (extraWhere.IsNotEmpty())
            {
                if (where.IsEmpty())
                {
                    where = $" where {extraWhere}";

                }
                else
                {
                    where += $" and {extraWhere}";
                }

            }

        }

    }
}
