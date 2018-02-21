using System;
using System.IO;
using SmartMirrorServer.Objects;

namespace SmartMirrorServer.RequestHandler
{
    internal static class ResponseImage
    {
        #region Public Methods

        /// <summary>
        /// Gibt das angeforderte Bild als Byte-Array zurück
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static byte[] LoadImage(Request request)
        {
            try
            {
                return File.ReadAllBytes("Images/" + request.Query.CompleteQuery.Replace(" ", ""));
            }
            catch (Exception exception)
            {
                if (Application.Notifications.ExceptionNotifications)
                    Notification.Notification.SendPushNotification("Fehler aufgetreten.", $"{exception.StackTrace}");

                return new byte[0];
            }
        }

        #endregion Public Methods
    }
}