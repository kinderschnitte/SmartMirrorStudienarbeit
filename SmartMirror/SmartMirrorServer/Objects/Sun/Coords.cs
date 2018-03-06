namespace SmartMirrorServer.Objects.Sun
{
    internal abstract class Coords
    {
        protected int MDegrees;
        protected int MMinutes;
        protected int MSeconds;

        public double ToDouble()
        {
            return Sign() * (MDegrees + (double)MMinutes / 60 + (double)MSeconds / 3600);
        }

        protected abstract int Sign();
    }
}