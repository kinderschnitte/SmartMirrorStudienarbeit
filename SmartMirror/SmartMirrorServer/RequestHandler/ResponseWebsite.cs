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
                case FileName.Settings:

                    return await Site.BuildSettings(request);

                case FileName.Unknown:
                    return new byte[0];

                case FileName.Home:

                    return await Site.BuildHome();

                case FileName.Time:
                    return await Site.BuildTime();

                case FileName.Weather:
                    return await Site.BuildWeather();

                case FileName.Weatherforecast:
                    return await Site.BuildWeatherforecast();

                case FileName.Light:
                    return await Site.BuildLight();

                case FileName.Quote:
                    return await Site.BuildQuote();

                case FileName.News:
                    return await Site.BuildNews(request);

                case FileName.Help:
                    return await Site.BuildHelp();

                default:
                    return new byte[0];
            }
        }
    }
}