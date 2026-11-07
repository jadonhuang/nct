using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Extentions
{
    public static class TypeExtensions
    {
        public static Assembly GetAssembly(this Type type)
        {
            return type.GetTypeInfo().Assembly;
        }


        public static IDictionary<string, object> ConstToDic(this Type t)
        {
            IDictionary<string, object> dic = new Dictionary<string, object>();
            FieldInfo[] fis = t.GetFields();
            foreach (var fieldInfo in fis)
            {
                DescriptionAttribute attr = (DescriptionAttribute)fieldInfo.GetCustomAttribute(typeof(DescriptionAttribute));
                string key = fieldInfo.Name;
                if (attr != null)
                {
                    key = attr.Description;
                }
                dic.Add(key, fieldInfo.GetRawConstantValue());
            }

            return dic;


        }

        public static bool IsAsyncType(this Type t)
        {
            return (
               t == typeof(Task) ||
                (t.GetTypeInfo().IsGenericType && t.GetGenericTypeDefinition() == typeof(Task<>))
            );
        }


        public static bool IsAnonymousType(this Type type)
        {

            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                &&type.Name.Contains("AnonymousType")
                && type.Name.StartsWith("<>");
              
        }


    }
}
