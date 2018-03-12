using System.Threading.Tasks;
using SmartMirrorServer.Enums.QueryEnums;
using SmartMirrorServer.Objects;

namespace SmartMirrorServer.RequestHandler
{
    internal static class ResponseWebsite
    {
        public static async Task<byte[]> BuildResponseWebsite(Request request)
        {
            switch (request.Query.FileName)
            {
                case FileName.SETTINGS:

                    return await Site.BuildSettings();

                case FileName.UNKNOWN:
                    return new byte[0];

                case FileName.HOME:

                    return await Site.BuildHome();

                case FileName.TIME:
                    return await Site.BuildTime();

                case FileName.WEATHER:
                    return await Site.BuildWeather();

                default:
                    return new byte[0];
            }
        }
    }
}