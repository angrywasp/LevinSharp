using System;
using System.Collections.Generic;
using AngryWasp.Helpers;

namespace LevinSharp.Requests
{
    public class Handshake
    {
        public byte[] Create(string version, int port)
        {
            Section section = new Section();

            Section nodeData = new Section();
            nodeData.Add("local_time", DateTimeHelper.TimestampNow);
            nodeData.Add("my_port", (uint)port);
            nodeData.Add("network_id", new HexString(Constants.NETWORK_ID));
            nodeData.Add("peer_id", new XoShiRo256PlusPlus().Next());
            nodeData.Add("version", new HexString(version, false));

            Section payloadData = new Section();
            payloadData.Add("cumulative_difficulty", (ulong)1);
            payloadData.Add("current_height", (ulong)1);
            payloadData.Add("top_id", new HexString(Constants.GENESIS_TX_HASH));
            payloadData.Add("top_version", (byte)1);

            section.Add("node_data", nodeData);
            section.Add("payload_data", payloadData);

            LevinWriter writer = new LevinWriter();
            writer.WritePayload(section);
            
            List<byte> ret = new List<byte>();

            ret.AddRange(new Header {
                Cb = (ulong)writer.Output.Count,
                HaveToReturnData = true,
                Command = Constants.P2P_COMMAND_HANDSHAKE,
                ReturnCode = 0,
                Flags = Constants.LEVIN_PACKET_REQUEST
            }.ToBytes());

            ret.AddRange(writer.Output);

            return ret.ToArray();
        }

        public Section Read(Header header, byte[] buffer, ref int offset)
        {
            LevinReader reader = new LevinReader();
            reader.ReadPayload(header, buffer, ref offset);
            return reader.Output;
        }
    }
}