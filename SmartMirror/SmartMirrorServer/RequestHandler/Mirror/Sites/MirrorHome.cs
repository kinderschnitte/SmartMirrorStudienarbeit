using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SmartMirrorServer.HelperMethods;
using SmartMirrorServer.Objects.Moduls;
using SmartMirrorServer.Objects.Moduls.Weather;
using SmartMirrorServer.SerializableClasses;

namespace SmartMirrorServer.RequestHandler.Mirror.Sites
{
    internal static class MirrorHome
    {
        #region Public Methods

        /// <summary>
        /// Bildet Home Seite und gibt diese zurück
        /// </summary>
        /// <returns></returns>
        public static async Task<byte[]> BuildMirrorHome()
        {
            string page = string.Empty;

            try
            {
                IEnumerable<string> file = await FileHelperClass.LoadFileFromStorage("Websites\\Mirror\\home.html");

                // Sunset / Sunrise
                Sun sun = new Sun();

                SingleResult<CurrentWeatherResult> currentResult = await getCurrentWeatherByCityName();
                Result<FiveDaysForecastResult> fiveDayForecastResult = await getFiveDaysForecastByCityName();

                foreach (string line in file)
                {
                    string tag = line;

                    if (tag.Contains("startTime"))
                        tag = tag.Replace("startTime", Application.StartTime);
                    else if (tag.Contains("sunriseTime") || tag.Contains("sunsetTime"))
                    {
                        tag = tag.Replace("sunriseTime", sun.Sunrise);
                        tag = tag.Replace("sunsetTime", sun.Sunset);
                    }

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

        private static async Task<SingleResult<CurrentWeatherResult>> getCurrentWeatherByCityName()
        {
            return await CurrentWeather.GetByCityNameAsync(Application.StorageData.City, Application.StorageData.Country, Application.StorageData.Language, "metric");
        }


        private static async Task<Result<FiveDaysForecastResult>> getFiveDaysForecastByCityName()
        {
            return await FiveDaysForecast.GetByCityNameAsync("Karlsruhe", "Germany", "de", "metric"); // TODO Wie oben anpassen
        }

        #endregion Public Methods
    }
}
