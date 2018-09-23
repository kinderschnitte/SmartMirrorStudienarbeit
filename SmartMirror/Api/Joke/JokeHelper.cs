using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;

namespace Api.Joke
{
    public static class JokeHelper
    {
        public static async Task<Joke> GetJoke()
        {
            string fileList = await GetXml();

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(fileList);

            Joke joke = new Joke
            {
                Title = xmlDocument.GetElementsByTagName("item").Item(0).ChildNodes.Item(0).InnerText,
                Description = xmlDocument.GetElementsByTagName("item").Item(0).ChildNodes.Item(1).InnerText.Replace("<br>", "").Replace("</br>", " ").Replace(@"\n", " ").Replace("\"", " ").Replace(@"\", "").Trim()
            };
            return joke;
        }

        private static async Task<string> GetXml()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://witze.net");
                HttpResponseMessage response = await client.GetAsync("/witze.rss?cfg=000000511");

                using (HttpContent content = response.Content)
                    return await content.ReadAsStringAsync();
            }
        }
    }
}