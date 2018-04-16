using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SmartMirrorServer.HelperClasses;
using SmartMirrorServer.Objects;

namespace SmartMirrorServer.RequestHandler.Sites
{
    internal static class Settings
    {
        #region Public Methods

        /// <summary>
        /// Bildet Home Seite und gibt diese zurück
        /// </summary>
        /// <returns></returns>
        public static async Task<byte[]> BuildSettings(Request request)
        {
            string page = string.Empty;

            try
            {
                IEnumerable<string> file = await FileHelperClass.LoadFileFromStorage("SmartMirrorServer\\Websites\\settings.html");
                page = string.Join("", file);
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
