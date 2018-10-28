using System;

namespace DataAccessLibrary.Module
{
    [Serializable]
    public class LatitudeCoords
    {
        public enum LatitudeDirection
        {
            North,
            South
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
        public LatitudeDirection MLatitudeDirection { get; set; }

        private LatitudeCoords() { }

        public LatitudeCoords(int degrees, int minutes, int seconds, LatitudeDirection latitudeDirection)
        {
            MDegrees = degrees;
            MMinutes = minutes;
            MSeconds = seconds;
            MLatitudeDirection = latitudeDirection;
        }

        private int Sign()
        {
            return MLatitudeDirection == LatitudeDirection.North ? 1 : -1;
        }

        public double ToDouble()
        {
            return Sign() * (MDegrees + (double)MMinutes / 60 + (double)MSeconds / 3600);
        }
    }
}