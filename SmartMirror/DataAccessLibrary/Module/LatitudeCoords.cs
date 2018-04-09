using System;

namespace DataAccessLibrary.Module
{
    [Serializable]
    public class LatitudeCoords
    {
        public enum LatitudeDirection
        {
            NORTH,
            SOUTH
        }

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public int MDegrees { get; set; }

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public int MMinutes { get; set; }

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public int MSeconds { get; set; }

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public LatitudeDirection mLatitudeDirection { get; set; }

        private LatitudeCoords() { }

        public LatitudeCoords(int degrees, int minutes, int seconds, LatitudeDirection latitudeDirection)
        {
            MDegrees = degrees;
            MMinutes = minutes;
            MSeconds = seconds;
            mLatitudeDirection = latitudeDirection;
        }

        private int sign()
        {
            return mLatitudeDirection == LatitudeDirection.NORTH ? 1 : -1;
        }

        public double ToDouble()
        {
            return sign() * (MDegrees + (double)MMinutes / 60 + (double)MSeconds / 3600);
        }
    }
}