using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace SmartMirrorServer.Objects.Moduls.Weather
{
    internal static class Deserializer
    {
        public static SingleResult<CurrentWeatherResult> GetWeatherCurrent(JObject response)
        {
            string error = getServerErrorFromResponse(response);

            if (!string.IsNullOrEmpty(error))
                return new SingleResult<CurrentWeatherResult>(null, false, error);

            CurrentWeatherResult weatherCurrent = new CurrentWeatherResult();

            if (response["sys"] != null)
            {
                weatherCurrent.Country = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(Convert.ToString(response["sys"]["country"])));
                weatherCurrent.Sunrise = DateTimeOffset.FromUnixTimeSeconds((long)Convert.ToDouble(Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(Convert.ToString(response["sys"]["sunrise"]))))).LocalDateTime;
                weatherCurrent.Sunset = DateTimeOffset.FromUnixTimeSeconds((long)Convert.ToDouble(Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(Convert.ToString(response["sys"]["sunset"]))))).LocalDateTime;
            }

            if (response["weather"] != null)
            {
                weatherCurrent.Title = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(Convert.ToString(response["weather"][0]["main"])));
                weatherCurrent.Description = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(Convert.ToString(response["weather"][0]["description"])));
                weatherCurrent.Icon = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(Convert.ToString(response["weather"][0]["icon"])));
            }

            if (response["main"] != null)
            {
                weatherCurrent.Temp = Convert.ToDouble(response["main"]["temp"].Value<double>());
                weatherCurrent.TempMax = Convert.ToDouble(response["main"]["temp_max"].Value<double>());

                try
                {
                    weatherCurrent.TempMin = Convert.ToDouble(response["main"]["temp_min"].Value<double>());
                }
                catch
                {
                    weatherCurrent.TempMin = 0;
                }

                weatherCurrent.Humidity = Convert.ToDouble(response["main"]["humidity"].Value<double>());

                weatherCurrent.Pressure = Convert.ToDouble(response["main"]["pressure"].Value<double>());
                //weatherCurrent.SeaLevelPressure = Convert.ToDouble(response["main"]["sea_level"].Value<double>());
                //weatherCurrent.GroundLevelPressure = Convert.ToDouble(response["main"]["grnd_level"].Value<double>());
            }

            if (response["wind"] != null)
            {
                weatherCurrent.WindSpeed = Convert.ToDouble(response["wind"]["speed"].Value<double>());
                weatherCurrent.WindDegree = Convert.ToDouble(response["wind"]["deg"].Value<double>());
            }

            if (response["clouds"] != null)
            {
                weatherCurrent.Cloudinesss = Convert.ToInt32(response["clouds"]["all"].Value<int>());
            }

            if (response["rain"] != null)
            {
                weatherCurrent.Rain = Convert.ToDouble(response["rain"]["all"].Value<double>());
            }

            if (response["snow"] != null)
            {
                weatherCurrent.Snow = Convert.ToDouble(response["snow"]["all"].Value<double>());
            }

            weatherCurrent.Date = DateTime.UtcNow;
            weatherCurrent.City = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(Convert.ToString(response["name"])));
            weatherCurrent.CityId = Convert.ToInt32(response["id"].Value<int>());

            return new SingleResult<CurrentWeatherResult>(weatherCurrent, true, TimeHelper.MessageSuccess);
        }

        public static Result<FiveDaysForecastResult> GetWeatherForecast(JObject response)
        {
            string error = getServerErrorFromResponse(response);

            if (!string.IsNullOrEmpty(error))
                return new Result<FiveDaysForecastResult>(null, false, error);


            List<FiveDaysForecastResult> weatherForecasts = new List<FiveDaysForecastResult>();

            JArray responseItems = JArray.Parse(response["list"].ToString());

            foreach (JToken item in responseItems)
            {
                FiveDaysForecastResult weatherForecast = new FiveDaysForecastResult();
                if (response["city"] != null)
                {
                    weatherForecast.City = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(Convert.ToString(response["city"]["name"])));
                    weatherForecast.Country = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(Convert.ToString(response["city"]["country"])));
                    weatherForecast.CityId = Convert.ToInt32(response["city"]["id"].Value<int>());
                }

                if (item["weather"] != null)
                {
                    weatherForecast.Title = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(Convert.ToString(item["weather"][0]["main"])));
                    weatherForecast.Description = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(Convert.ToString(item["weather"][0]["description"])));
                    weatherForecast.Icon = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(Convert.ToString(item["weather"][0]["icon"])));
                }

                if (item["main"] != null)
                {
                    weatherForecast.Temp = Convert.ToDouble(item["main"]["temp"].Value<double>());
                    weatherForecast.TempMax = Convert.ToDouble(item["main"]["temp_max"].Value<double>());
                    weatherForecast.TempMin = Convert.ToDouble(item["main"]["temp_min"].Value<double>());
                    weatherForecast.Humidity = Convert.ToDouble(item["main"]["humidity"].Value<double>());
                }

                if (item["wind"] != null)
                    weatherForecast.WindSpeed = Convert.ToDouble(item["wind"]["speed"].Value<double>());

                if (item["clouds"] != null)
                    weatherForecast.Clouds = Convert.ToDouble(item["clouds"]["all"].Value<double>());

                weatherForecast.Date = Convert.ToDateTime(item["dt_txt"].Value<DateTime>());
                weatherForecast.DateUnixFormat = Convert.ToInt32(item["dt"].Value<int>());

                weatherForecasts.Add(weatherForecast);
            }

            return new Result<FiveDaysForecastResult>(weatherForecasts, true, TimeHelper.MessageSuccess);
        }

        private static string getServerErrorFromResponse(JObject response)
        {
            if (response["cod"].ToString() == "200")
                return null;

            string errorMessage = "Server error " + response["cod"];

            if (!string.IsNullOrEmpty(response["message"].ToString()))
                errorMessage += ". " + response["message"];

            return errorMessage;
        }
    }
}
