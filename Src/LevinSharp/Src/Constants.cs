using System;
using System.Collections.Generic;

namespace LevinSharp
{
    public class Constants
    {
        public const uint P2P_COMMANDS_POOL_BASE = 1000;

        public const byte LEVIN_OK = 0;
        public const ulong LEVIN_SIGNATURE = 72340172838084865;
        public const uint LEVIN_PACKET_REQUEST = (uint)(0x00000001);
        public const uint LEVIN_PACKET_RESPONSE = (uint)(0x00000002);
        public const uint LEVIN_DEFAULT_MAX_PACKET_SIZE = 100000000;
        public const uint LEVIN_PROTOCOL_VER_1 = 1;

        public const int LEVIN_ERROR_CONNECTION = -1;
        public const int LEVIN_ERROR_CONNECTION_NOT_FOUND = -2;
        public const int LEVIN_ERROR_CONNECTION_DESTROYED = -3;
        public const int LEVIN_ERROR_CONNECTION_TIMEDOUT = -4;
        public const int LEVIN_ERROR_CONNECTION_NO_DUPLEX_PROTOCOL = -5;
        public const int LEVIN_ERROR_CONNECTION_HANDLER_NOT_DEFINED = -6;
        public const int LEVIN_ERROR_FORMAT = -7;

        public const uint P2P_COMMAND_HANDSHAKE = P2P_COMMANDS_POOL_BASE + 1;
        public const uint P2P_COMMAND_PING = P2P_COMMANDS_POOL_BASE + 3;
        public const uint P2P_COMMAND_REQUEST_SUPPORT_FLAGS = P2P_COMMANDS_POOL_BASE + 7;

        public const uint PORTABLE_STORAGE_SIGNATUREA = 16847105;
        public const uint PORTABLE_STORAGE_SIGNATUREB = 16908545;
        public const byte PORTABLE_STORAGE_FORMAT_VER = 1;
        
        public const byte PORTABLE_RAW_SIZE_MARK_MASK = 3;
        public const byte PORTABLE_RAW_SIZE_MARK_BYTE = 0;
        public const byte PORTABLE_RAW_SIZE_MARK_WORD = 1;
        public const byte PORTABLE_RAW_SIZE_MARK_DWORD = 2;
        public const byte PORTABLE_RAW_SIZE_MARK_INT64 = 3;

        public const byte SERIALIZE_TYPE_INT64 = 1;
        public const byte SERIALIZE_TYPE_INT32 = 2;
        public const byte SERIALIZE_TYPE_INT16 = 3;
        public const byte SERIALIZE_TYPE_INT8 = 4;
        public const byte SERIALIZE_TYPE_UINT64 = 5;
        public const byte SERIALIZE_TYPE_UINT32 = 6;
        public const byte SERIALIZE_TYPE_UINT16 = 7;
        public const byte SERIALIZE_TYPE_UINT8 = 8;
        public const byte SERIALIZE_TYPE_DOUBLE = 9;
        public const byte SERIALIZE_TYPE_STRING = 10;
        public const byte SERIALIZE_TYPE_BOOL = 11;
        public const byte SERIALIZE_TYPE_OBJECT = 12;
        public const byte SERIALIZE_TYPE_ARRAY = 13;
        public const byte SERIALIZE_FLAG_ARRAY = 0x80;

        public const string NETWORK_ID = "1230F171610441611731008216A1A110";

        public const string GENESIS_TX_HASH = "771fbcd656ec1464d3a02ead5e18644030007a0fc664c0a964d30922821a8148";

        public static readonly Dictionary<Type, byte> BoostTypes = new Dictionary<Type, byte>
        {
            {typeof(ulong),     SERIALIZE_TYPE_UINT64},
            {typeof(long),      SERIALIZE_TYPE_INT64},
            {typeof(uint),      SERIALIZE_TYPE_UINT32},
            {typeof(int),       SERIALIZE_TYPE_INT32},
            {typeof(ushort),    SERIALIZE_TYPE_UINT16},
            {typeof(short),     SERIALIZE_TYPE_INT16},
            {typeof(HexString), SERIALIZE_TYPE_STRING},
            {typeof(byte),      SERIALIZE_TYPE_UINT8},
            {typeof(sbyte),     SERIALIZE_TYPE_INT8}
        };
    }
}