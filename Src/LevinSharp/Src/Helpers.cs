using AngryWasp.Helpers;

namespace LevinSharp
{
    public class HexString
    {
        private bool isHex = true;
        private string value;

        public HexString(string value, bool isHex = true)
        {
            this.isHex = isHex;
            this.value = value;
        }

        public byte[] ToBytes()
        {
            if (isHex)
                return value.FromByteHex();
            else
                return value.CharsToByte();
        }

        public override string ToString()
        {
            return value;
        }
    }
}