using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Api.GoogleMapsGeocoding
{
    internal static class ApiClient
    {
        public static async Task<JObject> GetResponse(string city, string state, string country)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string apiKey = Api.ApiKeys[ApiEnum.Googlemapsgeocoding];
                    string apiUrl = Api.ApiUrls[ApiEnum.Googlemapsgeocoding];

                    string url = apiUrl + city + "," + state + "," + country + "&key=" + apiKey;

                    string response = await client.GetStringAsync(url);
                    JObject parsedResponse = JObject.Parse(response);

                    return parsedResponse;
                }
            }
            catch (Exception)
            {
                return await GetResponse(city, state, country);
            }
        }
    }
}