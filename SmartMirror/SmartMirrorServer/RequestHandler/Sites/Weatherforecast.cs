using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartMirrorServer.HelperMethods;
using SmartMirrorServer.Objects;
using SmartMirrorServer.Objects.Moduls.Weather;

namespace SmartMirrorServer.RequestHandler.Sites
{
    internal static class Weatherforecast
    {
        #region Public Methods

        /// <summary>
        /// Bildet Home Seite und gibt diese zurück
        /// </summary>
        /// <returns></returns>
        public static async Task<byte[]> BuildWeatherforecast()
        {
            string page = string.Empty;

            try
            {
                IEnumerable<string> file = await FileHelperClass.LoadFileFromStorage("SmartMirrorServer\\Websites\\weatherforecast.html");

                foreach (string line in file)
                {
                    string tag = line;

                    if (tag.Contains("Forecast"))
                        tag = tag.Replace("Forecast", await getForecastString());

                    page += tag;
                }
            }
            catch (Exception exception)
            {
                if (Application.Notifications.ExceptionNotifications)
                    Notification.Notification.SendPushNotification("Fehler aufgetreten.", $"{exception.StackTrace}");
            }

            return Encoding.UTF8.GetBytes(page);
        }

        private static async Task<string> getForecastString()
        {
            List<List<FiveDaysForecastResult>> result = await getcalculatedForecast(Application.StorageData.WeatherforecastModul);

            // Infos zu heutigen Tag löschen
            result.RemoveAt(0);

            StringBuilder forecastString = new StringBuilder();

            forecastString.Append("<table style=\"width: 100 %; height: 100 %; padding: 1 %; color: white; text-align: center; table-layout: fixed;\">");
            forecastString.Append(" <col width=\"11.11%;\"> <col width=\"11.11%;\"> <col width=\"11.11%;\"> <col width=\"11.11%;\"> <col width=\"11.11%;\"> <col width=\"11.11%;\"> <col width=\"11.11%;\"> <col width=\"11.11%;\"> <col width=\"11.11%;\">");
            forecastString.Append($" <tr style=\"cursor: pointer;\" onclick=\"window.location='https://openweathermap.org/city/{result[0][0].CityId}'\"> <td colspan=\"10\" style=\"font-size: 1.25em; text-align: right;\"> <img src=\"location.png\" alt=\"\" style=\"height: 0.75em;\"/>{result[0][0].City}</td> </tr>");

            foreach (List<FiveDaysForecastResult> fiveDaysForecastResults in result)
            {
                string day;

                if (result[0] == fiveDaysForecastResults)
                    day = "Morgen";
                else if (result[1] == fiveDaysForecastResults)
                    day = "Übermorgen";
                else
                    day = fiveDaysForecastResults[0].Date.ToString("dddd");
                
                forecastString.Append($" <tr style=\"cursor: pointer;\" onclick=\"window.location='https://openweathermap.org/city/{fiveDaysForecastResults[0].CityId}'\"> <td colspan=\"9\" style=\"font-size: 1.75em; color: grey; text-align: left; padding-top: 2.5%;\">{day}</td> </tr>");

                foreach (FiveDaysForecastResult fiveDaysForecastResult in fiveDaysForecastResults)
                {
                    forecastString.Append($" <tr style=\"cursor: pointer; font-size: 1em;\" onclick=\"window.location='https://openweathermap.org/city/{fiveDaysForecastResult.CityId}'\">");
                    forecastString.Append($" <td>{fiveDaysForecastResult.Date:t}</td>");
                    forecastString.Append($" <td> <img src=\"{WeatherHelperClass.ChooseWeatherIcon(fiveDaysForecastResult.Icon)}\" alt=\"\" style=\"width: 5%; margin: auto;\"/> </td>");
                    forecastString.Append($" <td>{fiveDaysForecastResult.Description}</td>");
                    forecastString.Append($" <td>{Math.Round(fiveDaysForecastResult.Temp, 1).ToString(CultureInfo.InvariantCulture)} °C</td>");
                    //forecastString.Append($" <td>{Math.Round(fiveDaysForecastResult.TempMin, 1).ToString(CultureInfo.InvariantCulture)} °C / {Math.Round(fiveDaysForecastResult.TempMax, 1).ToString(CultureInfo.InvariantCulture)} °C</td>");
                    forecastString.Append($" <td> <img src=\"humidity.png\" alt=\"\" style=\"height: 0.75em;\"/> {fiveDaysForecastResult.Humidity} %</td>");
                    forecastString.Append($" <td> <img src=\"windspeed.png\" alt=\"\" style=\"height: 0.75em;\"/> {fiveDaysForecastResult.WindSpeed} m/s</td>");
                    forecastString.Append($" <td> <img src=\"cloudiness.png\" alt=\"\" style=\"height: 0.75em;\"/> {fiveDaysForecastResult.Cloudinesss} %</td>");
                    forecastString.Append($" <td> <img src=\"pressure.png\" alt=\"\" style=\"height: 0.75em;\"/> {fiveDaysForecastResult.Pressure} hPa</td>");

                    string precipitationIcon = fiveDaysForecastResult.Snow > 0 ? "snowflake.png" : "rain.png";
                    double precipitation = fiveDaysForecastResult.Snow > 0 ? fiveDaysForecastResult.Snow : fiveDaysForecastResult.Rain;
                    forecastString.Append($" <td> <img src=\"{precipitationIcon}\" alt=\"\" style=\"height: 0.75em;\"/> {Math.Round(precipitation, 2).ToString(CultureInfo.InvariantCulture)} mm</td>");
                    forecastString.Append(" </tr>");
                }
            }
            
            forecastString.Append(" </table>");

            return forecastString.ToString();
        }

        private static async Task<Result<FiveDaysForecastResult>> getFiveDaysForecastByCityName(Module module)
        {
            return await FiveDaysForecast.GetByCityNameAsync(module.City, module.Country, module.Language, "metric");
        }

        private static async Task<List<List<FiveDaysForecastResult>>> getcalculatedForecast(Module module)
        {
            Result<FiveDaysForecastResult> forecasts = await getFiveDaysForecastByCityName(module);

            List<List<FiveDaysForecastResult>> result = forecasts.Items.GroupBy(d => d.Date.DayOfYear).Select(s => s.ToList()).ToList();

            return result;
        }

        #endregion Public Methods
    }
}
