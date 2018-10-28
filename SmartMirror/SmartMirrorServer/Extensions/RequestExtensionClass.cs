using System;
using System.Linq;
using System.Text.RegularExpressions;

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
                    request.Connection = ConnectionTyp.KeepAlive;
                    break;

                case "Keep-Alive":
                    request.Connection = ConnectionTyp.KeepAlive;
                    break;

                case "close":
                    request.Connection = ConnectionTyp.Close;
                    break;

                default:
                    request.Connection = ConnectionTyp.Unknown;
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
                request.ContentType.Add(ContentType.XWwwFormUrlencoded);
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
        /// Setzt den Post Query des Requests
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        public static void SetPostQuery(this Request request, string query)
        {
            string realPostQuery = query.Replace("\0", "");

            //realPostQuery = Regex.Replace(realPostQuery, @"[\d-]", string.Empty);

            PostQuery postQuery = new PostQuery();

            if (realPostQuery == "")
                postQuery.CompleteQuery = "";
            else
            {
                string[] splittedPostQuery = realPostQuery.Split(new[]{ "&" }, StringSplitOptions.None);

                postQuery.CompleteQuery = realPostQuery;

                foreach (string parameter in splittedPostQuery)
                {
                    string[] splittedParameter = parameter.Split('=');
                    postQuery.Value.Add(splittedParameter[0], splittedParameter[1]);
                }
            }

            request.PostQuery = postQuery;
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
                request.Encodings.Add(AcceptResponseEncoding.Gzip);

            if (encodings.Contains("deflate"))
                request.Encodings.Add(AcceptResponseEncoding.Deflate);
        }

        /// <summary>
        /// Setzt die vom Klienten akzeptierte Formate des Requests
        /// </summary>
        /// <param name="request"></param>
        /// <param name="formats"></param>
        public static void SetFormats(this Request request, string formats)
        {
            if (formats.Contains("/html"))
                request.Formats.Add(AcceptResponseFormat.Html);

            if (formats.Contains("/xhtml+xml"))
                request.Formats.Add(AcceptResponseFormat.XhtmlPlusXml);

            if (formats.Contains("/xml"))
                request.Formats.Add(AcceptResponseFormat.Xml);

            if (formats.Contains("/webp"))
                request.Formats.Add(AcceptResponseFormat.Webp);

            if (formats.Contains("/apng"))
                request.Formats.Add(AcceptResponseFormat.Apng);
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
                request.Languages.Add(AcceptResponseLanguage.EnGb);

            if (languages.Contains("en-US"))
                request.Languages.Add(AcceptResponseLanguage.EnUs);

            if (languages.Contains("de"))
                request.Languages.Add(AcceptResponseLanguage.De);
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
                query.FileName = FileName.Unknown;
                query.FilePath = "";
                query.FileType = FileType.Unknown;
                request.Query = query;

                return;
            }

            if (finalQuery == " ")
            {
                query.FileName = FileName.Home;
                query.FileType = FileType.Html;
                query.FilePath = "";
                request.Query = query;
            }
            else
            {
                string[] splittedQuery = finalQuery.Split(new[] { '/', '?', '&' }, StringSplitOptions.RemoveEmptyEntries);

                string path = splittedQuery[0].Trim();
                query.FilePath = path;

                int pointindex = path.LastIndexOf('.');

                if (pointindex != -1)
                {
                    switch (path.Substring(0, pointindex))
                    {
                        case "settings":
                            query.FileName = FileName.Settings;
                            break;

                        case "home":
                            query.FileName = FileName.Home;
                            break;

                        case "time":
                            query.FileName = FileName.Time;
                            break;

                        case "weather":
                            query.FileName = FileName.Weather;
                            break;

                        case "weatherforecast":
                            query.FileName = FileName.Weatherforecast;
                            break;

                        case "light":
                            query.FileName = FileName.Light;
                            break;

                        case "quote":
                            query.FileName = FileName.Quote;
                            break;

                        case "news":
                            query.FileName = FileName.News;
                            break;

                        case "help":
                            query.FileName = FileName.Help;
                            break;

                        default:
                            query.FileName = FileName.Unknown;
                            break;
                    }

                    if (path.Substring(pointindex + 1).Length > 1)
                    {
                        switch (path.Substring(pointindex + 1))
                        {
                            case "html":
                                query.FileType = FileType.Html;
                                break;

                            case "jpg":
                                query.FileType = FileType.Jpeg;
                                break;

                            case "png":
                                query.FileType = FileType.Png;
                                break;

                            case "ico":
                                query.FileType = FileType.Icon;
                                break;

                            case "css":
                                query.FileType = FileType.Css;
                                break;

                            case "js":
                                query.FileType = FileType.Js;
                                break;

                            default:
                                query.FileType = FileType.Unknown;
                                request.IsInvalidRequest = true;
                                break;
                        }
                    }
                    else
                    {
                        query.FileType = FileType.Unknown;
                        request.IsInvalidRequest = true;
                    }
                }
                else
                {
                    query.FileName = FileName.Unknown;
                    query.FileType = FileType.Unknown;
                    request.IsInvalidRequest = true;
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
                    request.Typ = HttpRequestTyp.Get;
                    break;

                case "POST":
                    request.Typ = HttpRequestTyp.Post;
                    break;

                default:
                    request.Typ = HttpRequestTyp.Unknown;
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
                request.UserAgents.Add(UserAgent.Mozilla);

            if (userAgents.Contains("AppleWebKit/"))
                request.UserAgents.Add(UserAgent.Applewebkit);

            if (userAgents.Contains("Chrome/"))
                request.UserAgents.Add(UserAgent.Chrome);

            if (userAgents.Contains("Safari/"))
                request.UserAgents.Add(UserAgent.Safari);

            if (userAgents.Contains("Gecko/"))
                request.UserAgents.Add(UserAgent.Gecko);

            if (userAgents.Contains("Firefox/"))
                request.UserAgents.Add(UserAgent.Firefox);
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
                request.Version = HttpVersion.Unknown;
                request.IsInvalidRequest = true;
                return;
            }

            switch (version.Substring(version.IndexOf("HTTP", StringComparison.Ordinal)))
            {
                case "HTTP/1.0":
                    request.Version = HttpVersion.Http10;
                    break;

                case "HTTP/1.1":
                    request.Version = HttpVersion.Http11;
                    break;

                case "HTTP/2.0":
                    request.Version = HttpVersion.Http20;
                    break;

                default:
                    request.Version = HttpVersion.Unknown;
                    request.IsInvalidRequest = true;
                    break;
            }
        }

        #endregion Public Methods
    }
}