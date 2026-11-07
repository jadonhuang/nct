using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Nct.Extentions
{

    public static class StringExtensions
    {



        public static string FormatEx(this string str, params object[] args)
        {
            return string.Format(str, args);
        }


        public static string EnsureStartWith(this string str, char c, StringComparison comparisonType)
        {
            ArgumentNullException.ThrowIfNull(str);

            if (str.StartsWith(c.ToString(), comparisonType))
            {
                return str;
            }

            return str + c;
        }

        public static string EnsureStartWith(this string str, char c)
        {
            return EnsureStartWith(str, c, StringComparison.Ordinal);
        }



        public static string EnsureEndsWith(this string str, char c)
        {
            return EnsureEndsWith(str, c, StringComparison.Ordinal);
        }



        public static string EnsureEndsWith(this string str, char c, StringComparison comparisonType)
        {

            ArgumentNullException.ThrowIfNull(str);

            if (str.EndsWith(c.ToString(), comparisonType))
            {
                return str;
            }

            return str + c;
        }





        public static bool IsEmpty(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }


        public static bool IsNotEmpty(this string str)
        {
            return !IsEmpty(str);
        }



        public static string[] SplitString(this string str, string separator = ",")
        {
            if (str.IsEmpty()) return [];
            return str.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);
        }


        public static string[] SplitToLines(this string str)
        {
            return str.Split(Environment.NewLine);
        }



        public static string ToCamelCase(this string str, bool invariantCulture = true)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return str;
            }

            if (str.Length == 1)
            {
                return invariantCulture ? str.ToLowerInvariant() : str.ToLower();
            }

            return (invariantCulture ? char.ToLowerInvariant(str[0]) : char.ToLower(str[0])) + str.Substring(1);
        }




        public static T ToEnum<T>(this string value)
            where T : struct
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return (T)Enum.Parse(typeof(T), value);
        }


        public static T ToEnum<T>(this string value, bool ignoreCase)
            where T : struct
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return (T)Enum.Parse(typeof(T), value, ignoreCase);
        }

        public static string ToMd5(this string str)
        {
            var inputBytes = Encoding.UTF8.GetBytes(str);
            var hashBytes = MD5.HashData(inputBytes);
            return Convert.ToHexString(hashBytes).ToLower();

        }




        public static string Truncate(this string str, int maxLength)
        {
            if (str == null)
            {
                return null;
            }

            if (str.Length <= maxLength)
            {
                return str;
            }

            return str.Substring(0, maxLength);
        }


        public static string RemovePostFix(this string str, params string[] postFixes)
        {
            if (str == null)
            {
                return null;
            }

            if (str == string.Empty)
            {
                return string.Empty;
            }

            if (postFixes.IsNullOrEmpty())
            {
                return str;
            }

            foreach (var postFix in postFixes)
            {
                if (str.EndsWith(postFix))
                {
                    return str.Substring(0, str.Length - postFix.Length);
                }
            }

            return str;
        }


        public static bool IgnoreCaseEqual(this string a, string b)
        {
            if (a == null && b == null) return true;

            return a != null && a.Equals(b, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IgnoreCaseContains(this string a, string b)
        {
            if (a == null && b == null) return true;

            return a != null && a.Contains(b, StringComparison.OrdinalIgnoreCase);
        }


        public static bool IsDateTime(this string str)
        {
            DateTime test;
            return DateTime.TryParse(str, out test);
        }

        public static bool IsInt(this string str)
        {
            long test;
            return long.TryParse(str, out test);
        }

        public static bool IsDecimal(this string str)
        {
            decimal test;
            return decimal.TryParse(str, out test);
        }

        public static bool IsFloat(this string str)
        {
            float test;
            return float.TryParse(str, out test);
        }

        public static bool IsGuid(this string str)
        {
            Guid test;
            return Guid.TryParse(str, out test);
        }


        public static string ToHtml(this string str)
        {
            if (str == null || str.Equals(""))
            {
                return str;
            }
            StringBuilder sb = new StringBuilder(str);
            sb.Replace("&", "&amp;");
            sb.Replace("<", "&lt;");
            sb.Replace(">", "&gt;");
            sb.Replace("\r\n", "<br>");
            sb.Replace("\n", "<br>");
            sb.Replace("\t", " ");
            sb.Replace(" ", "&nbsp;");
            return sb.ToString();
        }




        public static string NoHTML(this string html)
        {
            if (string.IsNullOrWhiteSpace(html)) return "";
            html = Regex.Replace(html, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"-->", "", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"<!--.*", "", RegexOptions.IgnoreCase);

            html = Regex.Replace(html, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&#(\d+);", "", RegexOptions.IgnoreCase);

            html.Replace("<", "");
            html.Replace(">", "");
            html.Replace("\r\n", "");
            html = HttpUtility.HtmlEncode(html);

            return html;
        }

        public static string UrlEncode(this string text)
        {
            return HttpUtility.UrlEncode(text);


        }

        public static string UrlDecode(this string text)
        {
            return HttpUtility.UrlDecode(text);


        }

        public static string HtmlDecode(this string text)
        {
            return HttpUtility.HtmlDecode(text);


        }

        public static string ClearSpace(this string text)
        {
            if (text.IsEmpty()) return text;
            return text.Replace(" ", "");


        }

        public static string ToBase64(this string text)
        {
            if (text.IsEmpty()) return text;

            var bytes = Encoding.UTF8.GetBytes(text);

            var base64 = Convert.ToBase64String(bytes, 0, bytes.Length);

            return base64;


        }

        public static bool IsValidParamName(this string paramName)
        {
            return Regex.IsMatch(paramName, "[a-zA-Z]\\w*");

        }
        public static string SafeSql(this string paramName)
        {
            if (paramName.IsEmpty()) return paramName;

            return paramName.Replace("'", "");

        }

    }
}