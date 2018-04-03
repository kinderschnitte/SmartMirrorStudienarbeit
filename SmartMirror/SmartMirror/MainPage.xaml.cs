using System;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace SmartMirror
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    internal partial class MainPage
    {
        private readonly SpeechRecognition.SpeechRecognition speechRecognition;
        public SpeechService.SpeechService SpeechService { get; }

        public MainPage()
        {
            InitializeComponent();

            Loaded += onLoaded;

            Unloaded += onUnloaded;

            CoreDispatcher dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;

            speechRecognition = new SpeechRecognition.SpeechRecognition(this, dispatcher);

            SpeechService = new SpeechService.SpeechService();
        }

        private void onUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            speechRecognition.StopRecognizing();
        }

        private async void onLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            speechRecognition.StartRecognizing();

            await Task.Delay(TimeSpan.FromSeconds(10));
            Browser.Navigate(new Uri("http://localhost/home.html"));
        }
    }
}
