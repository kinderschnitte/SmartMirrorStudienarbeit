using System.Runtime.Serialization;
using SmartMirrorServer.Objects;

namespace SmartMirrorServer.SerializableClasses
{
    [DataContract]
    internal class StorageData
    {
        [DataMember]
        public Module UpperLeftModule { get; set; } = new Module();

        [DataMember]
        public Module UpperRightModule { get; set; } = new Module();

        [DataMember]
        public Module MiddleLeftModule { get; set; } = new Module();

        [DataMember]
        public Module MiddleRightModule { get; set; } = new Module();

        [DataMember]
        public Module LowerLeftModule { get; set; } = new Module();

        [DataMember]
        public Module LowerRightModule { get; set; } = new Module();
    }
}