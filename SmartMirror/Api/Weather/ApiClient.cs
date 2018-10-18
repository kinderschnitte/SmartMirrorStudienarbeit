using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Api.Weather
{
    public static class ApiClient
    {
        private static int count;

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
            catch (Exception exception)
            {
                Log.Log.WriteException(exception);

                if (count > 2)
                {
                    count = 0;
                    return new JObject();
                }

                count++;
                return await GetResponse(queryString);
            }
        }
    }
}