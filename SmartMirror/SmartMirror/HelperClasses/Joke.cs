using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace SmartMirror.HelperClasses
{
    internal static class Joke
    {
        public static Objects.Joke GetJoke()
        {
            string fileList = getXml();

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(fileList);

            Objects.Joke joke = new Objects.Joke
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