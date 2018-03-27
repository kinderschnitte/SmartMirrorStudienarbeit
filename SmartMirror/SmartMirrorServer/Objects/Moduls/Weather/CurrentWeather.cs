using System;
using Newtonsoft.Json.Linq;

namespace SmartMirrorServer.Objects.Moduls.Weather
{
    internal static class CurrentWeather
    {
        public static SingleResult<CurrentWeatherResult> GetByCityName(string city, string country, string language, string units)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(city) || string.IsNullOrEmpty(country))
                    return new SingleResult<CurrentWeatherResult>(null, false, "City and/or Country cannot be empty.");
                JObject response = ApiClient.GetResponse("/weather?q=" + city + "," + country + "&lang=" + language + "&units=" + units);
                return Deserializer.GetWeatherCurrent(response);
            }
            catch (Exception ex)
            {
                return new SingleResult<CurrentWeatherResult> { Item = null, Success = false, Message = ex.Message };
            }
        }
    }
}
