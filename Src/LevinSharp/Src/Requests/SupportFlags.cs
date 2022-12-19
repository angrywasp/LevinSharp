using System;

namespace LevinSharp.Requests
{
    public class SupportFlags
    {
        public byte[] Create(uint port)
        {
            throw new NotImplementedException();
        }

        public Section Read(Header header, byte[] buffer, ref int offset)
        {
            LevinReader reader = new LevinReader();
            reader.ReadPayload(header, buffer, ref offset);
            return reader.Output;
        }
    }
}