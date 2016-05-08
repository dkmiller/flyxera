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
        [DataMember]
        public string PhotoUrl { get; set; }

        public User(string email, string name, string photoUrl)
        {
            Email = email;
            Name = name;
            PhotoUrl = photoUrl;
        }

        // Vsync code
        public byte[] toBArray()
        {
//            WebForm1.Debug("User toBArray 1");
            byte[] ba = Msg.toBArray(Email, Name, PhotoUrl);
//            WebForm1.Debug("User toBArray 2");
            return ba;
        }

        public User(byte[] ba)
        {
            object[] os = Msg.BArrayToObjects(ba);
            Email = (string)os[0];
            Name = (string)os[1];
            PhotoUrl = (string)os[2];
        }
    }
}