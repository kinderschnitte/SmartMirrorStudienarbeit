using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Api.GoogleMapsGeocoding
{
    public static class Deserializer
    {
        public static City GetCoordinatesByName(JObject response)
        {
            JsonSerializer serializer = new JsonSerializer();

            City city = (City) serializer.Deserialize(new JTokenReader(response), typeof(City));

            return city;
        }
    }
}
