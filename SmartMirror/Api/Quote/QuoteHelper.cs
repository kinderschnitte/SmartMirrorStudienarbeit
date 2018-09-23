﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Api.Quote
{
    public static class QuoteHelper
    {
        public static async Task<Quote> GetQuoteOfDay()
        {
            string fileList = await GetCsv();

            string[] tempStr = fileList.Split('|');

            List<string> splitted = tempStr.Where(item => !string.IsNullOrWhiteSpace(item)).ToList();

            return splitted.Count > 1 ? new Quote {Author = splitted[1], Text = splitted[0]} : new Quote {Author = "Unbekannt", Text = splitted[0]};
        }

        private static async Task<string> GetCsv()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://taeglicheszit.at");
                HttpResponseMessage response = await client.GetAsync("/zitat-api.php?format=csv");

                using (HttpContent content = response.Content)
                    return await content.ReadAsStringAsync();
            }
        }
    }
}