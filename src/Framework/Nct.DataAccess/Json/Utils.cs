using Nct.Extentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.DataAccess.Json
{
    public static class Utils
    {
        public static Dictionary<string, object> FieldValuesToDic(this List<FieldValue> values)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();

            foreach (var item in values)
            {
                dic[item.Name] = item.Value;

            }
            return dic;
        }

        public static void CopyToDic(this Dictionary<string, object> src, Dictionary<string, object> des)
        {

            foreach (var item in src)
            {
                des[item.Key] = item.Value;

            }

        }



        public static bool IsSqlFun(this string str)
        {
            if (str.IsEmpty()) return false;

            var index = str.IndexOf("(");
            if (index < 1)
                return false;

            var index2 = str.LastIndexOf(")");
            if (index2 == -1 || index2 < index) return false;

            var name = str.Substring(0, index);

           // name = name.Replace("dbo.", "");

            return IsName(name);
        }

        public static bool IsName(this string str)
        {

            if (str.IsEmpty()) return false;

            foreach (char c in str)
            {
                if (!(char.IsLetterOrDigit(c) || c == '_'|| c == '.'))
                    return false;

            }

            return true;
        }




    }
}
