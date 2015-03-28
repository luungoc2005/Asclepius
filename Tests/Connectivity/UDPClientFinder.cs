﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using System.IO;
using System.Net;

namespace Asclepius.Connectivity
{
    class UDPClientFinder
    {
        public event ClientFoundEvent OnClientFound;
        public delegate void ClientFoundEvent(byte[] clientIP);

        DatagramSocket socket;
        DatagramSocket socket2;

        private string finderPort = "7194";

        public static string LocalIPAddress()
        {
            List<string> ipAddresses = new List<string>();
            var hostnames = NetworkInformation.GetHostNames();
            foreach (var hn in hostnames)
            {
                //IanaInterfaceType == 71 => Wifi
                //IanaInterfaceType == 6 => Ethernet (Emulator)
                if (hn.IPInformation != null &&
                    (hn.IPInformation.NetworkAdapter.IanaInterfaceType == 71
                    || hn.IPInformation.NetworkAdapter.IanaInterfaceType == 6))
                {
                    string ipAddress = hn.DisplayName;
                    ipAddresses.Add(ipAddress);
                }
            }

            if (ipAddresses.Count < 1)
            {
                return null;
            }
            else if (ipAddresses.Count == 1)
            {
                return ipAddresses[0];
            }
            else
            {
                //if multiple suitable address were found use the last one
                //(regularly the external interface of an emulated device)
                return ipAddresses[ipAddresses.Count - 1];
            }
        }

        public async void StartFinder()
        {
            socket2 = new DatagramSocket();
            await socket2.BindEndpointAsync(new HostName(IPAddress.Broadcast.ToString()), finderPort);            
            socket2.MessageReceived += SocketOnMessageReceived;
        }

        public void StopFinder()
        {
            if (socket != null) socket.Dispose();
            if (socket2 != null) socket2.Dispose();
        }

        public async void BroadcastIP()
        {
            await SendMessage(IPMessage(), IPAddress.Broadcast.ToString(), finderPort);
        }

        private byte[] IPMessage()
        {
            //IP Address to byte()
            string[] strIPTemp = LocalIPAddress().Split('.');
            //if (strIPTemp.Length != 4) throw new Exception("Invalid IP Address");
            return Enumerable.Range(0, 5).Select(x =>
                        (x == 0) ? (byte)1 : Convert.ToByte(strIPTemp[x - 1])).ToArray();
        }

        private async Task SendMessage(byte[] message, string host, string port)
        {
            socket = new DatagramSocket();
            using (var stream = await socket.GetOutputStreamAsync(new HostName(host), port))
            {
                using (var writer = new DataWriter(stream))
                {
                    writer.WriteBytes(message);
                    await writer.StoreAsync();
                }
            }
            socket.Dispose();
        }

        private async void SocketOnMessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
        {
            var result = args.GetDataStream();
            var resultStream = result.AsStreamForRead(1024);

            try
            {
                byte[] buffer = new byte[5];
                byte[] message = IPMessage();
                await resultStream.ReadAsync(buffer, 0, 5);
                for (int i = 0; i <= 5; i++) //compare arrays
                {
                    if (buffer[i] != message[i])
                    {
                        if (buffer[0] == 1 && OnClientFound != null)
                        {
                            await SendMessage(message,
                                args.RemoteAddress.ToString(), args.RemotePort);
                            OnClientFound(buffer.Skip(1).ToArray());
                            break;
                        }
                    }
                }
            }
            catch
            { //TODO: Error handler
            }
        }
    }
}
