using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using DataAccessLibrary;
using DataAccessLibrary.Module;
using SmartMirrorServer.Features.Weather;
using SmartMirrorServer.HelperMethods;

namespace SmartMirrorServer.RequestHandler.Sites
{
    internal static class Weather
    {
        #region Public Methods

        /// <summary>
        /// Bildet Home Seite und gibt diese zurück
        /// </summary>
        /// <returns></returns>
        public static async Task<byte[]> BuildWeather()
        {
            string page = string.Empty;

            try
            {
                IEnumerable<string> file = await FileHelperClass.LoadFileFromStorage("SmartMirrorServer\\Websites\\weather.html");

                if (!ModuleData.Data.TryGetValue(Modules.WEATHER, out dynamic r))
                    return Encoding.UTF8.GetBytes(page);

                SingleResult<CurrentWeatherResult> currentResult = (SingleResult<CurrentWeatherResult>)r;

                foreach (string line in file)
                {
                    string tag = line;

                    if (tag.Contains("CityId"))
                        tag = tag.Replace("CityId", currentResult.Item.CityId.ToString());
                    else if (tag.Contains("City"))
                        tag = tag.Replace("City", currentResult.Item.City);
                    else if (tag.Contains("Description"))
                        tag = tag.Replace("Description", currentResult.Item.Description);
                    else if (tag.Contains("WeatherIcon"))
                        tag = tag.Replace("WeatherIcon", WeatherHelperClass.ChooseWeatherIcon(currentResult.Item.Icon));
                    else if (tag.Contains("Temperature"))
                        tag = tag.Replace("Temperature", Math.Round(currentResult.Item.Temp, 1).ToString(CultureInfo.InvariantCulture));
                    else if (tag.Contains("TempMin"))
                        tag = tag.Replace("TempMin", Math.Round(currentResult.Item.TempMin, 1).ToString(CultureInfo.InvariantCulture));

                    if (tag.Contains("TempMax"))
                        tag = tag.Replace("TempMax", Math.Round(currentResult.Item.TempMax, 1).ToString(CultureInfo.InvariantCulture));
                    else if (tag.Contains("Humidity"))
                        tag = tag.Replace("Humidity", currentResult.Item.Humidity.ToString(CultureInfo.InvariantCulture));
                    else if (tag.Contains("WindSpeed"))
                        tag = tag.Replace("WindSpeed", currentResult.Item.WindSpeed.ToString(CultureInfo.InvariantCulture));
                    else if (tag.Contains("Cloudiness"))
                        tag = tag.Replace("Cloudiness", currentResult.Item.Cloudinesss.ToString());
                    else if (tag.Contains("Pressure"))
                        tag = tag.Replace("Pressure", currentResult.Item.Pressure.ToString(CultureInfo.InvariantCulture));
                    else if (tag.Contains("PrecipitationIcon"))
                        tag = tag.Replace("PrecipitationIcon", currentResult.Item.Snow > 0 ? "snowflake.png" : "rain.png");

                    if (tag.Contains("Precipitation"))
                        tag = tag.Replace("Precipitation", currentResult.Item.Snow > 0 ? currentResult.Item.Snow.ToString(CultureInfo.InvariantCulture) : currentResult.Item.Rain.ToString(CultureInfo.InvariantCulture));

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

        #endregion Public Methods
    }
}
