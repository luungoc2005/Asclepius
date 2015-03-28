﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using System.IO;
using Windows.Storage.Streams;

namespace Asclepius.Connectivity
{
    class UDPClient
    {
        public delegate void DataReceivedEvent(byte[] dest, byte msgType, byte[] data);
        public event DataReceivedEvent OnDataReceived;
        
        DatagramSocket socket;
        DatagramSocket socket2;

        string udptPort = "8019";
        
        public async void Start()
        {
            socket2 = new DatagramSocket();
            await socket2.BindEndpointAsync(new HostName(IPAddress.Any.ToString()), udptPort);
            socket2.MessageReceived += SocketOnMessageReceived;
        }

        private async void SocketOnMessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
        {
            var result = args.GetDataStream();
            //var resultStream = result.AsStreamForRead();
            var dataReader = new DataReader(result);

            try
            {
                byte[] destination = new byte[4];
                await dataReader.LoadAsync(4);
                dataReader.ReadBytes(destination);

                await dataReader.LoadAsync(sizeof(byte));
                byte msgType = dataReader.ReadByte();

                await dataReader.LoadAsync(sizeof(Int32));
                int length = dataReader.ReadInt32();

                byte[] bData = new byte[Math.Max(length - 1, 0)];
                await dataReader.LoadAsync((uint)length);
                dataReader.ReadBytes(bData);

                dataReader.Dispose();

                if (OnDataReceived != null) OnDataReceived(destination, msgType, bData);
            }
            catch
            { //TODO: Error handler
            }
        }

        public void Stop()
        {
            if (socket != null) socket.Dispose();
            if (socket2 != null) socket2.Dispose();
        }

        public async Task SendMessage(byte msgType, byte[] message, byte[] host)
        {
            socket = new DatagramSocket();
            string _host = host[0].ToString() + "." + host[1].ToString() + "." + host[2].ToString() + "." + host[3].ToString();
            using (var stream = await socket.GetOutputStreamAsync(new HostName(_host), udptPort))
            {
                using (var writer = new DataWriter(stream))
                {
                    writer.WriteByte(msgType);
                    writer.WriteInt32(message.Length);
                    writer.WriteBytes(message);
                    await writer.StoreAsync();
                }
            }
            socket.Dispose();
        }
    }
}
