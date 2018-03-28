using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.System.Threading;
using NewsAPI;
using NewsAPI.Models;
using SmartMirrorServer.Enums;
using SmartMirrorServer.Enums.QueryEnums;
using SmartMirrorServer.Extensions;
using SmartMirrorServer.Objects;
using SmartMirrorServer.Objects.Moduls;
using SmartMirrorServer.Objects.Moduls.Weather;

namespace SmartMirrorServer
{
    internal class SmartMirrorServer
    {

        #region Private Fields

        /// <summary>
        /// Größe des Empfangsbuffer des Webservers
        /// </summary>
        private const uint bufferSize = 8192;

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// Startet den Server Smart Home Webserver
        /// </summary>
        public async void Start()
        {
            try
            {
                updateModules();

                TimeSpan period = TimeSpan.FromMinutes(Application.DataUpdateMinutes);
                ThreadPoolTimer.CreatePeriodicTimer(source => { updateModules(); }, period);

                StreamSocketListener listener = new StreamSocketListener();
                listener.ConnectionReceived += listener_ConnectionReceived;

                await listener.BindServiceNameAsync("80");

                CoreApplication.Properties.Add("listener", listener);

                if (Application.Notifications.SystemStartNotifications)
                    Notification.Notification.SendPushNotification("System wurde gestartet.", "Das Smart Mirror System wurde erfolgreich gestartet.");
            }
            catch (Exception exception)
            {
                if (Application.Notifications.ExceptionNotifications)
                    Notification.Notification.SendPushNotification("Fehler aufgetreten.", $"{exception.StackTrace}");
            }
        }

        #endregion Public Methods

        #region Private Methods

        private static void buildModul(Module module)
        {
            switch (module.ModuleType)
            {
                case ModuleType.TIME:
                    timeModul(module);
                    break;

                case ModuleType.WEATHER:
                    weatherModul(module);
                    break;

                case ModuleType.WEATHERFORECAST:
                    weatherforecastModul(module);
                    break;

                case ModuleType.NEWS:
                    newsModul(module);
                    break;

                case ModuleType.QUOTEOFDAY:
                    quoteOfDayModul(module);
                    break;
            }
        }

        private static List<ForecastDays> getcalculatedForecast(Module module)
        {
            List<List<FiveDaysForecastResult>> result = getFiveDaysForecastByCityName(module);

            List<ForecastDays> forecastDays = new List<ForecastDays>();

            foreach (List<FiveDaysForecastResult> fiveDaysForecastResult in result)
            {
                ForecastDays forecastDay = new ForecastDays
                {
                    City = fiveDaysForecastResult[0].City,
                    CityId = fiveDaysForecastResult[0].CityId,
                    Date = fiveDaysForecastResult[0].Date,
                    Temperature = fiveDaysForecastResult.Average(innerList => innerList.Temp),
                    MinTemp = fiveDaysForecastResult.Min(innerList => innerList.TempMin),
                    MaxTemp = fiveDaysForecastResult.Min(innerList => innerList.TempMax),
                    Icon = fiveDaysForecastResult.GroupBy(x => x.Icon).OrderByDescending(x => x.Count()).First().Key
                };

                forecastDays.Add(forecastDay);
            }

            // Infos zu heutigen Tag löschen
            if (forecastDays.Count > 5)
                forecastDays.RemoveAt(0);

            return forecastDays;
        }

        private static SingleResult<CurrentWeatherResult> getCurrentWeatherByCityName(Module module)
        {
            return CurrentWeather.GetByCityName(module.City, module.Country, module.Language, "metric");
        }

        private static List<List<FiveDaysForecastResult>> getFiveDaysForecastByCityName(Module module)
        {
            return FiveDaysForecast.GetByCityName(module.City, module.Country, module.Language, "metric");
        }

        private static ArticlesResult getNewsByCategory(Module module)
        {
            NewsApiClient newsApiClient = new NewsApiClient(Application.NewsApiKey);

            ArticlesResult topheadlines = newsApiClient.GetTopHeadlinesAsync(new TopHeadlinesRequest
            {
                Category = module.NewsCategory,
                Country = module.NewsCountry,
                Language = module.NewsLanguage
            }).Result;

            return topheadlines;
        }

        private static ArticlesResult getNewsBySource(Module module)
        {
            NewsApiClient newsApiClient = new NewsApiClient(Application.NewsApiKey);

            ArticlesResult topheadlines = newsApiClient.GetTopHeadlinesAsync(new TopHeadlinesRequest
            {
                Language = module.NewsLanguage,
                Sources = module.NewsSources
            }).Result;

            return topheadlines;
        }

        private static QuoteOfDay getQuoteOfDay()
        {
            return HelperMethods.QuoteOfDay.GetQuoteOfDay();
        }

        private static void newsModul(Module module)
        {
            Application.Data.AddOrUpdate(module, module.NewsSources == null ? getNewsByCategory(module) : getNewsBySource(module), (key, value) => module.NewsSources == null ? getNewsByCategory(module) : getNewsBySource(module));
        }

        private static void quoteOfDayModul(Module module)
        {
            Application.Data.AddOrUpdate(module, getQuoteOfDay(), (key, value) => getQuoteOfDay());
        }

        private static void timeModul(Module module)
        {
            Application.Data.AddOrUpdate(module, new Sun(module), (key, value) => new Sun(module));
        }

        private static void updateModules()
        {
            Task.Run(() =>
            {
                if (Application.StorageData.WeatherModul == null)
                    return;

                Application.Data.AddOrUpdate(Application.StorageData.WeatherModul, getCurrentWeatherByCityName(Application.StorageData.WeatherModul), (key, value) => getCurrentWeatherByCityName(Application.StorageData.WeatherModul));

                Debug.WriteLine("Wetter gesetzt.");
            });

            Task.Run(() =>
            {
                if (Application.StorageData.TimeModul == null)
                    return;

                Application.Data.AddOrUpdate(Application.StorageData.TimeModul, new Sun(Application.StorageData.TimeModul), (key, value) => new Sun(Application.StorageData.TimeModul));

                Debug.WriteLine("Zeit gesetzt.");
            });

            Task.Run(() =>
            {
                if (Application.StorageData.WeatherforecastModul == null)
                    return;

                Application.Data.AddOrUpdate(Application.StorageData.WeatherforecastModul, getFiveDaysForecastByCityName(Application.StorageData.WeatherforecastModul), (key, value) => getFiveDaysForecastByCityName(Application.StorageData.WeatherforecastModul));

                Debug.WriteLine("Wettervorhersage gesetzt.");
            });

            Task.Run(() =>
            {
                if (Application.StorageData.UpperLeftModule == null)
                    return;

                buildModul(Application.StorageData.UpperLeftModule);

                Debug.WriteLine("Module oben links gesetzt.");
            });

            Task.Run(() =>
            {
                if (Application.StorageData.UpperRightModule == null)
                    return;

                buildModul(Application.StorageData.UpperRightModule);

                Debug.WriteLine("Module oben rechts gesetzt.");
            });

            Task.Run(() =>
            {
                if (Application.StorageData.MiddleLeftModule == null)
                    return;

                buildModul(Application.StorageData.MiddleLeftModule);

                Debug.WriteLine("Module mitte links gesetzt.");
            });

            Task.Run(() =>
            {
                if (Application.StorageData.MiddleRightModule == null)
                    return;

                buildModul(Application.StorageData.MiddleRightModule);

                Debug.WriteLine("Module mitte rechts gesetzt.");
            });

            Task.Run(() =>
            {
                if (Application.StorageData.LowerLeftModule == null)
                    return;

                buildModul(Application.StorageData.LowerLeftModule);

                Debug.WriteLine("Module unten links gesetzt.");
            });

            Task.Run(() =>
            {
                if (Application.StorageData.LowerRightModule == null)
                    return;

                buildModul(Application.StorageData.LowerRightModule);

                Debug.WriteLine("Module unten rechts gesetzt.");
            });
        }
        private static void weatherforecastModul(Module module)
        {
            List<ForecastDays> result = getcalculatedForecast(module);

            Application.Data.AddOrUpdate(module, result, (key, value) => result);
        }

        private static void weatherModul(Module module)
        {
            SingleResult<CurrentWeatherResult> result = getCurrentWeatherByCityName(module);

            Application.Data.AddOrUpdate(module, result, (key, value) => result);
        }
        /// <summary>
        /// Nimmt den Request entgegen und gibt diesen als StringBuilder Objekt zurück
        /// </summary>
        /// <param name="inputStream"></param>
        /// <returns></returns>
        private async Task<StringBuilder> handleHttpRequest(IInputStream inputStream)
        {
            StringBuilder requestString = new StringBuilder();

            using (IInputStream input = inputStream)
            {
                byte[] data = new byte[bufferSize];
                IBuffer buffer = data.AsBuffer();
                uint dataRead = bufferSize;

                while (dataRead == bufferSize)
                {
                    await input.ReadAsync(buffer, bufferSize, InputStreamOptions.Partial);
                    requestString.Append(Encoding.UTF8.GetString(data, 0, data.Length));
                    dataRead = buffer.Length;
                }
            }

            return requestString;
        }

        /// <summary>
        /// Verarbeitet Anfrage und sendet passende Antwort zurück
        /// </summary>
        /// <param name="args"></param>
        /// <param name="requestString"></param>
        /// <returns></returns>
        private async Task handleHttpResponse(StreamSocketListenerConnectionReceivedEventArgs args, StringBuilder requestString)
        {
            Debug.WriteLine(requestString.ToString()); // TODO Löschen

            // Bestimmt die Anfrage
            Request request = requestString.GetRequest(args);

            // Erstellt die Response
            byte[] responseBytes = await RequestHandler.RequestHandler.BuildResponse(request);

            // Sendet Antwort an den Klienten
            await sendResponse(args.Socket.OutputStream, request, responseBytes);
        }

        /// <summary>
        /// Verarbeitet Webserver Anfragen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void listener_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            // Verarbeitet die HTTP Anfrage
            StringBuilder requestString = await handleHttpRequest(args.Socket.InputStream);

            // Erstellt die Antwort und sendet diese
            await handleHttpResponse(args, requestString);
        }

        /// <summary>
        /// Sendet Bild an den Klienten
        /// </summary>
        /// <param name="responseStream"></param>
        /// <param name="request"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        private async Task sendPicture(Stream responseStream, Request request, byte[] file)
        {
            string fileType;

            switch (request.Query.FileType)
            {
                case FileType.JPEG:
                    fileType = "jpeg";
                    break;

                case FileType.PNG:
                    fileType = "png";
                    break;

                case FileType.ICON:
                    fileType = "x-icon";
                    break;

                default:
                    fileType = "";
                    break;
            }

            using (MemoryStream bodyStream = new MemoryStream(file))
            {
                string header = $"HTTP/1.1 200 OK\r\nContent-Length: {bodyStream.Length}\r\nConnection: close\r\nContent-Type: image/{fileType}; charset=utf-8\r\n\r\n";
                byte[] headerArray = Encoding.UTF8.GetBytes(header);
                await responseStream.WriteAsync(headerArray, 0, headerArray.Length);
                await bodyStream.CopyToAsync(responseStream);
                await responseStream.FlushAsync();
            }
        }

        /// <summary>
        /// Sendet passende Antwort zum Klienten
        /// </summary>
        /// <param name="outputStream"></param>
        /// <param name="request"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        private async Task sendResponse(IOutputStream outputStream, Request request, byte[] file)
        {
            using (IOutputStream output = outputStream)
            {
                using (Stream responseStream = output.AsStreamForWrite())
                {
                    if (request.Query.FileType == FileType.HTML)
                        await sendWebsite(responseStream, file);
                    else if (request.Query.FileType != FileType.UNKNOWN)
                        await sendPicture(responseStream, request, file);
                }
            }
        }

        /// <summary>
        /// Sendet Website an Klienten
        /// </summary>
        /// <param name="responseStream"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        private async Task sendWebsite(Stream responseStream, byte[] file)
        {
            using (MemoryStream bodyStream = new MemoryStream(file))
            {
                string header = $"HTTP/1.1 200 OK\r\nContent-Length: {bodyStream.Length}\r\nConnection: close\r\nContent-MicrocontrollerType: text/html; charset=utf-8\r\n\r\n";
                byte[] headerArray = Encoding.UTF8.GetBytes(header);
                await responseStream.WriteAsync(headerArray, 0, headerArray.Length);
                await bodyStream.CopyToAsync(responseStream);
                await responseStream.FlushAsync();
            }
        }

        #endregion Private Methods

    }
}