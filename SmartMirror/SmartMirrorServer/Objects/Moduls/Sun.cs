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
            SunTimes.Instance.CalculateSunRiseSetTimes(Application.StorageData.LatitudeCoords, Application.StorageData.LongitudeCoords, date, ref sunrise, ref sunset, ref isSunrise, ref isSunset);
            Sunrise = sunrise.ToString("HH:mm");
            Sunset = sunset.ToString("HH:mm");
        }
    }
}