using System.Runtime.Serialization;
using SmartMirror.Enums;

namespace SmartMirror.SerializableClasses
{
    [DataContract]
    public class StorageData
    {
        [DataMember]
        public Module UpperLeftModule { get; set; } = Module.NONE;

        [DataMember]
        public Module UpperMiddleModule { get; set; } = Module.NONE;

        [DataMember]
        public Module UpperRightModule { get; set; } = Module.NONE;

        [DataMember]
        public Module MiddleLeftModule { get; set; } = Module.NONE;

        [DataMember]
        public Module MiddleMiddleModule { get; set; } = Module.NONE;

        [DataMember]
        public Module MiddleRightModule { get; set; } = Module.NONE;

        [DataMember]
        public Module LowerLeftModule { get; set; } = Module.NONE;

        [DataMember]
        public Module LowerMiddleModule { get; set; } = Module.NONE;

        [DataMember]
        public Module LowerRightModule { get; set; } = Module.NONE;
    }
}