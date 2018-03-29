using System.Collections.Generic;
using System.Runtime.Serialization;
using NewsAPI.Constants;
using SmartMirrorServer.Enums;
using SmartMirrorServer.Objects;
using SmartMirrorServer.Objects.Sun;

namespace SmartMirrorServer.SerializableClasses
{
    [DataContract]
    internal class StorageData
    {
        public StorageData()
        {
            UpperLeftModule = new Module { ModuleType = ModuleType.TIME, LongitudeCoords = new LongitudeCoords(8, 24, 13, LongitudeCoords.Direction.EAST), LatitudeCoords = new LatitudeCoords(49, 0, 25, LatitudeCoords.Direction.NORTH) };
            UpperRightModule = new Module { ModuleType = ModuleType.WEATHER, City = "Karlsruhe", Country = "Germany", Language = "de"};
            MiddleLeftModule = new Module { ModuleType = ModuleType.NEWS, NewsLanguage = Languages.DE, NewsSources = new List<string> { "bild", "der-tagesspiegel", "die-zeit", "focus" } };
            MiddleRightModule = new Module { ModuleType = ModuleType.NEWS, NewsLanguage = Languages.DE, NewsCountry = Countries.DE, NewsCategory = Categories.Sports};
            LowerLeftModule = new Module { ModuleType = ModuleType.QUOTEOFDAY };
            LowerRightModule = new Module { ModuleType = ModuleType.WEATHERFORECAST, City = "Karlsruhe", Country = "Germany", Language = "de" };

            TimeModul = new Module { ModuleType = ModuleType.TIME, LongitudeCoords = new LongitudeCoords(8, 24, 13, LongitudeCoords.Direction.EAST), LatitudeCoords = new LatitudeCoords(49, 0, 25, LatitudeCoords.Direction.NORTH) };
            WeatherModul = new Module { ModuleType = ModuleType.WEATHER, City = "Karlsruhe", Country = "Germany", Language = "de" };
            WeatherforecastModul = new Module { ModuleType = ModuleType.WEATHERFORECAST, City = "Karlsruhe", Country = "Germany", Language = "de" };
            QuoteModul = new Module { ModuleType = ModuleType.QUOTEOFDAY };
        }

        [DataMember]
        public Module UpperLeftModule { get; set; }

        [DataMember]
        public Module UpperRightModule { get; set; }

        [DataMember]
        public Module MiddleLeftModule { get; set; }

        [DataMember]
        public Module MiddleRightModule { get; set; }

        [DataMember]
        public Module LowerLeftModule { get; set; }

        [DataMember]
        public Module LowerRightModule { get; set; }

        [DataMember]
        public Module TimeModul { get; set; }

        [DataMember]
        public Module WeatherModul { get; set; }

        [DataMember]
        public Module WeatherforecastModul { get; set; }

        [DataMember]
        public Module QuoteModul { get; set; }
    }
}