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

                case FileName.WEATHERFORECAST:
                    return await Site.BuildWeatherforecast();

                case FileName.LIGHT:
                    return await Site.BuildLight();

                case FileName.QUOTE:
                    return await Site.BuildQuote();

                case FileName.NEWS:
                    return await Site.BuildNews(request);

                case FileName.HELP:
                    return await Site.BuildHelp();

                default:
                    return new byte[0];
            }
        }
    }
}