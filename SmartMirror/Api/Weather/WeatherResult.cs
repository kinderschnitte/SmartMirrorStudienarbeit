using System;

namespace Api.Weather
{
    public abstract class WeatherResult
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

        public double WindDegree { get; set; }

        public int Cloudinesss { get; set; }

        public double Pressure { get; set; }

        public double SeaLevelPressure { get; set; }

        public double GroundLevelPressure { get; set; }

        public double Rain { get; set; }

        public double Snow { get; set; }

        public DateTime Sunrise { get; set; }

        public DateTime Sunset { get; set; }

        public string Icon { get; set; }
    }
}
