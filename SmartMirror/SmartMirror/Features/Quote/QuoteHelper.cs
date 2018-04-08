using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace SmartMirror.Features.Quote
{
    internal static class QuoteHelper
    {
        public static Quote GetQuoteOfDay()
        {
            string fileList = getCsv();

            string[] tempStr = fileList.Split('|');

            List<string> splitted = tempStr.Where(item => !string.IsNullOrWhiteSpace(item)).ToList();

            return splitted.Count > 1 ? new Quote {Author = splitted[1], Text = splitted[0]} : new Quote {Author = "", Text = splitted[0]};
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