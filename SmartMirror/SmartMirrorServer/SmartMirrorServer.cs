﻿using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
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

                await Api.ApiData.GetApiData();

                Debug.WriteLine("Api Daten abgerufen");

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