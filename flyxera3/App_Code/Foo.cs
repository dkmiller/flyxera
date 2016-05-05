using System.Runtime.Serialization;
using Vsync;

namespace flyxera3
{
    [AutoMarshalled]
    [DataContract]
    public class Foo
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Val { get; set; }
    }
}