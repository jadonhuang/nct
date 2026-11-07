using Nct.Json;
using System;
using System.Linq;

namespace Nct.Extentions
{

    public static class ObjectExtensions
    {

        public static T As<T>(this object obj) where T : class
        {
            return (T)obj;
        }


      
        public static bool IsIn<T>(this T item, params T[] list)
        {
            return list.Contains(item);
        }


        /// <summary>
        /// 深表拷贝
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T Clone<T>(this T source)
        {
            if (source == null)
            {
                return default;
            }

            return source.ToJson().ToObject<T>();

        }

      

        public static T To<T>(this object obj, T defaultValue = default(T))
        {

            T result;

            if (obj != null)
            {
                Type typeFromHandle = typeof(T);
                if (typeFromHandle.IsInterface || typeFromHandle.IsClass)
                {
                    if (obj is T)
                    {
                        result = (T)obj;
                        return result;
                    }
                }
                else
                {
                    if (typeFromHandle.IsGenericType && typeFromHandle.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        result = (T)(Convert.ChangeType(obj, Nullable.GetUnderlyingType(typeFromHandle)));
                        return result;
                    }
                    if (typeFromHandle.IsEnum)
                    {
                        result = (T)(Enum.Parse(typeFromHandle, obj.ToString()));
                        return result;
                    }
                    result = (T)(Convert.ChangeType(obj, typeFromHandle));
                    return result;
                }
            }

            result = defaultValue;
            return result;

        }


     
        public static int ToInt(this object data, int defValue = 0)
        {
            if (data == null)
                return defValue;
            int result;
            var success = int.TryParse(data.ToString(), out result);
            if (success)
                return result;
            try
            {
                return Convert.ToInt32(ToDouble(data, 0));
            }
            catch (Exception)
            {
                return defValue;
            }
        }

      
        public static int? ToIntOrNull(this object data)
        {
            if (data == null)
                return null;
            int result;
            bool isValid = int.TryParse(data.ToString(), out result);
            if (isValid)
                return result;
            return null;
        }

        public static long ToInt64(this object data, long defValue = 0)
        {
            if (data == null)
                return defValue;
            long result;
            var success = long.TryParse(data.ToString(), out result);
            if (success)
                return result;
            try
            {
                return Convert.ToInt64(ToDouble(data, 0));
            }
            catch (Exception)
            {
                return defValue;
            }
        }

      
        public static long? ToInt64OrNull(this object data)
        {
            if (data == null)
                return null;
            long result;
            bool isValid = long.TryParse(data.ToString(), out result);
            if (isValid)
                return result;
            return null;
        }

        public static double ToDouble(this object data)
        {
            if (data == null)
                return 0;
            double result;
            return double.TryParse(data.ToString(), out result) ? result : 0;
        }

     
        public static double ToDouble(this object data, int digits)
        {
            return Math.Round(ToDouble(data), digits);
        }

       
        public static double? ToDoubleOrNull(this object data)
        {
            if (data == null)
                return null;
            double result;
            bool isValid = double.TryParse(data.ToString(), out result);
            if (isValid)
                return result;
            return null;
        }

       
        public static decimal ToDecimal(this object data)
        {
            if (data == null)
                return 0;
            decimal result;
            return decimal.TryParse(data.ToString(), out result) ? result : 0;
        }

        public static decimal ToDecimal(this object data, int digits)
        {
            return Math.Round(ToDecimal(data), digits);
        }

        public static decimal? ToDecimalOrNull(this object data)
        {
            if (data == null)
                return null;
            decimal result;
            bool isValid = decimal.TryParse(data.ToString(), out result);
            if (isValid)
                return result;
            return null;
        }

      
        public static decimal? ToDecimalOrNull(this object data, int digits)
        {
            var result = ToDecimalOrNull(data);
            if (result == null)
                return null;
            return Math.Round(result.Value, digits);
        }

      



    }
}
