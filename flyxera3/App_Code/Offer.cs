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
        public double Amount { get; set; }
        [DataMember]
        public Place Location { get; set; }
        [DataMember]
        public DateTime Time { get; set; }
        [DataMember]
        public string ShortDescription { get; set; }
        [DataMember]
        public string LongDescription { get; set; }
        [DataMember]
        public User Offerer { get; set; }

        private static RandomNumberGenerator rng = new RNGCryptoServiceProvider();
        private static byte[] token = new byte[32];

        public Offer(string amount, Place location, DateTime time, string shortDescription, string longDescription, User offerer)
        {
            rng.GetBytes(token);
            Id = Convert.ToBase64String(token);

            Amount = Convert.ToDouble(amount);
            Location = location;
            Time = time;
            ShortDescription = shortDescription;
            LongDescription = longDescription;
            Offerer = offerer;
        }


        // Implement ISelfMarshalled.
        public Offer(byte[] ba)
        {
            WebForm1.Debug("before Offer(byte[])");
            object[] os = Msg.BArrayToObjects(ba);
            WebForm1.Debug("after BArrayToObjects");
            var i = 0;
            Id = (string)os[i++];
            Amount = (double)os[i++];
            Location = (Place)os[i++];
            Time = (DateTime)os[i++];
            ShortDescription = (string)os[i++];
            LongDescription = (string)os[i++];
            Offerer = (User)os[i++];
            WebForm1.Debug("after Offer(byte[])");
        }

        public byte[] toBArray()
        {
            WebForm1.Debug("before toBArray");
            WebForm1.Debug("Id: " + Id);
            WebForm1.Debug("Amount: " + Amount);
            WebForm1.Debug("(Latitude,Longitude): " + Location.Latitude + "," + Location.Longitude);
            WebForm1.Debug("ShortDescription: " + ShortDescription);
            WebForm1.Debug("LongDescription: " + LongDescription);
            WebForm1.Debug("(Email,Name,PhotoUrl): " + Offerer.Email + "," + Offerer.Name + "," + Offerer.PhotoUrl);
            byte[] ba = Msg.toBArray(Id, (object)Amount, Location, ShortDescription, LongDescription, Offerer);
            WebForm1.Debug("after toBArray");
            return ba;
        }
    }
}