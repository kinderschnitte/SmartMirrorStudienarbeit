using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SmartMirrorServer.Objects.Moduls.Weather
{
    internal class CurrentWeather
    {
        public async Task<SingleResult<CurrentWeatherResult>> GetByCityNameAsync(string city, string country)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(city) || string.IsNullOrEmpty(country))
                    return new SingleResult<CurrentWeatherResult>(null, false, "City and/or Country cannot be empty.");
                Task<JObject> response = ApiClient.GetResponse("/weather?q=" + city + "," + country);
                return Deserializer.GetWeatherCurrent(await response);
            }
            catch (Exception ex)
            {
                return new SingleResult<CurrentWeatherResult> {Item = null, Success = false, Message = ex.Message};
            }
        }

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

        public static async Task<SingleResult<CurrentWeatherResult>> GetByCoordinatesAsync(double lat, double lon)
        {
            try
            {
                Task<JObject> o = ApiClient.GetResponse("/weather?lat=" + lat + "&lon=" + lon);
                return Deserializer.GetWeatherCurrent(await o);
            }
            catch (Exception ex)
            {
                return new SingleResult<CurrentWeatherResult> {Item = null, Success = false, Message = ex.Message};
            }
        }

        public static async Task<SingleResult<CurrentWeatherResult>> GetByCoordinatesAsync(double lat, double lon, string language, string units)
        {
            try
            {
                Task<JObject> o = ApiClient.GetResponse("/weather?lat=" + lat + "&lon=" + lon + "&lang=" + language + "&units=" + units);
                return Deserializer.GetWeatherCurrent(await o);
            }
            catch (Exception ex)
            {
                return new SingleResult<CurrentWeatherResult> { Item = null, Success = false, Message = ex.Message };
            }
        }

        public static async Task<SingleResult<CurrentWeatherResult>> GetByCityIdAsync(int id)
        {
            try
            {
                if (0 > id)
                    return new SingleResult<CurrentWeatherResult>(null, false, "City Id must be a positive number.");
                Task<JObject> o = ApiClient.GetResponse("/weather?id=" + id);
                return Deserializer.GetWeatherCurrent(await o);
            }
            catch (Exception ex)
            {
                return new SingleResult<CurrentWeatherResult> {Item = null, Success = false, Message = ex.Message};
            }
        }

        public static async Task<SingleResult<CurrentWeatherResult>> GetByCityIdAsync(int id, string language, string units)
        {
            try
            {
                if (0 > id)
                    return new SingleResult<CurrentWeatherResult>(null, false, "City Id must be a positive number.");
                Task<JObject> o = ApiClient.GetResponse("/weather?id=" + id + "&lang=" + language + "&units=" + units);
                return Deserializer.GetWeatherCurrent(await o);
            }
            catch (Exception ex)
            {
                return new SingleResult<CurrentWeatherResult> { Item = null, Success = false, Message = ex.Message };
            }
        }
    }
}
