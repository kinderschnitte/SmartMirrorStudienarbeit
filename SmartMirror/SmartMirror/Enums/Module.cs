using System.Runtime.Serialization;

namespace SmartMirror.Enums
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