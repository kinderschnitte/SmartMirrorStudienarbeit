using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SmartMirrorServer.HelperMethods;

namespace SmartMirrorServer.RequestHandler.Browser.Sites
{
    internal static class BrowserHome
    {
        #region Public Methods

        /// <summary>
        /// Bildet Home Seite und gibt diese zurück
        /// </summary>
        /// <returns></returns>
        public static async Task<byte[]> BuildBrowserHome()
        {
            string page = string.Empty;

            try
            {
                IEnumerable<string> file = await FileHelperClass.LoadFileFromStorage("Websites\\Browser\\home.html");

                foreach (string line in file)
                {
                    string tag = line;

                    if (tag.Contains("systemInformation.FriendlyName"))
                    {
                        tag = tag.Replace("systemInformation.FriendlyName",
                            Application.SystemInformation.FriendlyName);
                    }
                    else if (tag.Contains("systemInformation.OperatingSystem"))
                    {
                        tag = tag.Replace("systemInformation.OperatingSystem",
                            Application.SystemInformation.OperatingSystem);
                    }
                    else if (tag.Contains("systemInformation.SystemManufacturer"))
                    {
                        tag = tag.Replace("systemInformation.SystemManufacturer",
                            Application.SystemInformation.SystemManufacturer);
                    }
                    else if (tag.Contains("systemInformation.SystemProductName"))
                    {
                        tag = tag.Replace("systemInformation.SystemProductName",
                            Application.SystemInformation.SystemProductName);
                    }
                    else if (tag.Contains("systemInformation.SystemSku"))
                    {
                        tag = tag.Replace("systemInformation.SystemSku",
                            Application.SystemInformation.SystemSku);
                    }
                    else if (tag.Contains("startTime"))
                        tag = tag.Replace("startTime", Application.StartTime);

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
