using System;

namespace SmartMirror.HelperClasses
{
    public static class DayOfWeekHelper
    {
        public static DayOfWeek GetDayOfWeek(string dayString)
        {
            switch (dayString)
            {
                case "monday":
                    return DayOfWeek.Monday;

                case "tuesday":
                    return DayOfWeek.Tuesday;

                case "wednesday":
                    return DayOfWeek.Wednesday;

                case "thursday":
                    return DayOfWeek.Thursday;

                case "friday":
                    return DayOfWeek.Friday;

                case "saturday":
                    return DayOfWeek.Saturday;

                case "sunday":
                    return DayOfWeek.Sunday;

                default:
                    return DayOfWeek.Monday;
            }
        }
    }
}