using System.Threading.Tasks;
using SmartMirrorServer.RequestHandler.Mirror.Sites;

namespace SmartMirrorServer.RequestHandler.Mirror
{
    internal static class MirrorSite
    {
        public static async Task<byte[]> BuildMirrorHome()
        {
            return await MirrorHome.BuildMirrorHome();
        }
    }
}