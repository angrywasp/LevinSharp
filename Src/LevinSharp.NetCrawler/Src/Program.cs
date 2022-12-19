using System;
using System.Net.Sockets;
using AngryWasp.Logger;
using LevinSharp.Requests;
using AngryWasp.Helpers;
using System.Collections.Generic;
using AngryWasp.Cli.Args;

namespace LevinSharp.NetCrawler
{
    public static class MainClass
    {
        private const string VERSION = "0.18.1.2";
        private const string HOST = "78.106.12.87";
        private const int PORT = 18080;

        [STAThread]
        public static void Main(string[] rawArgs)
        {
            Arguments args = Arguments.Parse(rawArgs);
            Log.CreateInstance(true);

            var crawler = new Crawler();
            crawler.ProbeNode(VERSION, HOST, PORT);

            Log.Instance.SetColor(ConsoleColor.Green);
            Log.Instance.Write("=======================================");
            Log.Instance.SetColor(ConsoleColor.Yellow);
            Log.Instance.Write($"Found {crawler.AllNodes.Count} nodes, verified {crawler.VerifiedNodes.Count}");
            Log.Instance.SetColor(ConsoleColor.Green);
            Log.Instance.Write("---------------------------------------");
            Log.Instance.SetColor(ConsoleColor.White);
            foreach (var n in crawler.VerifiedNodes)
                Log.Instance.Write(n);
            Log.Instance.SetColor(ConsoleColor.Green);
            Log.Instance.Write("---------------------------------------");
            Log.Instance.SetColor(ConsoleColor.White);
        }

        public class Crawler
        {
            private HashSet<string> verifiedNodes = new HashSet<string>();
            private HashSet<string> allNodes = new HashSet<string>();

            public HashSet<string> AllNodes => allNodes;
            public HashSet<string> VerifiedNodes => verifiedNodes;

            public void ProbeNode(string version, string host, int port)
            {
                NetworkConnection nc = new NetworkConnection();
                object pl = nc.Run(version, host, port);

                if (pl != null)
                {
                    verifiedNodes.Add(host);
                    Log.Instance.SetColor(ConsoleColor.Green);
                    Log.Instance.WriteInfo($"Verified Node: {host}");
                    Log.Instance.SetColor(ConsoleColor.White);

                    object[] sec = (object[])pl;
                    foreach (var s in sec)
                    {
                        Section entry = (Section)s;
                        Section adr = (Section)entry.Entries["adr"];
                        Section addr = (Section)adr.Entries["addr"];

                        uint ipInt = 0;

                        if (addr.Entries.ContainsKey("m_ip"))
                            ipInt = (uint)addr.Entries["m_ip"];

                        if (ipInt == 0)
                            continue;

                        string ip = ToIP(ipInt);

                        if (!allNodes.Contains(ip))
                        {
                            allNodes.Add(ip);
                            ProbeNode(VERSION, ip, PORT);
                        }
                    }
                }
            }

            public static string ToIP(uint ip)
            {
                return String.Format("{3}.{2}.{1}.{0}",
                    ip >> 24, (ip >> 16) & 0xff, (ip >> 8) & 0xff, ip & 0xff);
            }
        }

        private static bool Read(TcpClient tcp, out Header header, out Section section)
        {
            header = null;
            section = null;

            NetworkStream ns = tcp.GetStream();

            byte[] headerBuffer = new byte[33];

            int offset = 0;
            int i = ns.Read(headerBuffer, 0, headerBuffer.Length);
            header = Header.FromBytes(headerBuffer, ref offset);

            if (BitShifter.ToULong(headerBuffer) != Constants.LEVIN_SIGNATURE)
            {
                Log.Instance.WriteError("Invalid response from remote node");
                return false;
            }

            if (i < headerBuffer.Length)
            {
                Log.Instance.WriteError("Invalid response from remote node");
                return false;
            }

            offset = 0;
            byte[] buffer = new byte[header.Cb];
            i = ns.Read(buffer, 0, buffer.Length);

            if (i < buffer.Length)
            {
                Log.Instance.WriteError("Invalid response from remote node");
                return false;
            }

            section = null;

            switch (header.Command)
            {
                case Constants.P2P_COMMAND_HANDSHAKE:
                    section = new Handshake().Read(header, buffer, ref offset);
                    break;
                case Constants.P2P_COMMAND_REQUEST_SUPPORT_FLAGS:
                    section = new SupportFlags().Read(header, buffer, ref offset);
                    break;
                default:
                    Log.Instance.WriteError($"Command {header.Command} is not yet supported");
                    return false;
            }

            Log.Instance.WriteInfo($"Read data package {header.Command}");

            if (!ns.DataAvailable)
            {
                Log.Instance.WriteInfo("Network stream ended");
                return false;
            }

            return true;
        }
    }
}
