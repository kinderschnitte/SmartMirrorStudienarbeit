using System;
using SmartMirrorServer.HelperMethods;

namespace SmartMirrorServer.Objects.Moduls
{
    internal class Sun
    {
        public string Sunrise { get; }

        public string Sunset { get; }

        public Sun()
        {
            DateTime date = DateTime.Today;
            bool isSunrise = false;
            bool isSunset = false;
            DateTime sunrise = DateTime.Now;
            DateTime sunset = DateTime.Now;
            SunTimes.Instance.CalculateSunRiseSetTimes(new SunTimes.LatitudeCoords(32, 4, 0, SunTimes.LatitudeCoords.Direction.NORTH), new SunTimes.LongitudeCoords(34, 46, 0, SunTimes.LongitudeCoords.Direction.EAST), date, ref sunrise, ref sunset, ref isSunrise, ref isSunset);
            Sunrise = sunrise.ToString("HH:mm");
            Sunset = sunset.ToString("HH:mm");
        }
    }
}