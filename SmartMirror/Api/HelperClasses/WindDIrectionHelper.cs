namespace Api.HelperClasses
{
    public static class WindDirectionHelper
    {
        public static string GetWindDirection(double windDegree)
        {
            if (windDegree >= 348.75 && windDegree <= 360 || windDegree >= 0 && windDegree <= 11.25)
                return "Nord";

            if (windDegree > 11.25 && windDegree < 33.75)
                return "Nord Nord Ost";

            if (windDegree >= 33.75 && windDegree <= 56.25)
                return "Nord Ost";

            if (windDegree > 56.25 && windDegree < 78.75)
                return "Ost Nord Ost";

            if (windDegree >= 78.75 && windDegree <= 101.25)
                return "Ost";

            if (windDegree > 101.25 && windDegree < 123.75)
                return "Ost Süd Ost";

            if (windDegree >= 123.75 && windDegree <= 146.25)
                return "Süd Ost";

            if (windDegree > 146.25 && windDegree < 168.75)
                return "Süd Süd Ost";

            if (windDegree >= 168.75 && windDegree <= 191.25)
                return "Süd";

            if (windDegree > 191.25 && windDegree < 213.75)
                return "Süd Süd West";

            if (windDegree >= 213.75 && windDegree <= 236.25)
                return "Süd West";

            if (windDegree > 236.25 && windDegree < 258.75)
                return "West Süd West";

            if (windDegree >= 258.75 && windDegree <= 291.25)
                return "West";

            if (windDegree > 291.25 && windDegree < 303.75)
                return "West Nord West";

            if (windDegree >= 303.75 && windDegree <= 326.25)
                return "Nord West";

            if (windDegree > 326.25 && windDegree < 348.75)
                return "Nord Nord West";

            return "Unknown";
        }
    }
}