using System;
using System.Diagnostics;
using System.Text;
using AngryWasp.Helpers;
using AngryWasp.Logger;

namespace LevinSharp
{
    public class LevinReader
    {
        private Section section = new Section();
        public Section Output => section;

        public bool ReadPayload(Header h, byte[] bytes, ref int offset)
        {
            uint sigA = BitShifter.ToUInt(bytes, ref offset);
            uint sigB = BitShifter.ToUInt(bytes, ref offset);

            if (sigA != Constants.PORTABLE_STORAGE_SIGNATUREA ||
                sigB != Constants.PORTABLE_STORAGE_SIGNATUREB)
            {
                Log.Instance.WriteError("Portable storage signature mismatch");
                return false;
            }

            if (bytes[offset++] != Constants.PORTABLE_STORAGE_FORMAT_VER)
            {
                Log.Instance.WriteError("Portable storage format version mismatch");
                return false;
            }

            //todo: Need to fail here if required
            section = ReadSection(bytes, ref offset);
            return true;
        }

        private Section ReadSection(byte[] bytes, ref int offset)
        {
            long count = VarInt.From(bytes, ref offset);

            if (count == 0)
                return null;

            Section sec = new Section();

            for (int i = 0; i < count; i++)
            {
                string name = ReadSectionName(bytes, ref offset);
                object entry = ReadEntry(bytes, ref offset);
                sec.Add(name, entry);
            }

            return sec;
        }

        private string ReadSectionName(byte[] bytes, ref int offset)
        {
            byte length = bytes[offset++];
            string name = Encoding.ASCII.GetString(bytes, offset, length);
            offset += length;

            return name;
        }

        private object ReadEntry(byte[] bytes, ref int offset)
        {
            byte type = bytes[offset++];

            if ((type & Constants.SERIALIZE_FLAG_ARRAY) != 0)
                return LoadStorageArrayEntry(type, bytes, ref offset);

            if (type == Constants.SERIALIZE_TYPE_ARRAY)
            {
                type = bytes[offset++];
                if ((type & Constants.SERIALIZE_FLAG_ARRAY) != 0)
                {
                    Log.Instance.WriteError("Wrong type sequences");
                    return null;
                }

                return LoadStorageArrayEntry(type, bytes, ref offset);
            }

            return Read(type, bytes, ref offset);
        }

        private object Read(byte type, byte[] bytes, ref int offset)
        {
            switch (type)
            {
                case Constants.SERIALIZE_TYPE_UINT64:
                    return BitShifter.ToULong(bytes, ref offset);
                case Constants.SERIALIZE_TYPE_INT64:
                    return BitShifter.ToLong(bytes, ref offset);
                case Constants.SERIALIZE_TYPE_UINT32:
                    return BitShifter.ToUInt(bytes, ref offset);
                case Constants.SERIALIZE_TYPE_INT32:
                    return BitShifter.ToInt(bytes, ref offset);
                case Constants.SERIALIZE_TYPE_UINT16:
                    return BitShifter.ToUShort(bytes, ref offset);
                case Constants.SERIALIZE_TYPE_INT16:
                    return BitShifter.ToShort(bytes, ref offset);
                case Constants.SERIALIZE_TYPE_UINT8:
                    return (byte)(bytes[offset++]);
                case Constants.SERIALIZE_TYPE_INT8:
                    return (sbyte)(bytes[offset++]);
                case Constants.SERIALIZE_TYPE_STRING:
                    return ReadByteArray(bytes, ref offset);
                case Constants.SERIALIZE_TYPE_OBJECT:
                    return ReadSection(bytes, ref offset);
            }

            return null;
        }

        private object LoadStorageArrayEntry(int type, byte[] bytes, ref int offset)
        {
            type &= ~Constants.SERIALIZE_FLAG_ARRAY;

            long count = VarInt.From(bytes, ref offset);
            object[] data = new object[count];

            for (int i = 0; i < count; i++)
                data[i] = Read((byte)type, bytes, ref offset);
            
            return data;
        }

        private object ReadByteArray(byte[] bytes, ref int offset)
        {
            long count = VarInt.From(bytes, ref offset);
            byte[] destination = new byte[count];
            Buffer.BlockCopy(bytes, offset, destination, 0, (int)count);
            offset += (int)count;
            return destination; 
        }
    }
}