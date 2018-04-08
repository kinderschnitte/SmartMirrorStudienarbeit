namespace SmartMirrorServer.Features.Weather
{
    /// <summary>
    ///     FiveDaysForecastResult result type.
    /// </summary>
    internal class FiveDaysForecastResult : WeatherResult
    {
        /// <summary>
        ///     Time of data receiving in unixtime GMT.
        /// </summary>
        public int DateUnixFormat { get; set; }
    }
}