using System;

namespace DataAccessLibrary.Module
{
    [Serializable]
    public class LongitudeCoords
    {
        public enum LongitudeDirection
        {
            East,
            West
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
        public LongitudeDirection MLongitudeDirection { get; set; }

        private LongitudeCoords() { }

        public LongitudeCoords(int degrees, int minutes, int seconds, LongitudeDirection longitudeDirection)
        {
            MDegrees = degrees;
            MMinutes = minutes;
            MSeconds = seconds;
            MLongitudeDirection = longitudeDirection;
        }

        private int Sign()
        {
            return MLongitudeDirection == LongitudeDirection.East ? 1 : -1;
        }

        public double ToDouble()
        {
            return Sign() * (MDegrees + (double)MMinutes / 60 + (double)MSeconds / 3600);
        }
    }
}