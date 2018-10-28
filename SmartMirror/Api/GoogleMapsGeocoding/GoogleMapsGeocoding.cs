using System;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLibrary.Module;
using Newtonsoft.Json.Linq;

namespace Api.GoogleMapsGeocoding
{
    public static class GoogleMapsGeocoding
    {
        public static async Task<Coordinates> GetCoordinatesForCity(string city, string state, string country)
        {
            JObject jobject = await ApiClient.GetResponse(city, state, country);
            City c = Deserializer.GetCoordinatesByName(jobject);

            if (c.Status != "OK")
                return null;

            Coordinates coordinates = new Coordinates { Latitude = GetLatitude(c), Longitude = GetLongitude(c) };

            return coordinates;
        }

        public static async Task<Coordinates> GetCoordinatesForPostal(string postal, string country)
        {
            try
            {
                JObject jobject = await ApiClient.GetResponse(postal, country);
                City c = Deserializer.GetCoordinatesByName(jobject);

                if (c.Status != "OK")
                    return null;

                Coordinates coordinates = new Coordinates { Latitude = GetLatitude(c), Longitude = GetLongitude(c) };

                return coordinates;
            }
            catch (Exception exception)
            {
                Log.Log.WriteException(exception);
                return new Coordinates();
            }
        }

        private static LongitudeCoords GetLongitude(City city)
        {
            try
            {
                double lng = city.Results.First().Geometry.Location.Lng;

                int degrees = (int)lng;
                int minutes = (int)((lng - degrees) * 60);
                int seconds = (int)((lng - degrees - minutes / 60) * 3600) % 60;

                return new LongitudeCoords(degrees, minutes, seconds, lng > 0 ? LongitudeCoords.LongitudeDirection.East : LongitudeCoords.LongitudeDirection.West);
            }
            catch (Exception exception)
            {
                Log.Log.WriteException(exception);
                return new LongitudeCoords(0, 0, 0, LongitudeCoords.LongitudeDirection.East);
            }
        }

        private static LatitudeCoords GetLatitude(City city)
        {
            try
            {
                double lat = city.Results.First().Geometry.Location.Lat;

                int degrees = (int)lat;
                int minutes = (int)((lat - degrees) * 60);
                int seconds = (int)((lat - degrees - minutes / 60) * 3600) % 60;

                return new LatitudeCoords(degrees, minutes, seconds, lat > 0 ? LatitudeCoords.LatitudeDirection.North : LatitudeCoords.LatitudeDirection.South);
            }
            catch (Exception exception)
            {
                Log.Log.WriteException(exception);
                return new LatitudeCoords(0, 0, 0, LatitudeCoords.LatitudeDirection.North);
            }
        }
    }
}