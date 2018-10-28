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
            if (request.IsInvalidRequest || request.Query.FileType == FileType.Unknown)
                return new byte[0];

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (request.Query.FileType)
            {
                case FileType.Html:
                    return await ResponseWebsite.BuildResponseWebsite(request);

                case FileType.Icon:
                case FileType.Jpeg:
                case FileType.Png:
                    return ResponseImage.LoadImage(request);

                case FileType.Css:
                case FileType.Js:
                    return ResponseStylesheet.LoadStylesheet(request);

                case FileType.Unknown:
                    break;
            }

            return new byte[0];
        }

        #endregion Public Methods
    }
}