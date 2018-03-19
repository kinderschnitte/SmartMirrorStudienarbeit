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

        public MainPage()
        {
            InitializeComponent();

            Loaded += onLoaded;

            Unloaded += onUnloaded;

            CoreDispatcher dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;

            speechRecognition = new SpeechRecognition.SpeechRecognition(dispatcher);
        }

        private void onUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            speechRecognition.StopRecognizing();
        }

        private void onLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            speechRecognition.StartRecognizing();
        }
    }
}
