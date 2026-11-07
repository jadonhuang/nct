using Nct.Module.Form.RuleDefs;
using System.Linq;

namespace Nct.Module.Form.Helpers
{
    public static class FormUtil
    {


        public static void CheckDataExist(RuleContext context, string itemName)
        {
            var idField = "Id";

            if (itemName.Contains('.'))
            {

                var rows = context.TableData;
                if (rows == null) return;

                var parts = itemName.Split('.');
                if (parts.Length != 2) return;

                var tbItemName = parts[0];
                var subItemName = parts[1];
                var item = (Table)context.DesignInfo.Items.FirstOrDefault(m => m.Name == tbItemName);
                if (item == null) return;
                var subItem = item.Items.FirstOrDefault(m => m.Name == subItemName);
                if (subItem == null) return;

                var tbRows = rows.GetOrDefault(item.Table);
                if (tbRows == null || tbRows.Count == 0) return;

                foreach (var row in tbRows)
                {
                    var value = row.GetOrDefault<string>(subItem.Field);
                    if (value.IsEmpty()) continue;
                    var id = context.Row.GetOrDefault<string>(idField);
                    var data = Sql.From(item.Table).Fields(idField).Where(m => m[subItem.Field] == value).FirstOrDefault();
                    if (data != null)
                        throw new BusinessException($"{subItem.Title}[{value}] 已经存在");
                }


            }
            else
            {

                var item = context.DesignInfo.Items.FirstOrDefault(m => m.Name == itemName);
                if (item == null || item.Field.IsEmpty()) return;
                var value = context.Row.GetOrDefault<string>(item.Field);
                if (value.IsEmpty()) return;

                var data = Sql.From(item.Table).Fields(idField).Where(m => m[item.Field] == value && m[idField]!=context.FormDataId).FirstOrDefault();
                if (data != null)
                    throw new BusinessException($"{item.Title}[{value}] 已经存在");

            }



        }
    }
}
