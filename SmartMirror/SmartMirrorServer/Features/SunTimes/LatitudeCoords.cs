namespace SmartMirrorServer.Features.SunTimes
{
    internal class LatitudeCoords : Coords
    {
        public enum Direction
        {
            NORTH,
            SOUTH
        }

        private readonly Direction mDirection;

        public LatitudeCoords(int degrees, int minutes, int seconds, Direction direction)
        {
            MDegrees = degrees;
            MMinutes = minutes;
            MSeconds = seconds;
            mDirection = direction;
        }

        protected override int Sign()
        {
            return mDirection == Direction.NORTH ? 1 : -1;
        }
    }
}