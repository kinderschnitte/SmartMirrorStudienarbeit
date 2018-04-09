using System;
using DataAccessLibrary.Module;

namespace SmartMirrorServer.Features.SunTimes
{
    internal class Sun
    {
        public string Sunrise { get; }

        public string Sunset { get; }

        public Sun(Module module)
        {
            DateTime date = DateTime.Today;
            bool isSunrise = false;
            bool isSunset = false;
            DateTime sunrise = DateTime.Now;
            DateTime sunset = DateTime.Now;
            SunTimes.Instance.CalculateSunRiseSetTimes(module.LatitudeCoords, module.LongitudeCoords, date, ref sunrise, ref sunset, ref isSunrise, ref isSunset);
            Sunrise = sunrise.ToString("HH:mm");
            Sunset = sunset.ToString("HH:mm");
        }
    }
}