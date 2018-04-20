using System.Collections.Generic;

namespace Api
{
    public static class Api
    {
        public static Dictionary<ApiEnum, string> ApiKeys { get; }

        public static Dictionary<ApiEnum, string> ApiUrls { get; }

        static Api()
        {
            ApiKeys = new Dictionary<ApiEnum, string>();

            ApiUrls = new Dictionary<ApiEnum, string>();

            addApiParameters();
        }

        private static void addApiParameters()
        {
            // OpenWeatherMap
            ApiKeys.Add(ApiEnum.OPENWEATHERMAP, "4ce3d25d1b8cb5953ba718abd11bd07a");
            ApiUrls.Add(ApiEnum.OPENWEATHERMAP, "http://api.openweathermap.org/data/2.5");

            // NewsAPI
            ApiKeys.Add(ApiEnum.NEWSAPI, "9d6d50c70043491ba1aa1a2048b4197a");

            // Google Maps Geocoding
            ApiKeys.Add(ApiEnum.GOOGLEMAPSGEOCODING, "AIzaSyCUWewgFV1-ALfuAQpGC3S5uHlud1ucj20");
            ApiUrls.Add(ApiEnum.GOOGLEMAPSGEOCODING, "https://maps.googleapis.com/maps/api/geocode/json?address=");
        }
    }
}