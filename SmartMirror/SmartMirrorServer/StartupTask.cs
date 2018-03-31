using System;
using Windows.ApplicationModel.Background;
using Windows.System.Threading;

namespace SmartMirrorServer
{
    // ReSharper disable once UnusedMember.Global
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class StartupTask : IBackgroundTask
    {
        #region Private Fields

        // ReSharper disable once NotAccessedField.Local
        private static BackgroundTaskDeferral deferral;

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// Startet den Smart Home Webserver
        /// </summary>
        /// <param name="taskInstance"></param>
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            deferral = taskInstance.GetDeferral();

            await ThreadPool.RunAsync(workItem =>
            {
                SmartMirrorServer.Start();
            });
        }

        public static void SaveData()
        {
            Application.SaveStorageData();
        }

        #endregion Public Methods
    }
}
