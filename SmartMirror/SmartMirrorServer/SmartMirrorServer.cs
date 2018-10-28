using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Api;
using Api.GoogleMapsGeocoding;
using DataAccessLibrary;
using DataAccessLibrary.Module;
using DataAccessLibrary.Tables;
using NewsAPI.Constants;
using SmartMirrorServer.Enums.QueryEnums;
using SmartMirrorServer.Extensions;
using SmartMirrorServer.Objects;

namespace SmartMirrorServer
{
    internal class SmartMirrorServer
    {

        #region Private Fields

        /// <summary>
        /// Größe des Empfangsbuffer des Webservers
        /// </summary>
        private const uint BufferSize = 8192;

        private StreamSocketListener listener;

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// Startet den Server Smart Home Webserver
        /// </summary>
        public async void Start()
        {
            try
            {
                listener = new StreamSocketListener();
                listener.ConnectionReceived += listener_ConnectionReceived;

                await listener.BindServiceNameAsync("80");

                CoreApplication.Properties.Add("listener", listener);

                Debug.WriteLine("Server gestartet");

                if (Application.Notifications.SystemStartNotifications)
                    Notification.Notification.SendPushNotification("System wurde gestartet.", "Das Smart Mirror System wurde erfolgreich gestartet.");

                Log.Log.WriteLog("Das Smart Mirror System wurde erfolgreich gestartet.");
            }
            catch (Exception exception)
            {
                if (Application.Notifications.ExceptionNotifications)
                    Notification.Notification.SendPushNotification("Fehler aufgetreten.", $"{exception.StackTrace}");

                Log.Log.WriteException(exception);
            }
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Nimmt den Request entgegen und gibt diesen als StringBuilder Objekt zurück
        /// </summary>
        /// <param name="inputStream"></param>
        /// <returns></returns>
        private static async Task<StringBuilder> HandleHttpRequest(IInputStream inputStream)
        {
            try
            {
                StringBuilder requestString = new StringBuilder();

                using (IInputStream input = inputStream)
                {
                    byte[] data = new byte[BufferSize];
                    IBuffer buffer = data.AsBuffer();
                    uint dataRead = BufferSize;

                    while (dataRead == BufferSize)
                    {
                        await input.ReadAsync(buffer, BufferSize, InputStreamOptions.Partial);
                        requestString.Append(Encoding.UTF8.GetString(data, 0, data.Length));
                        dataRead = buffer.Length;
                    }
                }

                return requestString;
            }
            catch (Exception exception)
            {
                Log.Log.WriteException(exception);
                return new StringBuilder();
            }
        }

        /// <summary>
        /// Verarbeitet Anfrage und sendet passende Antwort zurück
        /// </summary>
        /// <param name="args"></param>
        /// <param name="requestString"></param>
        /// <returns></returns>
        private static async Task HandleHttpResponse(StreamSocketListenerConnectionReceivedEventArgs args, StringBuilder requestString)
        {
            try
            {
                Debug.WriteLine(requestString.ToString());

                // Bestimmt die Anfrage
                Request request = requestString.GetRequest(args);

                #pragma warning disable 4014
                ProcessPostRequest(request.PostQuery);
                #pragma warning restore 4014

                // Erstellt die Response
                byte[] responseBytes = await RequestHandler.RequestHandler.BuildResponse(request);

                // Sendet Antwort an den Klienten
                await SendResponse(args.Socket.OutputStream, request, responseBytes);
            }
            catch (Exception exception)
            {
                Log.Log.WriteException(exception);
            }
        }

        private static async Task ProcessPostRequest(PostQuery requestPostQuery)
        {
            try
            {
                if (requestPostQuery.Value.Count == 0)
                    return;

                bool breakLoop = false;

                foreach (KeyValuePair<string, string> keyValuePair in requestPostQuery.Value)
                {
                    if (breakLoop)
                        break;

                    switch (keyValuePair.Key)
                    {
                        case "upperleft":
                            await SetModule(Modules.Upperleft, keyValuePair.Value);
                            break;

                        case "upperright":
                            await SetModule(Modules.Upperright, keyValuePair.Value);
                            break;

                        case "middleleft":
                            await SetModule(Modules.Middleleft, keyValuePair.Value);
                            break;

                        case "middleright":
                            await SetModule(Modules.Middleright, keyValuePair.Value);
                            break;

                        case "lowerleft":
                            await SetModule(Modules.Lowerleft, keyValuePair.Value);
                            break;

                        case "lowerright":
                            await SetModule(Modules.Lowerright, keyValuePair.Value);
                            break;

                        case "City":
                        case "Postal":
                        case "State":
                        case "Country":
                        case "Language":
                            await SetLocation(requestPostQuery.Value);
                            breakLoop = true;
                            break;
                    }
                }

                #pragma warning disable 4014
                ApiData.GetApiData();
                #pragma warning restore 4014
            }
            catch (Exception exception)
            {
                Log.Log.WriteException(exception);
            }
        }

        #pragma warning disable 1998
        private static async Task SetLocation(IReadOnlyDictionary<string, string> value)
        #pragma warning restore 1998
        {
            try
            {
                DataAccess.AddOrReplaceLocationData(value["City"], value["Postal"], value["Citycode"], value["Country"], value["Language"], value["State"]);
            }
            catch (Exception exception)
            {
                Log.Log.WriteException(exception);
            }
        }

        private static async Task SetModule(Modules modules, string value)
        {
            Module module = await GetModule(value);

            DataAccess.AddOrReplaceModule(modules, module);
        }

        #pragma warning disable 1998
        private static async Task<Module> GetModule(string value)
        #pragma warning restore 1998
        {
            try
            {
                ModuleType moduleType = ModuleType.None;
                string city = string.Empty;
                string postal = string.Empty;
                string cityCode = string.Empty;
                string country = string.Empty;
                string language = string.Empty;
                LatitudeCoords latitudeCoords = null;
                LongitudeCoords longitudeCoords = null;
                Categories categories = Categories.Business;
                Countries countries = Countries.AE;
                Languages languages = Languages.AF;

                if (value != string.Empty)
                {
                    if (!value.Contains("news"))
                    {
                        LocationTable locationTable = DataAccess.GetLocationData()?[0];

                        moduleType = (ModuleType)Enum.Parse(typeof(ModuleType), Regex.Replace(value, @"[\d-]", string.Empty).ToUpper());

                        if (locationTable != null)
                        {
                            switch (moduleType)
                            {
                                case ModuleType.None:
                                    break;

                                case ModuleType.Time:

                                    Coordinates coordinates = await GoogleMapsGeocoding.GetCoordinatesForPostal(locationTable.Postal, locationTable.Country);

                                    if (coordinates == null)
                                        break;

                                    latitudeCoords = coordinates.Latitude;
                                    longitudeCoords = coordinates.Longitude;
                                    break;

                                case ModuleType.Weather:

                                    city = locationTable.City;
                                    postal = locationTable.Postal;
                                    cityCode = locationTable.CityCode;
                                    country = locationTable.Country;
                                    language = locationTable.Language;
                                    break;

                                case ModuleType.Weatherforecast:

                                    city = locationTable.City;
                                    postal = locationTable.Postal;
                                    cityCode = locationTable.CityCode;
                                    country = locationTable.Country;
                                    language = locationTable.Language;
                                    break;

                                case ModuleType.News:
                                    break;

                                case ModuleType.Quote:
                                    break;

                                case ModuleType.Joke:
                                    break;
                            }
                        }
                    }
                    else
                    {
                        moduleType = ModuleType.News;

                        value = value.Remove(0, 4);

                        // To UppperCamelCase
                        value = value.First().ToString().ToUpper() + value.Substring(1);

                        categories = (Categories) Enum.Parse(typeof(Categories), Regex.Replace(value, @"[\d-]", string.Empty));

                        LocationTable locationTable = DataAccess.GetLocationData()?[0];

                        // ReSharper disable once InvertIf
                        if (locationTable != null)
                        {
                            countries = (Countries) Enum.Parse(typeof(Countries), locationTable.Country.ToUpper());

                            languages = (Languages) Enum.Parse(typeof(Languages), locationTable.Language.ToUpper());
                        }
                    }
                }

                return new Module
                {
                    ModuleType = moduleType,
                    City = city,
                    Postal = postal,
                    CityCode = cityCode,
                    Country =  country,
                    Language = language,
                    LatitudeCoords = latitudeCoords,
                    LongitudeCoords = longitudeCoords,
                    NewsCategory = categories,
                    NewsCountry = countries,
                    NewsLanguage = languages
                };
            }
            catch (Exception exception)
            {
                Log.Log.WriteException(exception);
                return new Module();
            }
        }

        /// <summary>
        /// Verarbeitet Webserver Anfragen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static async void listener_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            try
            {
                // Verarbeitet die HTTP Anfrage
                StringBuilder requestString = await HandleHttpRequest(args.Socket.InputStream);

                // Erstellt die Antwort und sendet diese
                await HandleHttpResponse(args, requestString);
            }
            catch (Exception exception)
            {
                Log.Log.WriteException(exception);
            }
        }

        /// <summary>
        /// Sendet Bild an den Klienten
        /// </summary>
        /// <param name="responseStream"></param>
        /// <param name="request"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        private static async Task SendPicture(Stream responseStream, Request request, byte[] file)
        {
            string fileType;

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (request.Query.FileType)
            {
                case FileType.Jpeg:
                    fileType = "jpeg";
                    break;

                case FileType.Png:
                    fileType = "png";
                    break;

                case FileType.Icon:
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
            catch (Exception exception)
            {
                Log.Log.WriteException(exception);
            }
        }

        /// <summary>
        /// Sendet passende Antwort zum Klienten
        /// </summary>
        /// <param name="outputStream"></param>
        /// <param name="request"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        private static async Task SendResponse(IOutputStream outputStream, Request request, byte[] file)
        {
            try
            {
                using (IOutputStream output = outputStream)
                {
                    using (Stream responseStream = output.AsStreamForWrite())
                    {
                        if (request.Query.FileType == FileType.Html)
                            await SendWebsite(responseStream, file);
                        else if (request.Query.FileType != FileType.Unknown)
                            await SendPicture(responseStream, request, file);
                    }
                }
            }
            catch (Exception exception)
            {
                Log.Log.WriteException(exception);
            }
        }

        /// <summary>
        /// Sendet Website an Klienten
        /// </summary>
        /// <param name="responseStream"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        private static async Task SendWebsite(Stream responseStream, byte[] file)
        {
            try
            {
                using (MemoryStream bodyStream = new MemoryStream(file))
                {
                    string header = $"HTTP/1.1 200 OK\r\nContent-Length: {bodyStream.Length}\r\nConnection: close\r\nContent-Type: text/html; charset=utf-8\r\n\r\n";
                    byte[] headerArray = Encoding.UTF8.GetBytes(header);
                    await responseStream.WriteAsync(headerArray, 0, headerArray.Length);
                    await bodyStream.CopyToAsync(responseStream);
                    await responseStream.FlushAsync();
                }
            }
            catch (Exception exception)
            {
                Log.Log.WriteException(exception);
            }
        }

        #endregion Private Methods

    }
}