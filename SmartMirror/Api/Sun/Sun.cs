using System;
using DataAccessLibrary.Module;

namespace Api.Sun
{
    [Serializable]
    public class Sun
    {
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public string Sunrise { get; set; }

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public string Sunset { get; set; }

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

        // ReSharper disable once MemberCanBePrivate.Global
        public Sun() { }
    }
}