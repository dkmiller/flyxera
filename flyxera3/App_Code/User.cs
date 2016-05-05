using System.Runtime.Serialization;
using Vsync;

namespace flyxera3
{
    [DataContract]
    public class User : ISelfMarshalled
    {
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string Name { get; set; }
        public User(string email, string name)
        {
            Email = email;
            Name = name;
        }

        // Vsync code
        public byte[] toBArray()
        {
            return Msg.toBArray(Email, Name);
        }

        public User(byte[] ba)
        {
            object[] os = Msg.BArrayToObjects(ba);
            Email = (string)os[0];
            Name = (string)os[1];
        }
    }
}