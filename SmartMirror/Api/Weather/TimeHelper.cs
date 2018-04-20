using System;

namespace Api.Weather
{
    internal static class TimeHelper
    {
        public const string MessageSuccess = "Successful operation";

        public static long ToUnixTimestamp(DateTime target)
        {
            DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, target.Kind);
            long unixTimestamp = Convert.ToInt64((target - date).TotalSeconds);

            return unixTimestamp;
        }

        public static DateTime ToDateTime(int timestamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            return dateTime.AddSeconds(timestamp);
        }
    }
}