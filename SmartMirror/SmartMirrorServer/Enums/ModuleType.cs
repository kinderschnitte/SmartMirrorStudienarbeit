using System.Runtime.Serialization;

namespace SmartMirrorServer.Enums
{
    [DataContract(Name = "Module")]
    public enum ModuleType
    {
        [EnumMember]
        NONE,
        [EnumMember]
        TIME,
        [EnumMember]
        WEATHER,
        [EnumMember]
        WEATHERFORECAST,
        [EnumMember]
        NEWS,
        [EnumMember]
        QUOTEOFDAY
    }
}