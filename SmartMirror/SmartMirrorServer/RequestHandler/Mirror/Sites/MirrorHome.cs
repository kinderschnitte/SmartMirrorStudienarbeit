using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SmartMirrorServer.HelperMethods;

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

                DateTime date = DateTime.Today;
                bool isSunrise = false;
                bool isSunset = false;
                DateTime sunrise = DateTime.Now;
                DateTime sunset = DateTime.Now;

                SunTimes.Instance.CalculateSunRiseSetTimes(new SunTimes.LatitudeCoords (32, 4, 0, SunTimes.LatitudeCoords.Direction.North), new SunTimes.LongitudeCoords (34, 46, 0, SunTimes.LongitudeCoords.Direction.East), date, ref sunrise, ref sunset, ref isSunrise, ref isSunset);

                string sunriseString = sunrise.ToString("HH:mm");
                string sunsetString = sunrise.ToString("HH:mm");

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
