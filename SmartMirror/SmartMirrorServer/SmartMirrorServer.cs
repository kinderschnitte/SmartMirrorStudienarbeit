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
        private const uint bufferSize = 8192;

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

                #pragma warning disable 4014
                Api.ApiData.GetApiData();
                #pragma warning restore 4014

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

            #pragma warning disable 4014
            processPostRequest(request.PostQuery);
            #pragma warning restore 4014

            // Erstellt die Response
            byte[] responseBytes = await RequestHandler.RequestHandler.BuildResponse(request);

            // Sendet Antwort an den Klienten
            await sendResponse(args.Socket.OutputStream, request, responseBytes);
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private static async Task processPostRequest(PostQuery requestPostQuery)
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
                        await setModule(Modules.UPPERLEFT, keyValuePair.Value);
                        break;

                    case "upperright":
                        await setModule(Modules.UPPERRIGHT, keyValuePair.Value);
                        break;

                    case "middleleft":
                        await setModule(Modules.MIDDLELEFT, keyValuePair.Value);
                        break;

                    case "middleright":
                        await setModule(Modules.MIDDLERIGHT, keyValuePair.Value);
                        break;

                    case "lowerleft":
                        await setModule(Modules.LOWERLEFT, keyValuePair.Value);
                        break;

                    case "lowerright":
                        await setModule(Modules.LOWERRIGHT, keyValuePair.Value);
                        break;

                    case "City":
                    case "State":
                    case "Country":
                    case "Language":
                        await setLocation(requestPostQuery.Value);
                        breakLoop = true;
                        break;
                }
            }

            #pragma warning disable 4014
            Api.ApiData.GetApiData();
            #pragma warning restore 4014
        }

        #pragma warning disable 1998
        private static async Task setLocation(IReadOnlyDictionary<string, string> value)
        #pragma warning restore 1998
        {
            try
            {
                DataAccess.AddOrReplaceLocationData(value["City"], value["Country"], value["Language"], value["State"]);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private static async Task setModule(Modules modules, string value)
        {
            Module module = await getModule(value);

            DataAccess.AddOrReplaceModule(modules, module);
        }

        #pragma warning disable 1998
        private static async Task<Module> getModule(string value)
        #pragma warning restore 1998
        {
            ModuleType moduleType = ModuleType.NONE;
            string city = string.Empty;
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

                    moduleType = (ModuleType)Enum.Parse(typeof(ModuleType), value.ToUpper());

                    if (locationTable != null)
                    {
                        switch (moduleType)
                        {
                            case ModuleType.NONE:
                                break;

                            case ModuleType.TIME:

                                Coordinates coordinates = await GoogleMapsGeocoding.GetCoordinatesForCity(locationTable.City, locationTable.State, locationTable.Country);
                                latitudeCoords = coordinates.Latitude;
                                longitudeCoords = coordinates.Longitude;
                                break;

                            case ModuleType.WEATHER:

                                city = locationTable.City;
                                country = locationTable.Country;
                                language = locationTable.Language;
                                break;

                            case ModuleType.WEATHERFORECAST:

                                city = locationTable.City;
                                country = locationTable.Country;
                                language = locationTable.Language;
                                break;

                            case ModuleType.NEWS:
                                break;

                            case ModuleType.QUOTE:
                                break;

                            case ModuleType.JOKE:
                                break;
                        }
                    }
                }
                else
                {
                    moduleType = ModuleType.NEWS;

                    value = value.Remove(0, 4);

                    // To UppperCamelCase
                    value = value.First().ToString().ToUpper() + value.Substring(1);

                    categories = (Categories) Enum.Parse(typeof(Categories), value);

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
                Country =  country,
                Language = language,
                LatitudeCoords = latitudeCoords,
                LongitudeCoords = longitudeCoords,
                NewsCategory = categories,
                NewsCountry = countries,
                NewsLanguage = languages
            };
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
                StringBuilder requestString = await handleHttpRequest(args.Socket.InputStream);

                // Erstellt die Antwort und sendet diese
                await handleHttpResponse(args, requestString);
            }
            catch (Exception)
            {
                // ignored
            }
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