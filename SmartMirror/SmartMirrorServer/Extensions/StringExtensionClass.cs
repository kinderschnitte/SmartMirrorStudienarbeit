using System;

namespace SmartMirrorServer.Extensions
{
    internal static class StringExtensionClass
    {
        /// <summary>
        /// Bestimmt den dazwischenliegenden String zwischen übergebenen Start- und Endestring
        /// </summary>
        /// <param name="strSource"></param>
        /// <param name="strStart"></param>
        /// <param name="strEnd"></param>
        /// <returns></returns>
        public static string GetBetween(this string strSource, string strStart, string strEnd)
        {
            if (!strSource.Contains(strStart) || !strSource.Contains(strEnd)) return "";
            int start = strSource.IndexOf(strStart, 0, StringComparison.Ordinal) + strStart.Length;
            int end = strSource.IndexOf(strEnd, start, StringComparison.Ordinal);
            return strSource.Substring(start, end - start);
        }

        /// <summary>
        /// Gibt den Valut Type als Enum zurück
        /// </summary>
        /// <param name="valueType"></param>
        /// <returns></returns>
        public static Enums.PostQueryEnums.ValueType GetValueType(this string valueType)
        {
            switch (valueType)
            {
                case "STATUS":
                    return Enums.PostQueryEnums.ValueType.STATUS;

                case "INTERVALL":
                    return Enums.PostQueryEnums.ValueType.INTERVALL;

                case "BRIGHTNESS":
                    return Enums.PostQueryEnums.ValueType.BRIGHTNESS;

                default:
                    return Enums.PostQueryEnums.ValueType.UNKNOWN;
            }
        }
    }
}