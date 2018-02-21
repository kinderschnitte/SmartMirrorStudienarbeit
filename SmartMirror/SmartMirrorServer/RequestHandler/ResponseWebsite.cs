using System.Threading.Tasks;
using SmartMirrorServer.Enums.QueryEnums;
using SmartMirrorServer.Objects;
using SmartMirrorServer.RequestHandler.Mirror;

namespace SmartMirrorServer.RequestHandler
{
    internal static class ResponseWebsite
    {
        public static async Task<byte[]> BuildResponseWebsite(Request request)
        {
            bool isMirror = request.Query.FilePath.Contains("MagicMirror");

            switch (request.Query.FileName)
            {
                case FileName.HOME:

                    if (isMirror)
                        return await MirrorSite.BuildMirrorHome();

                    return await Browser.BrowserSite.BuildBrowserHome();

                case FileName.TIME:

                    if (isMirror)
                        return await MirrorSite.BuildMirrorTime();

                    return new byte[0];

                case FileName.UNKNOWN:
                    return new byte[0];

                default:
                    return new byte[0];
            }
        }
    }
}