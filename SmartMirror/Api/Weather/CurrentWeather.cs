using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Api.Weather
{
    public static class CurrentWeather
    {
        public static async Task<SingleResult<CurrentWeatherResult>> GetByCityName(string city, string country, string language, string units)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(city) || string.IsNullOrEmpty(country))
                    return new SingleResult<CurrentWeatherResult>(null, false, "City and/or Country cannot be empty.");

                JObject response = await ApiClient.GetResponse("/weather?q=" + city + "," + country + "&lang=" + language + "&units=" + units);

                return Deserializer.GetWeatherCurrent(response);
            }
            catch (Exception ex)
            {
                return new SingleResult<CurrentWeatherResult> { Item = null, Success = false, Message = ex.Message };
            }
        }

        public static async Task<SingleResult<CurrentWeatherResult>> GetByPostal(string postal, string country, string language, string units)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(postal) || string.IsNullOrEmpty(country))
                    return new SingleResult<CurrentWeatherResult>(null, false, "Postal and/or Country cannot be empty.");

                JObject response = await ApiClient.GetResponse("/weather?zip=" + postal + "," + country + "&lang=" + language + "&units=" + units);

                return Deserializer.GetWeatherCurrent(response);
            }
            catch (Exception ex)
            {
                return new SingleResult<CurrentWeatherResult> { Item = null, Success = false, Message = ex.Message };
            }
        }

        public static async Task<SingleResult<CurrentWeatherResult>> GetByCityCode(string cityCode, string language, string units)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cityCode))
                    return new SingleResult<CurrentWeatherResult>(null, false, "Postal and/or Country cannot be empty.");

                JObject response = await ApiClient.GetResponse("/weather?id=" + cityCode + "&lang=" + language + "&units=" + units);

                return Deserializer.GetWeatherCurrent(response);
            }
            catch (Exception ex)
            {
                return new SingleResult<CurrentWeatherResult> { Item = null, Success = false, Message = ex.Message };
            }
        }
    }
}
