using System;
using Windows.Security.ExchangeActiveSyncProvisioning;
using SmartMirrorServer.Objects;

namespace SmartMirrorServer
{
    public static class Application
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

        #endregion Public Properties
    }
}