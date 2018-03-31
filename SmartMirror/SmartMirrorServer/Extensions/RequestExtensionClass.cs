using System;
using System.Linq;

using SmartMirrorServer.Enums.QueryEnums;
using SmartMirrorServer.Enums.RequestEnums;
using SmartMirrorServer.Objects;

namespace SmartMirrorServer.Extensions
{
    internal static class RequestExtensionClass
    {
        #region Public Methods

        /// <summary>
        /// Setzt den Verbindungstyp des Requests
        /// </summary>
        /// <param name="request"></param>
        /// <param name="connection"></param>
        public static void SetConnection(this Request request, string connection)
        {
            switch (connection.Split(' ')[1])
            {
                case "keep-alive":
                    request.Connection = ConnectionTyp.KEEP_ALIVE;
                    break;

                case "Keep-Alive":
                    request.Connection = ConnectionTyp.KEEP_ALIVE;
                    break;

                case "close":
                    request.Connection = ConnectionTyp.CLOSE;
                    break;

                default:
                    request.Connection = ConnectionTyp.UNKNOWN;
                    request.IsInvalidRequest = true;
                    break;
            }
        }

        /// <summary>
        /// Setzt die Länge der gesendeten Nachricht
        /// </summary>
        /// <param name="request"></param>
        /// <param name="contentLength"></param>
        public static void SetContentLength(this Request request, string contentLength)
        {
            request.ContentLength = Convert.ToInt32(contentLength.Split(' ')[1]);
        }

        /// <summary>
        /// Setzt die Art des Kontents
        /// </summary>
        /// <param name="request"></param>
        /// <param name="contentType"></param>
        public static void SetContentType(this Request request, string contentType)
        {
            if (contentType.Contains("application/x-www-form-urlencoded"))
                request.ContentType.Add(ContentType.X_WWW_FORM_URLENCODED);
        }

        /// <summary>
        /// Setzt das Cookie des Requests
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cookie"></param>
        public static void SetCookie(this Request request, string cookie)
        {
            request.Cookie = cookie.Substring(8);
        }

        /// <summary>
        /// Setzt DO NOT TRACK des Requests
        /// </summary>
        /// <param name="request"></param>
        /// <param name="dnt"></param>
        public static void SetDnt(this Request request, string dnt)
        {
            request.Dnt = !dnt.Contains("0");
        }

        /// <summary>
        /// Setzt die akzeptierten Encondings des Klienten
        /// </summary>
        /// <param name="request"></param>
        /// <param name="encodings"></param>
        public static void SetEncodings(this Request request, string encodings)
        {
            if (encodings.Contains("gzip"))
                request.Encodings.Add(AcceptResponseEncoding.GZIP);

            if (encodings.Contains("deflate"))
                request.Encodings.Add(AcceptResponseEncoding.DEFLATE);
        }

        /// <summary>
        /// Setzt die vom Klienten akzeptierte Formate des Requests
        /// </summary>
        /// <param name="request"></param>
        /// <param name="formats"></param>
        public static void SetFormats(this Request request, string formats)
        {
            if (formats.Contains("/html"))
                request.Formats.Add(AcceptResponseFormat.HTML);

            if (formats.Contains("/xhtml+xml"))
                request.Formats.Add(AcceptResponseFormat.XHTML_PLUS_XML);

            if (formats.Contains("/xml"))
                request.Formats.Add(AcceptResponseFormat.XML);

            if (formats.Contains("/webp"))
                request.Formats.Add(AcceptResponseFormat.WEBP);

            if (formats.Contains("/apng"))
                request.Formats.Add(AcceptResponseFormat.APNG);
        }

        /// <summary>
        /// Setzt die IP des Hostes, welche übergeben wurde
        /// </summary>
        /// <param name="request"></param>
        /// <param name="host"></param>
        public static void SetHost(this Request request, string host)
        {
            request.Host = host.Substring(host.IndexOf(' ') + 1);
        }

        /// <summary>
        /// Setzt die akzeptierten Sprachen des Klienten
        /// </summary>
        /// <param name="request"></param>
        /// <param name="languages"></param>
        public static void SetLanguages(this Request request, string languages)
        {
            if (languages.Contains("en-GB"))
                request.Languages.Add(AcceptResponseLanguage.EN_GB);

            if (languages.Contains("en-US"))
                request.Languages.Add(AcceptResponseLanguage.EN_US);

            if (languages.Contains("de"))
                request.Languages.Add(AcceptResponseLanguage.DE);
        }

        /// <summary>
        /// Setzt die Herkunft der HTML Seite
        /// </summary>
        /// <param name="request"></param>
        /// <param name="origin"></param>
        public static void SetOrigin(this Request request, string origin)
        {
            request.Origin = origin.Split(' ')[1];
        }

        /// <summary>
        /// Setzt das Query des Requests
        /// </summary>
        /// <param name="request"></param>
        /// <param name="queryString"></param>
        public static void SetQuery(this Request request, string queryString)
        {
            string finalQuery = queryString.GetBetween("/", "HTTP");

            Query query = new Query { CompleteQuery = finalQuery };

            if (string.IsNullOrEmpty(finalQuery))
            {
                request.IsInvalidRequest = true;
                query.FileName = FileName.UNKNOWN;
                query.FilePath = "";
                query.FileType = FileType.UNKNOWN;
                request.Query = query;

                return;
            }

            if (finalQuery == " ")
            {
                query.FileName = FileName.HOME;
                query.FileType = FileType.HTML;
                query.FilePath = "";
                request.Query = query;
            }
            else
            {
                string[] splittedQuery = finalQuery.Split(new[] { '/', '?', '&' }, StringSplitOptions.RemoveEmptyEntries);

                string path = splittedQuery[0].Trim();
                query.FilePath = path;

                string[] splittedFile = splittedQuery[0].Trim().Split('.');

                switch (splittedFile[0])
                {
                    case "settings":
                        query.FileName = FileName.SETTINGS;
                        break;

                    case "home":
                        query.FileName = FileName.HOME;
                        break;

                    case "time":
                        query.FileName = FileName.TIME;
                        break;

                    case "weather":
                        query.FileName = FileName.WEATHER;
                        break;

                    case "weatherforecast":
                        query.FileName = FileName.WEATHERFORECAST;
                        break;

                    case "light":
                        query.FileName = FileName.LIGHT;
                        break;

                    case "quote":
                        query.FileName = FileName.QUOTE;
                        break;

                    case "news":
                        query.FileName = FileName.NEWS;
                        break;

                    default:
                        query.FileName = FileName.UNKNOWN;
                        break;
                }

                switch (splittedFile[1])
                {
                    case "html":
                        query.FileType = FileType.HTML;
                        break;

                    case "jpg":
                        query.FileType = FileType.JPEG;
                        break;

                    case "png":
                        query.FileType = FileType.PNG;
                        break;

                    case "ico":
                        query.FileType = FileType.ICON;
                        break;

                    default:
                        query.FileType = FileType.UNKNOWN;
                        request.IsInvalidRequest = true;
                        break;
                }

                if (splittedQuery.Length == 1)
                    request.Query = query;
                else
                {
                    foreach (string parameter in splittedQuery.Skip(1).ToArray())
                        query.Parameters.Add(parameter.Trim());

                    request.Query = query;
                }
            }
        }

        /// <summary>
        /// Setzt die Seite, über der der Klient auf seine aktuelle Seite gekommen ist
        /// </summary>
        /// <param name="request"></param>
        /// <param name="referer"></param>
        public static void SetReferer(this Request request, string referer)
        {
            request.Referer = referer.Split(' ')[1];
        }

        /// <summary>
        /// Setzt des Request Typ des übergebenen Requests mit dem übergebenen Request Typ
        /// </summary>
        /// <param name="request"></param>
        /// <param name="requestTyp"></param>
        public static void SetRequestTyp(this Request request, string requestTyp)
        {
            switch (requestTyp.Split(' ')[0])
            {
                case "GET":
                    request.Typ = HttpRequestTyp.GET;
                    break;

                case "POST":
                    request.Typ = HttpRequestTyp.POST;
                    break;

                default:
                    request.Typ = HttpRequestTyp.UNKNOWN;
                    request.IsInvalidRequest = true;
                    break;
            }
        }
        /// <summary>
        /// Setzt den Boolean, ob der Klient auch eine verschnlüsselte Verbindung unterstützt
        /// </summary>
        /// <param name="request"></param>
        /// <param name="upgradeInsecureRequest"></param>
        public static void SetUpgradeInsecureRequests(this Request request, string upgradeInsecureRequest)
        {
            request.UpgradeInsecureRequest = !upgradeInsecureRequest.Contains("0");
        }

        /// <summary>
        /// Setzt die User Agents des Requests
        /// </summary>
        /// <param name="request"></param>
        /// <param name="userAgents"></param>
        public static void SetUserAgents(this Request request, string userAgents)
        {
            if (userAgents.Contains("Mozilla/"))
                request.UserAgents.Add(UserAgent.MOZILLA);

            if (userAgents.Contains("AppleWebKit/"))
                request.UserAgents.Add(UserAgent.APPLEWEBKIT);

            if (userAgents.Contains("Chrome/"))
                request.UserAgents.Add(UserAgent.CHROME);

            if (userAgents.Contains("Safari/"))
                request.UserAgents.Add(UserAgent.SAFARI);

            if (userAgents.Contains("Gecko/"))
                request.UserAgents.Add(UserAgent.GECKO);

            if (userAgents.Contains("Firefox/"))
                request.UserAgents.Add(UserAgent.FIREFOX);
        }

        /// <summary>
        /// Setzt die Version des Requests
        /// </summary>
        /// <param name="request"></param>
        /// <param name="version"></param>
        public static void SetVersion(this Request request, string version)
        {
            if (version.Contains("\0"))
            {
                request.Version = HttpVersion.UNKNOWN;
                request.IsInvalidRequest = true;
                return;
            }

            switch (version.Substring(version.IndexOf("HTTP", StringComparison.Ordinal)))
            {
                case "HTTP/1.0":
                    request.Version = HttpVersion.HTTP_1_0;
                    break;

                case "HTTP/1.1":
                    request.Version = HttpVersion.HTTP_1_1;
                    break;

                case "HTTP/2.0":
                    request.Version = HttpVersion.HTTP_2_0;
                    break;

                default:
                    request.Version = HttpVersion.UNKNOWN;
                    request.IsInvalidRequest = true;
                    break;
            }
        }

        #endregion Public Methods
    }
}