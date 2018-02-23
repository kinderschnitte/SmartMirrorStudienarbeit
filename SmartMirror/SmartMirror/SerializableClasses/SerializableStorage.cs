using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.Storage;

namespace SmartMirror.SerializableClasses
{
    /// <summary>
    /// Provides functions to save and load single object as well as List of 'T' using serialization
    /// </summary>
    /// <typeparam name="T">Type parameter to be serialize</typeparam>
    public static class SerializableStorage<T> where T : new()
    {
        public static async void Save(string fileName, T data)
        {
            MemoryStream memoryStream = new MemoryStream();
            DataContractSerializer serializer = new DataContractSerializer(typeof(T));
            serializer.WriteObject(memoryStream, data);

            Task.WaitAll();

            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

            using (Stream fileStream = await file.OpenStreamForWriteAsync())
            {
                memoryStream.Seek(0, SeekOrigin.Begin);
                await memoryStream.CopyToAsync(fileStream);
                await fileStream.FlushAsync();
                fileStream.Dispose();
            }
        }

        public static async Task<T> Load(string fileName)
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;

            try
            {
                Task.WaitAll();
                StorageFile file = await folder.GetFileAsync(fileName);

                T result;

                using (Stream stream = await file.OpenStreamForReadAsync())
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(T));

                    result = (T)serializer.ReadObject(stream);

                }

                return result;
            }
            catch (Exception)
            {
                return new T();
            }
        }
    }

}