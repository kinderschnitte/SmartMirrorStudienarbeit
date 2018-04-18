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
using SmartMirrorServer.Enums.Api;
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
                await updateModules();

                TimeSpan period = TimeSpan.FromMinutes(Application.DataUpdateMinutes);
                ThreadPoolTimer.CreatePeriodicTimer(async source => { await updateModules(); }, period);

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

        private static async Task buildModul(Modules modules, Module module)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (module.ModuleType)
            {
                case ModuleType.TIME:
                    timeModul(modules, module);
                    break;

                case ModuleType.WEATHER:
                    await weatherModul(modules, module);
                    break;

                case ModuleType.WEATHERFORECAST:
                    await weatherforecastModul(modules, module);
                    break;

                case ModuleType.NEWS:
                    await newsModul(modules, module);
                    break;

                case ModuleType.QUOTEOFDAY:
                    await quoteOfDayModul(modules);
                    break;
            }
        }

        private static async Task<List<ForecastDays>> getcalculatedForecast(Module module)
        {
            IEnumerable<List<FiveDaysForecastResult>> result = await getFiveDaysForecastByCityName(module);

            List<ForecastDays> forecastDays = result.Select(fiveDaysForecastResult => new ForecastDays { City = fiveDaysForecastResult[0].City, CityId = fiveDaysForecastResult[0].CityId, Date = fiveDaysForecastResult[0].Date, Temperature = Math.Round(fiveDaysForecastResult.Average(innerList => innerList.Temp), 1) , MinTemp = Math.Round(fiveDaysForecastResult.Min(innerList => innerList.TempMin), 1), MaxTemp = Math.Round(fiveDaysForecastResult.Min(innerList => innerList.TempMax), 1), Icon = fiveDaysForecastResult.GroupBy(x => x.Icon).OrderByDescending(x => x.Count()).First().Key }).ToList();

            // Infos zu heutigen Tag löschen
            if (forecastDays.Count > 5)
                forecastDays.RemoveAt(0);

            return forecastDays;
        }

        private static async Task<SingleResult<CurrentWeatherResult>> getCurrentWeatherByCityName(Module module)
        {
            return await CurrentWeather.GetByCityName(module.City, module.Country, module.Language, "metric");
        }

        private static async Task<List<List<FiveDaysForecastResult>>> getFiveDaysForecastByCityName(Module module)
        {
            return await FiveDaysForecast.GetByCityName(module.City, module.Country, module.Language, "metric");
        }

        private static async Task<ArticlesResult> getNewsByCategory(Module module)
        {
            NewsApiClient newsApiClient = new NewsApiClient(Application.ApiKeys[Api.NEWSAPI]);

            ArticlesResult topheadlines = await newsApiClient.GetTopHeadlinesAsync(new TopHeadlinesRequest
            {
                Category = module.NewsCategory,
                Country = module.NewsCountry,
                Language = module.NewsLanguage
            });

            return topheadlines;
        }

        private static async Task<ArticlesResult> getNewsBySource(Module module)
        {
            NewsApiClient newsApiClient = new NewsApiClient(Application.ApiKeys[Api.NEWSAPI]);

            ArticlesResult topheadlines = await newsApiClient.GetTopHeadlinesAsync(new TopHeadlinesRequest
            {
                Language = module.NewsLanguage,
                Sources = module.NewsSources
            });

            return topheadlines;
        }

        private static async Task<Quote> getQuoteOfDay()
        {
            return await QuoteHelper.GetQuoteOfDay();
        }

        private static async Task newsModul(Modules modules, Module module)
        {
            ArticlesResult result = module.NewsSources.Count == 0 ? await getNewsByCategory(module) : await getNewsBySource(module);
            ModuleData.Data.AddOrUpdate(modules, result, (key, value) => result);
        }

        private static async Task quoteOfDayModul(Modules modules)
        {
            Quote result = await getQuoteOfDay();
            ModuleData.Data.AddOrUpdate(modules, result, (key, value) => result);
        }

        private static void timeModul(Modules modules, Module module)
        {
            ModuleData.Data.AddOrUpdate(modules, new Sun(module), (key, value) => new Sun(module));
        }

        private static async Task updateModules()
        {
            if (DataAccess.ModuleExists(Modules.UPPERLEFT))
                await buildModul(Modules.UPPERLEFT, DataAccess.GetModule(Modules.UPPERLEFT));

            if (DataAccess.ModuleExists(Modules.UPPERRIGHT))
                await buildModul(Modules.UPPERRIGHT, DataAccess.GetModule(Modules.UPPERRIGHT));

            if (DataAccess.ModuleExists(Modules.MIDDLELEFT))
                await buildModul(Modules.MIDDLELEFT, DataAccess.GetModule(Modules.MIDDLELEFT));

            if (DataAccess.ModuleExists(Modules.MIDDLERIGHT))
                await buildModul(Modules.MIDDLERIGHT, DataAccess.GetModule(Modules.MIDDLERIGHT));

            if (DataAccess.ModuleExists(Modules.LOWERLEFT))
                await buildModul(Modules.LOWERLEFT, DataAccess.GetModule(Modules.LOWERLEFT));

            if (DataAccess.ModuleExists(Modules.LOWERRIGHT))
                await buildModul(Modules.LOWERRIGHT, DataAccess.GetModule(Modules.LOWERRIGHT));


            if (DataAccess.ModuleExists(Modules.WEATHER))
                await weatherModul(Modules.WEATHER, DataAccess.GetModule(Modules.WEATHER));

            if (DataAccess.ModuleExists(Modules.TIME))
                timeModul(Modules.TIME, DataAccess.GetModule(Modules.TIME));

            if (DataAccess.ModuleExists(Modules.WEATHERFORECAST))
            {
                Module weatherforecastModule = DataAccess.GetModule(Modules.WEATHERFORECAST);
                List<List<FiveDaysForecastResult>> result = await getFiveDaysForecastByCityName(weatherforecastModule);
                ModuleData.Data.AddOrUpdate(Modules.WEATHERFORECAST, result, (key, value) => result);
            }

            if (DataAccess.ModuleExists(Modules.QUOTE))
                await quoteOfDayModul(Modules.QUOTE);

            if (DataAccess.ModuleExists(Modules.NEWSSCIENCE))
                await newsModul(Modules.NEWSSCIENCE, DataAccess.GetModule(Modules.NEWSSCIENCE));

            if (DataAccess.ModuleExists(Modules.NEWSENTERTAINMENT))
                await newsModul(Modules.NEWSENTERTAINMENT, DataAccess.GetModule(Modules.NEWSENTERTAINMENT));

            if (DataAccess.ModuleExists(Modules.NEWSHEALTH))
                await newsModul(Modules.NEWSHEALTH, DataAccess.GetModule(Modules.NEWSHEALTH));

            if (DataAccess.ModuleExists(Modules.NEWSSPORT))
                await newsModul(Modules.NEWSSPORT, DataAccess.GetModule(Modules.NEWSSPORT));

            if (DataAccess.ModuleExists(Modules.NEWSTECHNOLOGY))
                await newsModul(Modules.NEWSTECHNOLOGY, DataAccess.GetModule(Modules.NEWSTECHNOLOGY));

            if (DataAccess.ModuleExists(Modules.NEWSBUSINESS))
                await newsModul(Modules.NEWSBUSINESS, DataAccess.GetModule(Modules.NEWSBUSINESS));
        }

        private static async Task weatherforecastModul(Modules modules, Module module)
        {
            List<ForecastDays> result = await getcalculatedForecast(module);
            ModuleData.Data.AddOrUpdate(modules, result, (key, value) => result);
        }

        private static async Task weatherModul(Modules modules, Module module)
        {
            SingleResult<CurrentWeatherResult> result = await getCurrentWeatherByCityName(module);
            ModuleData.Data.AddOrUpdate(modules, result, (key, value) => result);
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

            try
            {
                using (MemoryStream bodyStream = new MemoryStream(file))
                {
                    string header = $"HTTP/1.1 200 OK\r\nContent-Length: {bodyStream.Length}\r\nConnection: close\r\nContent-Type: image/{fileType}; charset=utf-8\r\n\r\n";
                    byte[] headerArray = Encoding.UTF8.GetBytes(header);
                    await responseStream.WriteAsync(headerArray, 0, headerArray.Length);
                    await bodyStream.CopyToAsync(responseStream);
                    await responseStream.FlushAsync();
                }
            }
            catch (Exception)
            {
                // ignored
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
            try
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
            catch (Exception)
            {
                // ignored
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
            try
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
            catch (Exception)
            {
                // ignored
            }
        }

        #endregion Private Methods

    }
}