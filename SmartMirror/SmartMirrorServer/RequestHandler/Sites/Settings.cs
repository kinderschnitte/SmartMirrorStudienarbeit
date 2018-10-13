using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DataAccessLibrary;
using DataAccessLibrary.Tables;
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

                bool tabOneActive = false;
                bool tabTwoActive = false;

                List<LocationTable> locationData = DataAccess.GetLocationData();

                if (request.PostQuery.Value.Count == 0 || request.PostQuery.Value.ContainsKey("upperleft"))
                    tabOneActive = true;

                else if (request.PostQuery.Value.ContainsKey("City"))
                    tabTwoActive = true;

                foreach (string line in file)
                {
                    string tag = line;

                    if (tag.Contains("activebooltab1"))
                        tag = tag.Replace("activebooltab1", tabOneActive ? "is-active" : "");

                    if (tag.Contains("activebooltab2"))
                        tag = tag.Replace("activebooltab2", tabTwoActive ? "is-active" : "");

                    if (tag.Contains("cityValue"))
                        tag = tag.Replace("cityValue", locationData[0].City);

                    if (tag.Contains("postalValue"))
                        tag = tag.Replace("postalValue", locationData[0].Postal);

                    if (tag.Contains("stateValue"))
                        tag = tag.Replace("stateValue", locationData[0].State);

                    if (tag.Contains("countryValue"))
                        tag = tag.Replace("countryValue", locationData[0].Country);

                    if (tag.Contains("languageValue"))
                        tag = tag.Replace("languageValue", locationData[0].Language);

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
