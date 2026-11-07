using Nct.LinqQuery;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Extentions
{
    public static class IEnumerableExtensions
    {
        private static bool IsNullable(Type t)
        {
            return !t.IsValueType || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }


        private static Type GetCoreType(Type t)
        {
            if (t != null && IsNullable(t))
            {
                if (!t.IsValueType)
                {
                    return t;
                }
                else
                {
                    return Nullable.GetUnderlyingType(t);
                }
            }
            else
            {
                return t;
            }
        }

        public static DataTable ToDataTable<T>(this IEnumerable<T> list)
        {

            var dt = new DataTable();

            if (list == null || !list.Any())
                return dt;

            Type type= typeof(T);

            if (type == typeof(Dictionary<string, object>))
            {
                var dic = list.First() as Dictionary<string, object>;

                foreach (var kv in dic)
                {
                    dt.Columns.Add(kv.Key, kv.Value.GetType());
                }

                foreach (var item in list)
                {
                    var itemDic = item as Dictionary<string, object>;
                    var row = dt.NewRow();
                    foreach (var kv in itemDic)
                    {
                        row[kv.Key] = kv.Value;
                    }
                    dt.Rows.Add(row);
                }

            }

            else
            {

                var entity = EntityCache.GetOrAdd(type);
                if (entity.TableName != null)
                {
                    dt.TableName = entity.TableName;
                }

                PropertyInfo[] props = type.GetProperties();

                for (int i = 0; i < props.Length; i++)
                {
                    dt.Columns.Add(props[i].Name, GetCoreType(props[i].PropertyType));
                }

                foreach (var item in list)
                {

                    var values = new object[props.Length];

                    for (int i = 0; i < props.Length; i++)
                    {
                        values[i] = props[i].GetValue(item, null);

                    }
                    dt.Rows.Add(values);


                }
            }

            return dt;
        }





    }
}
