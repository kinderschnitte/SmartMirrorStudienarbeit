using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SmartMirrorServer.Features.SunTimes;
using SmartMirrorServer.HelperMethods;

namespace SmartMirrorServer.RequestHandler.Sites
{
    internal static class Time
    {
        #region Public Methods

        /// <summary>
        /// Bildet Home Seite und gibt diese zurück
        /// </summary>
        /// <returns></returns>
        public static async Task<byte[]> BuildTime()
        {
            string page = string.Empty;

            try
            {
                IEnumerable<string> file = await FileHelperClass.LoadFileFromStorage("SmartMirrorServer\\Websites\\time.html");

                if (!Application.Data.TryGetValue(Application.StorageData.TimeModul, out dynamic r))
                    return Encoding.UTF8.GetBytes(page);

                Sun sun = (Sun)r;

                foreach (string line in file)
                {
                    string tag = line;

                    if (tag.Contains("Datum"))
                        tag = tag.Replace("Datum", DateTime.Now.ToString("D"));
                    else if (tag.Contains("sunrisetime") || tag.Contains("sunsettime"))
                    {
                        tag = tag.Replace("sunrisetime", sun.Sunrise);
                        tag = tag.Replace("sunsettime", sun.Sunset);
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
