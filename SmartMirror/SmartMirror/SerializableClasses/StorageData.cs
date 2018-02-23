using System.Runtime.Serialization;

namespace SmartMirror.SerializableClasses
{
    [DataContract]
    public class StorageData
    {
        [DataMember]
        public string Dummy { get; set; }
    }
}