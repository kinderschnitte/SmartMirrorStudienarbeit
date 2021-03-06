﻿namespace Api.Weather
{
    /// <summary>
    ///     FiveDaysForecastResult result type.
    /// </summary>
    public class FiveDaysForecastResult : WeatherResult
    {
        /// <summary>
        ///     Time of data receiving in unixtime GMT.
        /// </summary>
        public int DateUnixFormat { get; set; }
    }
}