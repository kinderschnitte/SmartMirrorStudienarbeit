namespace SmartMirrorServer.HelperMethods
{
    public static class WeatherHelperClass
    {
        public static string ChooseWeatherIcon(string itemIcon)
        {
            switch (itemIcon)
            {
                case "01d":
                case "01n":
                    return "sunny.png";

                case "02d":
                case "02n":
                    return "sunny-cloudy.png";

                case "03d":
                case "03n":
                case "04d":
                case "04n":
                    return "cloudy.png";

                case "09d":
                case "09n":
                case "10d":
                case "10n":
                    return "shower.png";

                case "11d":
                case "11n":
                    return "thunder.png";

                case "13d":
                case "13n":
                    return "snowy.png";

                default:
                    return "";
            }
        }
    }
}