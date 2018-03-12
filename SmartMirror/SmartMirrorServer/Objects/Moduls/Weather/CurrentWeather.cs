using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SmartMirrorServer.Objects.Moduls.Weather
{
    internal static class CurrentWeather
    {
        public static async Task<SingleResult<CurrentWeatherResult>> GetByCityNameAsync(string city, string country, string language, string units)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(city) || string.IsNullOrEmpty(country))
                    return new SingleResult<CurrentWeatherResult>(null, false, "City and/or Country cannot be empty.");
                Task<JObject> response = ApiClient.GetResponse("/weather?q=" + city + "," + country + "&lang=" + language + "&units=" + units);
                return Deserializer.GetWeatherCurrent(await response);
            }
            catch (Exception ex)
            {
                return new SingleResult<CurrentWeatherResult> { Item = null, Success = false, Message = ex.Message };
            }
        }
    }
}
