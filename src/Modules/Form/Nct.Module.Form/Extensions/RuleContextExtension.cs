using Nct.Module.Form.RuleDefs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Module.Form.Extensions
{
    public static class RuleContextExtension
    {


        private static bool IsSubItem(string itemName)
        {
            return itemName.Contains('.');

        }
        private static BaseItem FindItem(List<BaseItem> items, string itemName)
        {

            BaseItem item = null;
            var nameParts = itemName.SplitString(".");
            if (nameParts.Length == 1)
            {
                item = items.FirstOrDefault(m => m.Name == itemName);

            }
            if (nameParts.Length == 2)
            {
                if (items.FirstOrDefault(m => m.Name == nameParts[0]) is Table table)
                {
                    item = table.Items.FirstOrDefault(m => m.Name == itemName);

                }
            }

            return item;

        }

        public static void SetItemValue(this RuleContext ruleContext, string itemName, object value)
        {

            var item = FindItem(ruleContext.DesignInfo.AllItems, itemName);
            if (item == null) return;

            bool isSubItem = IsSubItem(itemName);

            Sql.From(item.Table).WhereIf(m => m["id"] == ruleContext.FormDataId, !isSubItem)
                .Update(m => new Dictionary<string, object> { { item.Field, value } });


        }


        public static void CheckItemValueExists(this RuleContext ruleContext, string itemName, string errMsg = null)
        {

            bool exist = false;

            var item = FindItem(ruleContext.DesignInfo.AllItems, itemName);
            if (item == null) return;

            if (IsSubItem(itemName))
            {

                var rows = ruleContext.TableData.GetOrDefault(item.Table);
                if (rows == null) return;

                var index = 1;
                foreach (var row in rows)
                {
                    var itemValue = row.GetValueOrDefault(item.Field)?.ToString();
                    var id = row.GetOrDefault("Id") as string;
                    var result = Sql.From(item.Table).Fields("Id").Where(m => m[item.Field] == itemValue && m["Id"] != id).FirstOrDefault();
                    exist = result != null;
                    if (exist)
                    {
                        errMsg ??= $"第{index}行{item.Title}[{itemValue}]已存在\"";
                        break;
                    }

                    index++;
                }


            }
            else
            {
                var itemValue = ruleContext.Row.GetValueOrDefault(item.Field)?.ToString();
                var result = Sql.From(item.Table).Fields("Id").Where(m => m[item.Field] == itemValue && m["Id"] != ruleContext.FormDataId).FirstOrDefault();
                if (result != null)
                {
                    exist = true;
                    errMsg ??= $"{item.Title}[{itemValue}]已存在";
                }
            }

            if (exist)
            {
                throw new BusinessException(errMsg);

            }


        }


    }
}
