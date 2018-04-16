using System.Collections.Generic;

namespace SmartMirrorServer.HelperApis.GoogleMapsGeocoding
{
    internal class City
    {
        public List<Result> Results { get; set; }

        public string Status { get; set; }
    }

    internal class AddressComponent
    {
        public string Long_name { get; set; }

        public string Short_name { get; set; }

        public List<string> Types { get; set; }
    }

    internal class Northeast
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    internal class Southwest
    {
        public double Lat { get; set; }

        public double Lng { get; set; }
    }

    internal class Bounds
    {
        public Northeast Northeast { get; set; }
        public Southwest Southwest { get; set; }
    }

    internal class Location
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }

    internal class Northeast2
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }

    internal class Southwest2
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }

    internal class Viewport
    {
        public Northeast2 Northeast { get; set; }
        public Southwest2 Southwest { get; set; }
    }

    internal class Geometry
    {
        public Bounds Bounds { get; set; }
        public Location Location { get; set; }
        public string Location_type { get; set; }
        public Viewport Viewport { get; set; }
    }

    internal class Result
    {
        public List<AddressComponent> Address_components { get; set; }
        public string Formatted_address { get; set; }
        public Geometry Geometry { get; set; }
        public string Place_id { get; set; }
        public List<string> Postcode_localities { get; set; }
        public List<string> Types { get; set; }
    }
}