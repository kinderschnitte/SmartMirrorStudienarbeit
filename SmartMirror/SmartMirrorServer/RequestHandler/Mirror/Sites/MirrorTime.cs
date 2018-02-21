using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartMirrorServer.HelperMethods;

namespace SmartMirrorServer.RequestHandler.Mirror.Sites
{
    internal static class MirrorTime
    {
        #region Public Methods

        /// <summary>
        /// Bildet Time Seite und gibt diese zurück
        /// </summary>
        /// <returns></returns>
        public static async Task<byte[]> BuildMirrorTime()
        {
            string page = string.Empty;

            try
            {
                IEnumerable<string> file = await FileHelperClass.LoadFileFromStorage("Websites\\Mirror\\time.html");

                page = file.Aggregate(page, (current, line) => current + line);
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
