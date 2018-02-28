namespace SmartMirrorServer.Objects
{
    public sealed class Notifications
    {
        /// <summary>
        /// Bei Start des Systems
        /// </summary>
        public bool SystemStartNotifications { get; }

        /// <summary>
        /// Beim Auftreten von Systemfehlern eine Nachricht versenden
        /// </summary>
        public bool ExceptionNotifications { get; }

        /// <summary>
        /// Standard Konstruktor der Klasse
        /// </summary>
        public Notifications()
        {
            SystemStartNotifications = true;
            ExceptionNotifications = true;
        }
    }
}