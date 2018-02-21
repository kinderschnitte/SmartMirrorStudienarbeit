using System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace SmartMirror
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class MainPage
    {
        private WebView browser;

        private readonly SpeechRecognition.SpeechRecognition speechRecognition;

        private readonly CoreDispatcher dispatcher;

        public MainPage()
        {
            InitializeComponent();

            Loaded += onLoaded;

            Unloaded += onUnloaded;

            dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;

            speechRecognition = new SpeechRecognition.SpeechRecognition(dispatcher, browser);
        }

        private void onUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            speechRecognition.StopRecognizing();
        }

        private void onLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            browser = new WebView();
            RotateTransform rotateTransform = new RotateTransform {Angle = 90};
            browser.RenderTransform = rotateTransform;
            grid.Children.Add(browser);

            Uri timeUri = new Uri("http://localhost/MagicMirror/time.html");
            browser.Navigate(timeUri);

            speechRecognition.StartRecognizing();
        }
    }
}
