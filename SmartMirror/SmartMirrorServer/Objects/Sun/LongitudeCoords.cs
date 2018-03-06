namespace SmartMirrorServer.Objects.Sun
{
    internal class LongitudeCoords : Coords
    {
        public enum Direction
        {
            EAST,
            WEST
        }

        private readonly Direction mDirection;

        public LongitudeCoords(int degrees, int minutes, int seconds, Direction direction)
        {
            MDegrees = degrees;
            MMinutes = minutes;
            MSeconds = seconds;
            mDirection = direction;
        }

        protected override int Sign()
        {
            return mDirection == Direction.EAST ? 1 : -1;
        }
    }
}