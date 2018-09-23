using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Api.Weather
{
    public static class ApiClient
    {
        public static async Task<JObject> GetResponse(string queryString)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string apiKey = Api.ApiKeys[ApiEnum.Openweathermap];
                    string apiUrl = Api.ApiUrls[ApiEnum.Openweathermap];
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
            catch (Exception)
            {
                //return await GetResponse(queryString);
                return null;
            }
        }
    }
}