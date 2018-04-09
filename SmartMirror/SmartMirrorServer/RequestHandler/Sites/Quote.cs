using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DataAccessLibrary;
using DataAccessLibrary.Module;
using SmartMirrorServer.HelperMethods;

namespace SmartMirrorServer.RequestHandler.Sites
{
    internal static class Quote
    {
        #region Public Methods

        /// <summary>
        /// Bildet Home Seite und gibt diese zurück
        /// </summary>
        /// <returns></returns>
        public static async Task<byte[]> BuildQuote()
        {
            string page = string.Empty;

            try
            {
                IEnumerable<string> file = await FileHelperClass.LoadFileFromStorage("SmartMirrorServer\\Websites\\quote.html");

                if (!Application.Data.TryGetValue(DataAccess.GetModule(Modules.QUOTE), out dynamic r))
                    return Encoding.UTF8.GetBytes(page);

                Features.Quote.Quote result = (Features.Quote.Quote)r;

                foreach (string line in file)
                {
                    string tag = line;

                    if (tag.Contains("quote") || tag.Contains("author"))
                    {
                        tag = tag.Replace("quote", result.Text);
                        tag = tag.Replace("author", result.Author);
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
