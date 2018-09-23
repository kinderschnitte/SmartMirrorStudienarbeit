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

            AddApiParameters();
        }

        private static void AddApiParameters()
        {
            // OpenWeatherMap
            ApiKeys.Add(ApiEnum.Openweathermap, "4ce3d25d1b8cb5953ba718abd11bd07a");
            ApiUrls.Add(ApiEnum.Openweathermap, "http://api.openweathermap.org/data/2.5");

            // NewsAPI
            ApiKeys.Add(ApiEnum.Newsapi, "9d6d50c70043491ba1aa1a2048b4197a");

            // Google Maps Geocoding
            ApiKeys.Add(ApiEnum.Googlemapsgeocoding, "AIzaSyCUWewgFV1-ALfuAQpGC3S5uHlud1ucj20");
            ApiUrls.Add(ApiEnum.Googlemapsgeocoding, "https://maps.googleapis.com/maps/api/geocode/json?address=");
        }
    }
}