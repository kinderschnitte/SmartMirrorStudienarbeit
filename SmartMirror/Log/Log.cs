using System;
using System.Text;
using Windows.Storage;

namespace Log
{
    public static class Log
    {
        static Log()
        {
            CreateLogFile();
        }

        private static async void CreateLogFile()
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;

            await storageFolder.CreateFileAsync("log.txt", CreationCollisionOption.OpenIfExists);
        }

        public static async void WriteLog(Exception exception)
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile logFile = await storageFolder.GetFileAsync("log.txt");

            StringBuilder message = new StringBuilder();

            message.AppendLine("<---------------" + DateTime.Now.ToString("G") + "--------------->");
            message.AppendLine("Message: " + exception.Message);
            message.AppendLine("Stacktrace: " + exception.StackTrace);
            message.AppendLine("");

            await FileIO.WriteTextAsync(logFile, message.ToString());
        }
    }
}
