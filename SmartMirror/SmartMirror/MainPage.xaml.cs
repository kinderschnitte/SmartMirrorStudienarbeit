using System;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using SmartMirror.SpeechSynthesis;

namespace SmartMirror
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    internal partial class MainPage
    {
        #region Public Constructors

        public MainPage()
        {
            InitializeComponent();

            Loaded += onLoaded;

            Unloaded += onUnloaded;

            CoreDispatcher dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;

            SpeechRecognition = new SpeechRecognition.SpeechRecognition(this, dispatcher);

            SpeechService = new SpeechService(this, dispatcher);
        }

        #endregion Public Constructors

        #region Public Properties

        public SpeechRecognition.SpeechRecognition SpeechRecognition { get; }

        public SpeechService SpeechService { get; }

        #endregion Public Properties

        #region Private Methods

        private async void onLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            await SpeechRecognition.StartRecognizing();

            //await SpeechService.Startup();

            await waitingForServerToStart();

            Browser.Navigate(new Uri("http://localhost/home.html"));
        }

        private static async Task waitingForServerToStart()
        {
            using (HttpClient client = new HttpClient())
            {
                const string url = "http://localhost/light.html";
                try
                {
                    string response = await client.GetStringAsync(url);

                    if (string.IsNullOrEmpty(response))
                        await Task.Delay(5000);
                }
                catch (Exception)
                {
                    await waitingForServerToStart();
                }
            }
        }

        private void onUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            SpeechRecognition.StopRecognizing();
        }

        #endregion Private Methods

    }
}
