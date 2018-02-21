using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace SmartMirrorServer.HelperMethods
{
    internal static class FileHelperClass
    {
        /// <summary>
        /// Gibt die angefragte Desktop-Website zurück
        /// </summary>
        /// <returns></returns>
        public static async Task<IEnumerable<string>> LoadFileFromStorage(string path)
        {
            StorageFolder folder = Windows.ApplicationModel.Package.Current.InstalledLocation;

            IList<string> readFile = new List<string>();

            try
            {
                StorageFile file = await folder.GetFileAsync(path);

                readFile = await FileIO.ReadLinesAsync(file);
            }
            catch (Exception exception)
            {
                if (Application.Notifications.ExceptionNotifications)
                    Notification.Notification.SendPushNotification("Fehler aufgetreten.", $"{exception.StackTrace}");
            }

            return readFile;
        }
    }
}