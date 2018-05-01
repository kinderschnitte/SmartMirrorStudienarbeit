using SQLite.Net.Attributes;

namespace DataAccessLibrary.Tables
{
    public class LocationTable
    {
        [PrimaryKey]
        public int Id { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string Language { get; set; }
    }
}