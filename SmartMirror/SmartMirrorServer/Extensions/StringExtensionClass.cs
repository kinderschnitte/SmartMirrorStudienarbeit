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
    }
}