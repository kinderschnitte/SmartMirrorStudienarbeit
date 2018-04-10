using System.Threading.Tasks;
using SmartMirrorServer.Objects;
using SmartMirrorServer.RequestHandler.Sites;

namespace SmartMirrorServer.RequestHandler
{
    internal static class Site
    {
        public static async Task<byte[]> BuildSettings()
        {
            return await Settings.BuildSettings();
        }

        public static async Task<byte[]> BuildHome()
        {
            return await Home.BuildHome();
        }

        public static async Task<byte[]> BuildTime()
        {
            return await Time.BuildTime();
        }

        public static async Task<byte[]> BuildWeather()
        {
            return await Weather.BuildWeather();
        }

        public static async Task<byte[]> BuildWeatherforecast()
        {
            return await Weatherforecast.BuildWeatherforecast();
        }

        public static async Task<byte[]> BuildLight()
        {
            return await Light.BuildLight();
        }

        public static async Task<byte[]> BuildQuote()
        {
            return await Quote.BuildQuote();
        }

        public static async Task<byte[]> BuildNews(Request request)
        {
            return await News.BuildNews(request);
        }

        public static async Task<byte[]> BuildHelp()
        {
            return await Help.BuildHelp();
        }
    }
}