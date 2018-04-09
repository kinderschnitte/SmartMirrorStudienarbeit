using System;

namespace DataAccessLibrary.Module
{
    [Serializable]
    public class LongitudeCoords
    {
        public enum LongitudeDirection
        {
            EAST,
            WEST
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
        public LongitudeDirection mLongitudeDirection { get; set; }

        private LongitudeCoords() { }

        public LongitudeCoords(int degrees, int minutes, int seconds, LongitudeDirection longitudeDirection)
        {
            MDegrees = degrees;
            MMinutes = minutes;
            MSeconds = seconds;
            mLongitudeDirection = longitudeDirection;
        }

        private int sign()
        {
            return mLongitudeDirection == LongitudeDirection.EAST ? 1 : -1;
        }

        public double ToDouble()
        {
            return sign() * (MDegrees + (double)MMinutes / 60 + (double)MSeconds / 3600);
        }
    }
}