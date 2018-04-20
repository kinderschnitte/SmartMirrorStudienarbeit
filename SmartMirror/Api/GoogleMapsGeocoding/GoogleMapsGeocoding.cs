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

            Coordinates coordinates = new Coordinates { Latitude = getLatitude(c), Longitude = getLongitude(c) };

            return coordinates;
        }

        private static LongitudeCoords getLongitude(City city)
        {
            double lng = city.Results.First().Geometry.Location.Lng;

            int degrees = (int)lng;
            int minutes = (int)((lng - degrees) * 60);
            int seconds = (int)((lng - degrees - minutes / 60) * 3600) % 60;

            return new LongitudeCoords(degrees, minutes, seconds, lng > 0 ? LongitudeCoords.LongitudeDirection.EAST : LongitudeCoords.LongitudeDirection.WEST);
        }

        private static LatitudeCoords getLatitude(City city)
        {
            double lat = city.Results.First().Geometry.Location.Lat;

            int degrees = (int)lat;
            int minutes = (int)((lat - degrees) * 60);
            int seconds = (int)((lat - degrees - minutes / 60) * 3600) % 60;

            return new LatitudeCoords(degrees, minutes, seconds, lat > 0 ? LatitudeCoords.LatitudeDirection.NORTH : LatitudeCoords.LatitudeDirection.SOUTH);
        }
    }
}