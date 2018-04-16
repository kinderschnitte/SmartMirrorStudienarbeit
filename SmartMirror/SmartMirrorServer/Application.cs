using System;
using System.Collections.Generic;
using Windows.Security.ExchangeActiveSyncProvisioning;
using SmartMirrorServer.Enums.Api;
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

            ApiKeys = new Dictionary<Api, string>();

            ApiUrls = new Dictionary<Api, string>();

            DataUpdateMinutes = 30;

            addApiParameters();
        }

        private static void addApiParameters()
        {
            // OpenWeatherMap
            ApiKeys.Add(Api.OPENWEATHERMAP, "4ce3d25d1b8cb5953ba718abd11bd07a");
            ApiUrls.Add(Api.OPENWEATHERMAP, "http://api.openweathermap.org/data/2.5");

            // NewsAPI
            ApiKeys.Add(Api.NEWSAPI, "9d6d50c70043491ba1aa1a2048b4197a");

            // Google Maps Geocoding
            ApiKeys.Add(Api.GOOGLEMAPSGEOCODING, "AIzaSyCUWewgFV1-ALfuAQpGC3S5uHlud1ucj20");
            ApiUrls.Add(Api.GOOGLEMAPSGEOCODING, "https://maps.googleapis.com/maps/api/geocode/json?address=");
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

        public static int DataUpdateMinutes { get; }

        public static Dictionary<Api, string> ApiKeys { get; }

        public static Dictionary<Api, string> ApiUrls { get; }

        #endregion Public Properties
    }
}