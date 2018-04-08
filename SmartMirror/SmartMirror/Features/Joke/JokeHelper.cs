using System;
using System.Net.Http;
using System.Xml;

namespace SmartMirror.Features.Joke
{
    internal static class JokeHelper
    {
        public static Joke GetJoke()
        {
            string fileList = getXml();

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(fileList);

            Joke joke = new Joke
            {
                Title = xmlDocument.GetElementsByTagName("item").Item(0).ChildNodes.Item(0).InnerText,
                Description = xmlDocument.GetElementsByTagName("item").Item(0).ChildNodes.Item(1).InnerText.Replace("<br>", "").Replace("</br>", " ").Replace(@"\n", " ").Replace("\"", " ").Replace(@"\", "").Trim()
            };
            return joke;
        }

        private static string getXml()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://witze.net");
                HttpResponseMessage response = client.GetAsync("/witze.rss?cfg=000000511").Result;

                using (HttpContent content = response.Content)
                    return content.ReadAsStringAsync().Result;
            }
        }
    }
}