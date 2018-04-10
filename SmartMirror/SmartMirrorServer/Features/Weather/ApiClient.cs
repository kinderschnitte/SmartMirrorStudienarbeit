using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SmartMirrorServer.Features.Weather
{
    internal static class ApiClient
    {
        public static async Task<JObject> GetResponse(string queryString)
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

                string response = await client.GetStringAsync(url);
                JObject parsedResponse = JObject.Parse(response);

                return parsedResponse;
            }
        }
    }
}