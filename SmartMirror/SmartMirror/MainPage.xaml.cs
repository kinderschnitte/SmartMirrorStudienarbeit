using Windows.UI.Core;
using Windows.UI.Xaml;
using SmartMirror.SerializableClasses;

namespace SmartMirror
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class MainPage
    {
        private readonly SpeechRecognition.SpeechRecognition speechRecognition;

        private readonly CoreDispatcher dispatcher;

        private StorageData storageData;

        public MainPage()
        {
            InitializeComponent();
  
            Loaded += onLoaded;

            Unloaded += onUnloaded;

            dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;

            speechRecognition = new SpeechRecognition.SpeechRecognition(dispatcher);
        }

        private void onUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            speechRecognition.StopRecognizing();
            SerializableStorage<StorageData>.Save("StorageData.dat", storageData);
        }

        private void onLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            loadModuls();

            speechRecognition.StartRecognizing();
        }

        private async void loadModuls()
        {
            storageData = await SerializableStorage<StorageData>.Load("StorageData.dat");
        }
    }
}
