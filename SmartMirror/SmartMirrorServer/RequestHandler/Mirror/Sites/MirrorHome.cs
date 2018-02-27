using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SmartMirrorServer.HelperMethods;
using SmartMirrorServer.Weather;

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
                DateTime date = DateTime.Today;
                bool isSunrise = false;
                bool isSunset = false;
                DateTime sunrise = DateTime.Now;
                DateTime sunset = DateTime.Now;
                SunTimes.Instance.CalculateSunRiseSetTimes(new SunTimes.LatitudeCoords (32, 4, 0, SunTimes.LatitudeCoords.Direction.NORTH), new SunTimes.LongitudeCoords (34, 46, 0, SunTimes.LongitudeCoords.Direction.EAST), date, ref sunrise, ref sunset, ref isSunrise, ref isSunset);
                string sunriseString = sunrise.ToString("HH:mm");
                string sunsetString = sunrise.ToString("HH:mm");

                // Aktuelles Wetter
                Conditions actualConditions = await Weather.Weather.GetCurrentConditions(""); // TODO Postleitzahl oder Ort hier einfügen

                string condition = string.Empty;
                string temperature = string.Empty;
                string humidity = string.Empty;
                string wind = string.Empty;
                string errorMessage = string.Empty;

                if (actualConditions != null)
                {
                    condition = "Conditions: " + actualConditions.Condition;
                    //string temperature = "Temperature (F): " + actualConditions.TempF;
                    temperature = "Temperature: " + actualConditions.TempC + " °C";
                    humidity = "Humidity: " + actualConditions.Humidity;
                    wind = "Wind: " + actualConditions.Wind;
                }
                else
                {
                    errorMessage = "There was an error processing the request.";
                }

                // Wettervorhersage
                List<Conditions> conditions = await Weather.Weather.GetForecast(""); // TODO Postleitzahl oder Ort hier einfügen

                if (conditions != null)
                {
                    foreach (Conditions c in conditions)
                    {
                        Console.WriteLine("Day: " + c.DayOfWeek);
                        Console.WriteLine("Conditions: " + c.Condition);
                        Console.WriteLine("Temperature (High): " + c.High);
                        Console.WriteLine("Temperature (Low): " + c.Low);
                        Console.WriteLine();
                    }
                }
                else
                {
                    Console.WriteLine("There was an error processing the request.");
                    Console.WriteLine("Please, make sure you are using the correct location or try again later.");
                    Console.WriteLine();
                }

                foreach (string line in file)
                {
                    string tag = line;

                    if (tag.Contains("startTime"))
                        tag = tag.Replace("startTime", Application.StartTime);
                    else if (tag.Contains("sunriseTime") || tag.Contains("sunsetTime"))
                    {
                        tag = tag.Replace("sunriseTime", sunriseString);
                        tag = tag.Replace("sunsetTime", sunsetString);
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

        #endregion Public Methods
    }
}
