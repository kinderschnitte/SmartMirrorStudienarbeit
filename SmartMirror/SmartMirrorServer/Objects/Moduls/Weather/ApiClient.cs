using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace SmartMirrorServer.Objects.Moduls.Weather
{
    internal static class ApiClient
    {
        public static JObject GetResponse(string queryString)
        {
            using (HttpClient client = new HttpClient())
            {
                string apiKey = Application.WeatherApiKey;
                string apiUrl = Application.WeatherApiUrl;
                string url;

                if (!string.IsNullOrEmpty(apiKey))
                    url = apiUrl + queryString + "&APPID=" + apiKey;
                else
                    url = apiUrl + queryString;

                string response = client.GetStringAsync(url).Result;
                JObject parsedResponse = JObject.Parse(response);

                return parsedResponse;
            }
        }
    }
}