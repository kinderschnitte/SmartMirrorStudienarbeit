using System;
using System.Collections.Generic;
using NewsAPI.Constants;

namespace DataAccessLibrary.Module
{
    [Serializable]
    public class Module
    {
        public ModuleType ModuleType { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public string Language { get; set; }

        public LatitudeCoords LatitudeCoords { get; set; } = new LatitudeCoords(49, 0, 25, LatitudeCoords.LatitudeDirection.NORTH);

        public LongitudeCoords LongitudeCoords { get; set; } = new LongitudeCoords(8, 24, 13, LongitudeCoords.LongitudeDirection.EAST);

        public Categories NewsCategory { get; set; }

        public Countries NewsCountry { get; set; }

        public Languages NewsLanguage { get; set; }

        public List<string> NewsSources { get; set; }
    }
}