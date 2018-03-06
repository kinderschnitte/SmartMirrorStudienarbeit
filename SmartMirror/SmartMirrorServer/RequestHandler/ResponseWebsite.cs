using System.Threading.Tasks;
using SmartMirrorServer.Enums.QueryEnums;
using SmartMirrorServer.Objects;
using SmartMirrorServer.RequestHandler.Browser;
using SmartMirrorServer.RequestHandler.Mirror;

namespace SmartMirrorServer.RequestHandler
{
    internal static class ResponseWebsite
    {
        public static async Task<byte[]> BuildResponseWebsite(Request request)
        {
            bool isMirror = request.Query.FilePath.Contains("SmartMirror");

            switch (request.Query.FileName)
            {
                case FileName.SETTINGS:

                    if (!isMirror)
                        return await BrowserSite.BuildBrowserSettings();
                    else
                        return new byte[0];

                case FileName.UNKNOWN:
                    return new byte[0];

                case FileName.HOME:

                    if (isMirror)
                        return await MirrorSite.BuildMirrorHome();
                    else
                        return new byte[0];

                case FileName.TIME:
                    return new byte[0];

                default:
                    return new byte[0];
            }
        }
    }
}