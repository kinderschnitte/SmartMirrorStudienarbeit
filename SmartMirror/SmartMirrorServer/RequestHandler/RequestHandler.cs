using System.Threading.Tasks;
using SmartMirrorServer.Enums.QueryEnums;
using SmartMirrorServer.Objects;

namespace SmartMirrorServer.RequestHandler
{
    internal static class RequestHandler
    {

        #region Public Methods

        /// <summary>
        /// Baut die Antwort für den Browser auf
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<byte[]> BuildResponse(Request request)
        {
            if (request.IsInvalidRequest || request.Query.FileType == FileType.UNKNOWN)
                return new byte[0];

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (request.Query.FileType)
            {
                case FileType.HTML:
                    return await ResponseWebsite.BuildResponseWebsite(request);

                case FileType.ICON:
                case FileType.JPEG:
                case FileType.PNG:
                    return ResponseImage.LoadImage(request);

                case FileType.CSS:
                case FileType.JS:
                    return ResponseStylesheet.LoadStylesheet(request);

                case FileType.UNKNOWN:
                    break;
            }

            return new byte[0];
        }

        #endregion Public Methods
    }
}