using System;
using Windows.System.Threading;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;
using Api;

namespace SmartMirror
{
    internal partial class MainPage
    {

        #region Private Fields

        private readonly CoreDispatcher dispatcher;
        private readonly SpeechRecognition.SpeechRecognition speechRecognition;

        #endregion Private Fields

        #region Public Constructors

        public MainPage()
        {
            InitializeComponent();

            Loaded += onLoaded;

            Unloaded += onUnloaded;

            dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;

            speechRecognition = new SpeechRecognition.SpeechRecognition(this, dispatcher);
        }

        #endregion Public Constructors

        #region Public Methods

        public async void StartColorAnimation(DependencyObject control, string property, Color from, Color to, double timeSpanInSecond = 1)
        {
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ColorAnimation opacityAnimation = new ColorAnimation { From = from, To = to, AutoReverse = true };

                TimeSpan timeSpan = TimeSpan.FromSeconds(timeSpanInSecond);
                opacityAnimation.Duration = timeSpan;

                Storyboard storyboard = new Storyboard();
                storyboard.Children.Add(opacityAnimation);

                Storyboard.SetTargetProperty(opacityAnimation, property);
                Storyboard.SetTarget(storyboard, control);
                storyboard.Begin();
            });
        }

        #endregion Public Methods

        #region Private Methods

        private async void onLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            await ApiData.GetApiData();

            await Speechservice.SpeechService.Startup();

            Browser.Navigate(new Uri("http://localhost/home.html"));

            speechRecognition.StartRecognizing();

            TimeSpan period = TimeSpan.FromMinutes(30);
            ThreadPoolTimer.CreatePeriodicTimer(async source => { await ApiData.GetApiData(); }, period);
        }

        private void onUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            speechRecognition.StopRecognizing();
        }

        #endregion Private Methods

    }
}
