using System;
using System.Text;
using Windows.Networking.Sockets;
using SmartMirrorServer.Objects;

namespace SmartMirrorServer.Extensions
{
    internal static class StringBuilderExtensionClass
    {
        #region Public Methods

        /// <summary>
        /// Bildet eine Request Objekt passend zum übergebenen Request des Klienten
        /// </summary>
        /// <param name="requestStringBuilder"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Request GetRequest(this StringBuilder requestStringBuilder, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            Request request = new Request();

            if (requestStringBuilder.ToString().Length < 2)
            {
                request.IsInvalidRequest = true;

                return request;
            }

            try
            {
                string[] stringSeparators = {"\r\n"};
                string[] requestHeader = requestStringBuilder.ToString().Split(stringSeparators, StringSplitOptions.None);

                request.SetRequestTyp(requestHeader[0]);
                request.SetQuery(requestHeader[0]);
                request.SetVersion(requestHeader[0]);

                foreach (string field in requestHeader)
                {
                    if (field.Contains("Host:"))
                        request.SetHost(field);
                    else if (field.Contains("Connection:"))
                        request.SetConnection(field);
                    else if (field.Contains("User-Agent:"))
                        request.SetUserAgents(field);
                    else if (field.Contains("Accept:"))
                        request.SetFormats(field);
                    else if (field.Contains("DNT:"))
                        request.SetDnt(field);
                    else if (field.Contains("Accept-Encoding:"))
                        request.SetEncodings(field);
                    else if (field.Contains("Accept-Language:"))
                        request.SetLanguages(field);
                    else if (field.Contains("Cookie:"))
                        request.SetCookie(field);
                    else if (field.Contains("Content-Length:"))
                        request.SetContentLength(field);
                    else if (field.Contains("Origin:"))
                        request.SetOrigin(field);
                    else if (field.Contains("Upgrade-Insecure-Requests:"))
                        request.SetUpgradeInsecureRequests(field);
                    else if (field.Contains("Content-Type:"))
                        request.SetContentType(field);
                    else if (field.Contains("Referer:"))
                        request.SetReferer(field);
                    else if (field.Contains("\0"))
                        request.SetPostQuery(requestHeader[requestHeader.Length - 1]);
                }

                return request;
            }
            catch (Exception exception)
            {
                if (Application.Notifications.ExceptionNotifications)
                    Notification.Notification.SendPushNotification("Fehler aufgetreten.", $"{exception.StackTrace}");
            }

            request.IsInvalidRequest = true;

            return request;
        }

        #endregion Public Methods
    }
}