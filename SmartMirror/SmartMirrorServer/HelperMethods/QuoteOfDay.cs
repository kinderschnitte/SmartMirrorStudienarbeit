using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SmartMirrorServer.HelperMethods
{
    internal static class QuoteOfDay
    {
        public static async Task<Objects.QuoteOfDay> GetQuoteOfDay()
        {
            string fileList = await getCsv(Application.QuoteOfDay);

            string[] tempStr = fileList.Split('|');

            List<string> splitted = tempStr.Where(item => !string.IsNullOrWhiteSpace(item)).ToList();

            return splitted.Count > 1 ? new Objects.QuoteOfDay {Author = splitted[1], Text = splitted[0]} : new Objects.QuoteOfDay {Author = "", Text = splitted[0]};
        }

        private static async Task<string> getCsv(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://taeglicheszit.at");
                HttpResponseMessage response = await client.GetAsync("/zitat-api.php?format=csv");

                using (HttpContent content = response.Content)
                {
                    return await content.ReadAsStringAsync();
                }
            }
        }
    }
}