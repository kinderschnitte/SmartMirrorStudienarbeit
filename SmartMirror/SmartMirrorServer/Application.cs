using System;
using Windows.Security.ExchangeActiveSyncProvisioning;
using SmartMirrorServer.Objects;

namespace SmartMirrorServer
{
    internal static class Application
    {
        #region Public Constructors

        /// <summary>
        /// Standard Konstruktor der Klasse
        /// </summary>
        static Application()
        {
            StartTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            SystemInformation = new EasClientDeviceInformation();

            Notifications = new Notifications();

            WeatherApiUrl = "http://api.openweathermap.org/data/2.5";

            WeatherApiKey = "4ce3d25d1b8cb5953ba718abd11bd07a";

            NewsApiKey = "9d6d50c70043491ba1aa1a2048b4197a";

            DataUpdateMinutes = 30;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Startzeit des Systems
        /// </summary>
        public static string StartTime { get; }

        /// <summary>
        /// Systeminformation des Systems
        /// </summary>
        public static EasClientDeviceInformation SystemInformation { get; }

        /// <summary>
        /// Aktivitätsnachrichten
        /// </summary>
        public static Notifications Notifications { get; }

        public static string WeatherApiUrl { get; }

        public static string WeatherApiKey { get; }

        public static string NewsApiKey { get; }

        public static int DataUpdateMinutes { get; }

        #endregion Public Properties
    }
}