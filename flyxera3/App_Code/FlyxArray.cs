using System;
using System.Collections.Generic;
using Vsync;

namespace flyxera3
{
    internal class FlyxArray<T> : ISelfMarshalled
    {
        private List<T> Content;

        public FlyxArray(List<T> l)
        {
            Content = l;
        }

        public FlyxArray(byte[] ba)
        {
            var os = Msg.BArrayToObjects(ba);
            Content = new List<T>();
            foreach (var o in os)
                Content.Add((T)o);
        }

        public byte[] toBArray()
        {
            return Msg.toBArray(Content.ToArray());
        }
    }
}