using System.Runtime.Serialization;

namespace SmartMirrorServer.Enums
{
    [DataContract(Name = "Module")]
    public enum Module
    {
        [EnumMember]
        NONE,
        [EnumMember]
        TIME,
        [EnumMember]
        WEATHER
    }
}