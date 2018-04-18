using System;
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
        #region Private Fields

        private readonly SpeechRecognition.SpeechRecognition speechRecognition;

        #endregion Private Fields

        #region Public Constructors

        public MainPage()
        {
            InitializeComponent();

            Loaded += onLoaded;

            Unloaded += onUnloaded;

            CoreDispatcher dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;

            speechRecognition = new SpeechRecognition.SpeechRecognition(this, dispatcher);

            SpeechService = new SpeechService(this, dispatcher);
        }

        #endregion Public Constructors

        #region Public Properties

        public SpeechService SpeechService { get; }

        #endregion Public Properties

        #region Private Methods

        private async void onLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            speechRecognition.StartRecognizing();

            Browser.Navigate(new Uri("ms-appx-web:///loading.html"));

            await Task.Delay(TimeSpan.FromSeconds(60));
            Browser.Navigate(new Uri("http://localhost/home.html"));
        }

        private void onUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            speechRecognition.StopRecognizing();
        }

        #endregion Private Methods
    }
}
