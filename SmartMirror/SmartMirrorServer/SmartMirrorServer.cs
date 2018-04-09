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
using DataAccessLibrary;
using DataAccessLibrary.Module;
using NewsAPI;
using NewsAPI.Models;
using SmartMirrorServer.Enums.QueryEnums;
using SmartMirrorServer.Extensions;
using SmartMirrorServer.Features.Quote;
using SmartMirrorServer.Features.SunTimes;
using SmartMirrorServer.Features.Weather;
using SmartMirrorServer.Objects;

namespace SmartMirrorServer
{
    internal static class SmartMirrorServer
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
        public static async void Start()
        {
            try
            {
                #pragma warning disable 4014
                Task.Run(() =>
                {
                    updateModules();
                });
                #pragma warning restore 4014

                TimeSpan period = TimeSpan.FromMinutes(Application.DataUpdateMinutes);
                ThreadPoolTimer.CreatePeriodicTimer(source => { updateModules(); }, period);

                StreamSocketListener listener = new StreamSocketListener();
                listener.ConnectionReceived += listener_ConnectionReceived;

                await listener.BindServiceNameAsync("80");

                CoreApplication.Properties.Add("listener", listener);

                DataAccess.InitializeDatabase();

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
            // ReSharper disable once SwitchStatementMissingSomeCases
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
            IEnumerable<List<FiveDaysForecastResult>> result = getFiveDaysForecastByCityName(module);

            List<ForecastDays> forecastDays = result.Select(fiveDaysForecastResult => new ForecastDays { City = fiveDaysForecastResult[0].City, CityId = fiveDaysForecastResult[0].CityId, Date = fiveDaysForecastResult[0].Date, Temperature = Math.Round(fiveDaysForecastResult.Average(innerList => innerList.Temp), 1) , MinTemp = Math.Round(fiveDaysForecastResult.Min(innerList => innerList.TempMin), 1), MaxTemp = Math.Round(fiveDaysForecastResult.Min(innerList => innerList.TempMax), 1), Icon = fiveDaysForecastResult.GroupBy(x => x.Icon).OrderByDescending(x => x.Count()).First().Key }).ToList();

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

        private static Quote getQuoteOfDay()
        {
            return QuoteHelper.GetQuoteOfDay();
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

        private static async void updateModules()
        {
            Task upperLeftModuleTask = Task.Run(() =>
            {
                if (DataAccess.ModuleExists(Modules.UPPERLEFT))
                    return;

                buildModul(DataAccess.GetModule(Modules.UPPERLEFT));
            });

            Task upperRightModuleTask = Task.Run(() =>
            {
                if (DataAccess.ModuleExists(Modules.UPPERRIGHT))
                    return;

                buildModul(DataAccess.GetModule(Modules.UPPERRIGHT));
            });

            Task middleLeftModuleTask = Task.Run(() =>
            {
                if (DataAccess.ModuleExists(Modules.MIDDLELEFT))
                    return;

                buildModul(DataAccess.GetModule(Modules.MIDDLELEFT));
            });

            Task middleRightModuleTask = Task.Run(() =>
            {
                if (DataAccess.ModuleExists(Modules.MIDDLERIGHT))
                    return;

                buildModul(DataAccess.GetModule(Modules.MIDDLERIGHT));
            });

            Task lowerLeftModuleTask = Task.Run(() =>
            {
                if (DataAccess.ModuleExists(Modules.LOWERLEFT))
                    return;

                buildModul(DataAccess.GetModule(Modules.LOWERLEFT));
            });

            Task lowerrightModuleTask = Task.Run(() =>
            {
                if (DataAccess.ModuleExists(Modules.LOWERRIGHT))
                    return;

                buildModul(DataAccess.GetModule(Modules.LOWERRIGHT));
            });

            await Task.WhenAny(Task.WhenAll(upperLeftModuleTask, upperRightModuleTask, middleLeftModuleTask, middleRightModuleTask, lowerLeftModuleTask, lowerrightModuleTask), Task.Delay(TimeSpan.FromSeconds(10)));

            #pragma warning disable 4014
            Task.Run(() =>
            {
                if (DataAccess.ModuleExists(Modules.WEATHER))
                    return;

                weatherModul(DataAccess.GetModule(Modules.WEATHER));
            });

            Task.Run(() =>
            {
                if (DataAccess.ModuleExists(Modules.TIME))
                    return;

                timeModul(DataAccess.GetModule(Modules.TIME));
            });

            Task.Run(() =>
            {
                if (DataAccess.ModuleExists(Modules.WEATHERFORECAST))
                    return;

                Module weatherforecastModule = DataAccess.GetModule(Modules.WEATHERFORECAST);
                Application.Data.AddOrUpdate(weatherforecastModule, getFiveDaysForecastByCityName(weatherforecastModule), (key, value) => getFiveDaysForecastByCityName(weatherforecastModule));
            });

            Task.Run(() =>
            {
                if (DataAccess.ModuleExists(Modules.QUOTE))
                    return;

                quoteOfDayModul(DataAccess.GetModule(Modules.QUOTE));
            });

            Task.Run(() =>
            {
                if (DataAccess.ModuleExists(Modules.NEWSSCIENCE))
                    return;

                newsModul(DataAccess.GetModule(Modules.NEWSSCIENCE));
            });

            Task.Run(() =>
            {
                if (DataAccess.ModuleExists(Modules.NEWSENTERTAINMENT))
                    return;

                newsModul(DataAccess.GetModule(Modules.NEWSENTERTAINMENT));
            });

            Task.Run(() =>
            {
                if (DataAccess.ModuleExists(Modules.NEWSHEALTH))
                    return;

                newsModul(DataAccess.GetModule(Modules.NEWSHEALTH));
            });

            Task.Run(() =>
            {
                if (DataAccess.ModuleExists(Modules.NEWSSPORT))
                    return;

                newsModul(DataAccess.GetModule(Modules.NEWSSPORT));
            });

            Task.Run(() =>
            {
                if (DataAccess.ModuleExists(Modules.NEWSTECHNOLOGY))
                    return;

                newsModul(DataAccess.GetModule(Modules.NEWSTECHNOLOGY));
            });

            Task.Run(() =>
            {
                if (DataAccess.ModuleExists(Modules.NEWSBUSINESS))
                    return;

                newsModul(DataAccess.GetModule(Modules.NEWSBUSINESS));
            });
            #pragma warning restore 4014
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
        private static async Task<StringBuilder> handleHttpRequest(IInputStream inputStream)
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
        private static async Task handleHttpResponse(StreamSocketListenerConnectionReceivedEventArgs args, StringBuilder requestString)
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
        private static async void listener_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
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
        private static async Task sendPicture(Stream responseStream, Request request, byte[] file)
        {
            string fileType;

            // ReSharper disable once SwitchStatementMissingSomeCases
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
        private static async Task sendResponse(IOutputStream outputStream, Request request, byte[] file)
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
        private static async Task sendWebsite(Stream responseStream, byte[] file)
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