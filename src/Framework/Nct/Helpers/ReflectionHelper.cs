using Nct.Application;
using Nct.Extentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Helpers
{
   public class ReflectionHelper
    {


        public static TAttribute GetSingleAttributeOfMemberOrDeclaringTypeOrDefault<TAttribute>(MemberInfo memberInfo, TAttribute defaultValue = default(TAttribute), bool inherit = true)
         where TAttribute : class
        {
            return memberInfo.GetCustomAttributes(true).OfType<TAttribute>().FirstOrDefault()
                   ?? memberInfo.ReflectedType?.GetTypeInfo().GetCustomAttributes(true).OfType<TAttribute>().FirstOrDefault()
                   ?? defaultValue;
        }


        public static TAttribute GetSingleAttributeOrDefault<TAttribute>(MemberInfo memberInfo, TAttribute defaultValue = default(TAttribute), bool inherit = true)
            where TAttribute : Attribute
        {
            if (memberInfo.IsDefined(typeof(TAttribute), inherit))
            {
                return memberInfo.GetCustomAttributes(typeof(TAttribute), inherit).Cast<TAttribute>().First();
            }

            return defaultValue;
        }

        public static List<object> GetAttributesOfMemberAndType(MemberInfo memberInfo, Type type, bool inherit = true)
        {
            var attributeList = new List<object>();

            if(memberInfo!=null)
            attributeList.AddRange(memberInfo.GetCustomAttributes(inherit));

            attributeList.AddRange(type.GetTypeInfo().GetCustomAttributes(inherit));
            return attributeList;
        }


        public static bool HasAnyAttribute<TAttr>(Type type)
        {

            if (type.GetTypeInfo().IsDefined(typeof(TAttr), true))
            {
                return true;
            }

            return type
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Any(m => m.IsDefined(typeof(TAttr), true));

        }


        public static bool IsAppService(Type type)
        {

            return typeof(IApplicationService).IsAssignableFrom(type);

        }


        public static bool IsSystemType(Type type)
        {
            return (type == typeof(object) || Type.GetTypeCode(type) != TypeCode.Object);
        }


        public static bool IsImplementedFrom(Type type,  Type generic)
        {

            Check.NotNull(type, nameof(type));
            Check.NotNull(generic, nameof(generic));
           
            var isTheRawGenericType = type.GetInterfaces().Any(IsTheRawGenericType);
            if (isTheRawGenericType) return true;

           
            while (type != null && type != typeof(object))
            {
                isTheRawGenericType = IsTheRawGenericType(type);
                if (isTheRawGenericType) return true;
                type = type.BaseType;
            }

            return false;

            bool IsTheRawGenericType(Type test)
            => generic == (test.IsGenericType ? test.GetGenericTypeDefinition() : test);

        }


        public static bool IsPrimitiveExtendedIncludingNullable(Type type, bool includeEnums = false)
        {
            if (IsPrimitiveExtended(type, includeEnums))
            {
                return true;
            }

            if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return IsPrimitiveExtended(type.GenericTypeArguments[0], includeEnums);
            }

            return false;
        }

        private static bool IsPrimitiveExtended(Type type, bool includeEnums)
        {
            if (type.GetTypeInfo().IsPrimitive)
            {
                return true;
            }

            if (includeEnums && type.GetTypeInfo().IsEnum)
            {
                return true;
            }

            return type == typeof(string) ||
                   type == typeof(decimal) ||
                   type == typeof(DateTime) ||
                   type == typeof(DateTimeOffset) ||
                   type == typeof(TimeSpan) ||
                   type == typeof(Guid);
        }


        public static string GetConstDescription(Type t,object value)
        {
            var dic = t.ConstToDic();

            foreach (var key in dic.Keys)
            {

                if (dic[key].Equals(value))
                    return key;
            
            }


            return string.Empty;


        }

        public static Type FindIEnumerable(Type type)
        {
            if (type == null || type == typeof(string))
                return null;

            if (type.IsArray)
                return typeof(IEnumerable<>).MakeGenericType(type.GetElementType());

            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsGenericType)
            {
                foreach (Type arg in typeInfo.GenericTypeArguments)
                {
                    Type ienum = typeof(IEnumerable<>).MakeGenericType(arg);
                    if (ienum.GetTypeInfo().IsAssignableFrom(typeInfo))
                    {
                        return ienum;
                    }
                }
            }

            foreach (Type iface in typeInfo.ImplementedInterfaces)
            {
                Type ienum = FindIEnumerable(iface);
                if (ienum != null) return ienum;
            }

            if (typeInfo.BaseType != null && typeInfo.BaseType != typeof(object))
            {
                return FindIEnumerable(typeInfo.BaseType);
            }

            return null;
        }


        public static Type GetElementType(Type sequenceType)
        {
            
            Type ienum = FindIEnumerable(sequenceType);
            if (ienum == null) return sequenceType;
            return ienum.GetTypeInfo().GenericTypeArguments[0];
        }


      

    }
}
