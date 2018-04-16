using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using SmartMirrorServer.HelperClasses;

namespace SmartMirrorServer.RequestHandler.Sites
{
    internal static class Help
    {
        #region Public Methods

        /// <summary>
        /// Bildet Help Seite und gibt diese zurück
        /// </summary>
        /// <returns></returns>
        public static async Task<byte[]> BuildHelp()
        {
            string page = string.Empty;

            try
            {
                IEnumerable<string> file = await FileHelperClass.LoadFileFromStorage("SmartMirrorServer\\Websites\\help.html");
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
