using Windows.UI.Xaml;

namespace SmartMirror.Modules
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class Weather
    {
        public Weather()
        {
            InitializeComponent();
  
            Loaded += onLoaded;

            Unloaded += onUnloaded;
        }

        private void onUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {

        }

        private void onLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            loadDate();
        }

        private void loadDate()
        {
            //weatherImage.Source = ;
        }
    }
}
