using System;
using System.Collections.Generic;
using System.Linq;


namespace Nct.Extentions
{

    public static class DateTimeExtensions
    {

        public static double ToUnixTimestamp(this DateTime target)
        {
            if (target.Kind != DateTimeKind.Utc)
                target = target.ToUniversalTime();
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var diff = target - origin;
            return Math.Floor(diff.TotalSeconds);
        }


        public static DateTime FromUnixTimestamp(this double unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return epoch.AddSeconds(unixTime).ToLocalTime();
        }


        public static DateTime ToDayEnd(this DateTime target)
        {
            return target.Date.AddDays(1).AddMilliseconds(-1);
        }




        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            var diff = dt.DayOfWeek - startOfWeek;

            if (diff < 0)
                diff += 7;

            return dt.AddDays(-1 * diff).Date;
        }



        public static DateTime TrimMilliseconds(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, date.Kind);
        }


        public static string ToZHDate(this DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }

        public static string ToZHTime(this DateTime date)
        {
            return date.ToString("yyyy-MM-dd HH:mm:ss");
        }


        public static string ToFileName(this DateTime date)
        {
            return date.ToString("yyyyMMddHHmmss");
        }

    }
}