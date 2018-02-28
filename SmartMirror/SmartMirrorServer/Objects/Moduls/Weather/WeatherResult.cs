using System;

namespace SmartMirrorServer.Objects.Moduls.Weather
{
    internal abstract class WeatherResult
    {
        public DateTime Date { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public int CityId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public double Temp { get; set; }

        public double Humidity { get; set; }

        public double TempMax { get; set; }

        public double TempMin { get; set; }

        public double WindSpeed { get; set; }

        public string Icon { get; set; }
    }
}
