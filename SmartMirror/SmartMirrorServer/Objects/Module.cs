using System.Collections.Generic;
using System.Runtime.Serialization;
using NewsAPI.Constants;
using SmartMirrorServer.Enums;
using SmartMirrorServer.Features.SunTimes;

namespace SmartMirrorServer.Objects
{
    [DataContract]
    internal class Module
    {
        [DataMember]
        public ModuleType ModuleType { get; set; }

        [DataMember]
        public string City { get; set; }

        [DataMember]
        public string Country { get; set; }

        [DataMember]
        public string Language { get; set; }

        [DataMember]
        public LatitudeCoords LatitudeCoords { get; set; } = new LatitudeCoords(49, 0, 25, LatitudeCoords.Direction.NORTH);

        [DataMember]
        public LongitudeCoords LongitudeCoords { get; set; } = new LongitudeCoords(8, 24, 13, LongitudeCoords.Direction.EAST);

        [DataMember]
        public Categories NewsCategory { get; set; }

        [DataMember]
        public Countries NewsCountry { get; set; }

        [DataMember]
        public Languages NewsLanguage { get; set; }

        [DataMember]
        public List<string> NewsSources { get; set; }
    }
}