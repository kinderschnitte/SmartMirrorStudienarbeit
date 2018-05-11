using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLibrary;
using DataAccessLibrary.Module;
using NewsAPI.Models;
using SmartMirrorServer.HelperClasses;
using SmartMirrorServer.Objects;

namespace SmartMirrorServer.RequestHandler.Sites
{
    internal static class News
    {
        #region Public Methods

        /// <summary>
        /// Bildet Home Seite und gibt diese zurück
        /// </summary>
        /// <returns></returns>
        public static async Task<byte[]> BuildNews(Request request)
        {
            string page = string.Empty;

            try
            {
                IEnumerable<string> file = await FileHelperClass.LoadFileFromStorage("SmartMirrorServer\\Websites\\news.html");

                foreach (string line in file)
                {
                    string tag = line;

                    if (tag.Contains("News"))
                        tag = tag.Replace("News", await getNews(request));

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

        private static async Task<string> getNews(Request request)
        {
            ArticlesResult result = null;

            if (request.Query.Parameters.Contains("Business"))
                result = DataAccess.DeserializeModuleData(typeof(ArticlesResult), await DataAccess.GetModuleData(Modules.NEWSBUSINESS));
            else if (request.Query.CompleteQuery.Contains("Entertainment"))
                result = DataAccess.DeserializeModuleData(typeof(ArticlesResult), await DataAccess.GetModuleData(Modules.NEWSENTERTAINMENT));
            else if (request.Query.CompleteQuery.Contains("Health"))
                result = DataAccess.DeserializeModuleData(typeof(ArticlesResult), await DataAccess.GetModuleData(Modules.NEWSHEALTH));
            else if (request.Query.CompleteQuery.Contains("Science"))
                result = DataAccess.DeserializeModuleData(typeof(ArticlesResult), await DataAccess.GetModuleData(Modules.NEWSSCIENCE));
            else if (request.Query.CompleteQuery.Contains("Sports"))
                result = DataAccess.DeserializeModuleData(typeof(ArticlesResult), await DataAccess.GetModuleData(Modules.NEWSSPORT));
            else if (request.Query.CompleteQuery.Contains("Technology"))
                result = DataAccess.DeserializeModuleData(typeof(ArticlesResult), await DataAccess.GetModuleData(Modules.NEWSTECHNOLOGY));

            if (result == null)
                return string.Empty;

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("<table style=\"width: 100%; height: 60%; padding: 2.5%; display: table; color: white;\"> <col width=\"20%;\"> <col width=\"80%;\">");

            foreach (Article article in result.Articles.Take(11))
                stringBuilder.Append($" <tr onclick=\"window.location='{article.Url}'\"> <td style=\"padding-top: 2.5%; vertical-align: top;\"><image src=\"{article.UrlToImage}\" alt=\"\" style=\"width: 100%; max-height: 150px;\"/></td> <td style=\"vertical-align: top; padding-top: 2.5%; padding-left: 2.5%;\"> <label style=\"display: block; font-size: 1.25em; cursor: pointer; overflow: hidden; text-align: left; color: white;\"><bold>{article.Title}</bold></label> <label style=\"display: block; text-align: left; font-size: 0.6em; cursor: pointer; color: white; padding-top: 0.5%; padding-bottom: 0.5%;\">{article.PublishedAt?.ToString("f")}</label> <label style=\"display: block; text-align: left; font-size: 1em; cursor: pointer; color: white;\">{article.Description}</label> </td> </tr>");

            stringBuilder.Append(" </table>");

            return stringBuilder.ToString();
        }

        #endregion Public Methods
    }
}