using System;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using Vsync;

namespace flyxera3
{
    [DataContract]
    public class Offer : ISelfMarshalled
    {
        // Id is a cryptographically secure unique identifier. 
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Amount { get; set; }
        [DataMember]
        public Place Location { get; set; }
        [DataMember]
        public string Time { get; set; }
        [DataMember]
        public string ShortDescription { get; set; }
        [DataMember]
        public string LongDescription { get; set; }
        [DataMember]
        public User Offerer { get; set; }

        private static RandomNumberGenerator rng = new RNGCryptoServiceProvider();
        private static byte[] token = new byte[32];

        public Offer() { }

        public Offer(string amount, Place location, DateTime time, string shortDescription, string longDescription, User offerer)
        {
            rng.GetBytes(token);
            Id = Convert.ToBase64String(token);

            Amount = amount;
            Location = location;
            Time = time.ToString();
            ShortDescription = shortDescription;
            LongDescription = longDescription;
            Offerer = offerer;
        }


        // Implement ISelfMarshalled.
        public Offer(byte[] ba)
        {
            object[] os = Msg.BArrayToObjects(ba);

            Id = (string)os[0];
            Amount = (string)os[1];
            Location = (Place)os[2];
            Time = (string)os[3];
            ShortDescription = (string)os[4];
            LongDescription = (string)os[5];
            Offerer = (User)os[6];
        }

        public byte[] toBArray()
        {
            return Msg.toBArray(
                Id, 
                Amount, 
                Location, 
                Time,
                ShortDescription, 
                LongDescription, 
                Offerer);
        }
    }
}