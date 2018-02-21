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
            return await buildResponse(request);
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Baut die Antwort für den Browser auf
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static async Task<byte[]> buildResponse(Request request)
        {
            if (request.IsInvalidRequest)
                return new byte[0];

            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (request.Query.FileType != FileType.HTML)
                return ResponseImage.LoadImage(request);

            return await ResponseWebsite.BuildResponseWebsite(request);
        }

        #endregion Private Methods
    }
}