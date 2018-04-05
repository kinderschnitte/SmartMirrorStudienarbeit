using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace SmartMirror.HelperClasses
{
    internal static class QuoteOfDay
    {
        public static Objects.QuoteOfDay GetQuoteOfDay()
        {
            string fileList = getCsv();

            string[] tempStr = fileList.Split('|');

            List<string> splitted = tempStr.Where(item => !string.IsNullOrWhiteSpace(item)).ToList();

            return splitted.Count > 1 ? new Objects.QuoteOfDay {Author = splitted[1], Text = splitted[0]} : new Objects.QuoteOfDay {Author = "", Text = splitted[0]};
        }

        private static string getCsv()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://taeglicheszit.at");
                HttpResponseMessage response = client.GetAsync("/zitat-api.php?format=csv").Result;

                using (HttpContent content = response.Content)
                    return content.ReadAsStringAsync().Result;
            }
        }
    }
}