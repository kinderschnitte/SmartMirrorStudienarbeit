using System.Collections.Generic;
using SmartMirrorServer.Enums.RequestEnums;

namespace SmartMirrorServer.Objects
{
    internal class Request
    {
        /// <summary>
        /// HTTP Request Typ des Querys
        /// </summary>
        public HttpRequestTyp Typ { get; set; }

        /// <summary>
        /// Query des HTTP Requests
        /// </summary>
        public Query Query { get; set; }

        /// <summary>
        /// HTTP Version des Headers
        /// </summary>
        public HttpVersion Version { get; set; }

        /// <summary>
        /// Host der Anfrage (immer SmartHomeWebServer)
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// User Agent des Klienten
        /// </summary>
        public List<UserAgent> UserAgents { get; }

        /// <summary>
        /// Formate, welche vom Klienten akzeptiert werden
        /// </summary>
        public List<AcceptResponseFormat> Formats { get; }

        /// <summary>
        /// Vom Klienten akzpierte Sprachen
        /// </summary>
        public List<AcceptResponseLanguage> Languages { get; }

        /// <summary>
        /// Vom Klienten akzeptierte Encodings
        /// </summary>
        public List<AcceptResponseEncoding> Encodings { get; }

        /// <summary>
        /// Vom Klienten erwartete Charsets
        /// </summary>
        public List<Charset> Charsets { get; }

        /// <summary>
        /// Intervall, über welches die Verbindung aufrecht erhalten bleiben soll
        /// </summary>
        public int KeepAliveInterval { get; set; }

        /// <summary>
        /// Art der Verbindung zwischen Server und Klient
        /// </summary>
        public ConnectionTyp Connection { get; set; }

        /// <summary>
        /// Cookie des Klienten
        /// </summary>
        public string Cookie { get; set; }

        /// <summary>
        /// Klient erlaubt getrackt zu werden
        /// </summary>
        public bool Dnt { get; set; }

        /// <summary>
        /// Falscher oder richtiger Request
        /// </summary>
        public bool IsInvalidRequest { get; set; }

        /// <summary>
        /// Länge der gesendeten Nachricht
        /// </summary>
        public int ContentLength { get; set; }

        /// <summary>
        /// Herkunft der HTML Seite
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// Klient zeigt an, dass er auch eine verschnlüsselte Verbindung unterstützt
        /// </summary>
        public bool UpgradeInsecureRequest { get; set; }

        /// <summary>
        /// Seite, über die der Benutzer auf die aktuelle Seite gekommen ist
        /// </summary>
        public string Referer { get; set; }

        /// <summary>
        /// Art des Inhaltes
        /// </summary>
        public List<ContentType> ContentType { get; }

        /// <summary>
        /// Standaard konstruktor der Klasse
        /// </summary>
        public Request()
        {
            UserAgents = new List<UserAgent>();
            Formats = new List<AcceptResponseFormat>();
            Languages = new List<AcceptResponseLanguage>();
            Encodings = new List<AcceptResponseEncoding>();
            Charsets = new List<Charset>();
            ContentType = new List<ContentType>();
        }
    }
}