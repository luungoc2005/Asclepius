using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Asclepius.Connectivity.Messages;
using Windows.Devices.Enumeration;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Foundation;

namespace Asclepius.Connectivity
{
    public class ConnectionManager
    {
        #region "Variables"
        private IAsyncOperation<RfcommDeviceService> connectService;

        private RfcommDeviceService rfcommService;

        private StreamSocket socket;

        private DataWriter dataWriter;

        private DataReader dataReader;

        private BackgroundWorker dataReadWorker;

        public delegate void MessageReceivedHandler(int ID, byte[] data);

        public event MessageReceivedHandler MessageReceived;
        #endregion

        #region "Public methods"

        public void Initialize()
        {
            socket = new StreamSocket();
            dataReadWorker = new BackgroundWorker();
            dataReadWorker.WorkerSupportsCancellation = true;
            dataReadWorker.DoWork += new DoWorkEventHandler(ReceiveMessages);
        }

        public void Terminate()
        {
            if (socket != null)
            {
                socket.Dispose();
            }
            if (dataReadWorker != null)
            {
                dataReadWorker.CancelAsync();
            }
        }

        public async Task<List<DeviceInformation>> EnumerateDevices()
        {
            var serviceInfoCollection = await DeviceInformation.FindAllAsync(RfcommDeviceService.GetDeviceSelector(RfcommServiceId.SerialPort));
            var retList = new List<DeviceInformation>();
            foreach (var serviceInfo in serviceInfoCollection)
                retList.Add(serviceInfo);
            return retList;
        }

        public async void Connect(DeviceInformation serviceInfo)
        {
            if (socket != null)
            {
                try
                {
                    connectService = RfcommDeviceService.FromIdAsync(serviceInfo.Id);
                    rfcommService = await connectService;
                    if (rfcommService != null)
                    {
                        await socket.ConnectAsync(rfcommService.ConnectionHostName, rfcommService.ConnectionServiceName, SocketProtectionLevel.BluetoothEncryptionAllowNullAuthentication);
                        dataReader = new DataReader(socket.InputStream);
                        dataReadWorker.RunWorkerAsync();
                        dataWriter = new DataWriter(socket.OutputStream);
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }
        
        public async Task<bool> SendCommand(byte[] buffer)
        {
            if (dataWriter != null)
            {
                dataWriter.WriteBytes(buffer);

                try
                {
                    await dataWriter.StoreAsync();
                    return true;
                }
                catch
                {
                    return false;
                    throw;
                }
            }
            else return false;
        }

        #endregion

        #region "Private methods"

        private async void ReceiveMessages(object sender, DoWorkEventArgs e)
        {
            try
            {
                while (true)
                {
                    //MSG type
                    if (await dataReader.LoadAsync(sizeof(byte)) != sizeof(byte))
                    {
                        //Disconnected
                    }

                    byte intMsgType = dataReader.ReadByte();

                    //int intLength = MsgCommon.dictMessages[intMsgType];
                    int intLength = sizeof(float);

                    byte[] bData = new byte[Math.Max(intLength - 1, 0)];

                    if (intLength > 0)
                    {
                        await dataReader.LoadAsync((uint)intLength);

                        dataReader.ReadBytes(bData);
                    }

                    MessageReceived(intMsgType, bData);
                }
            }
            catch //(Exception ex)
            {
                //Debug.WriteLine(ex.Message);
            }

        }


        #endregion
        
    }
}
