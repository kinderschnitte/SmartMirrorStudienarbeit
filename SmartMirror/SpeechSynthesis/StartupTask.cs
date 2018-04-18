using System;
using Windows.ApplicationModel.Background;
using Windows.System.Threading;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace SpeechSynthesis
{
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
                SpeechService speechService = new SpeechService();
            });
        }

        #endregion Public Methods
    }
}
