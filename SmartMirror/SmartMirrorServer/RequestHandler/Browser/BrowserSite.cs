using System.Threading.Tasks;
using SmartMirrorServer.RequestHandler.Browser.Sites;

namespace SmartMirrorServer.RequestHandler.Browser
{
    internal static class BrowserSite
    {
        public static async Task<byte[]> BuildBrowserSettings()
        {
            return await BrowserSettings.BuildBrowserSettings();
        }
    }
}