using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.Devices.Enumeration;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Foundation;
using Windows.Networking.Proximity;
using System.Windows;
using System.IO;
using System.Windows.Threading;

namespace Asclepius.Connectivity
{
    public class BluetoothConnection
    {
        #region "Variables"
        private IAsyncOperation<RfcommDeviceService> connectService;

        private RfcommDeviceService rfcommService;

        private StreamSocket socket;

        private DataWriter dataWriter;

        private DataReader dataReader;

        private BackgroundWorker dataReadWorker;

        public delegate void MessageReceivedHandler(float num1, float num2);

        public event MessageReceivedHandler MessageReceived;
        #endregion

        #region "Public methods"

        public BluetoothConnection() 
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

        public async Task EnumerateDevices()
        {
            var serviceInfoCollection = await DeviceInformation.FindAllAsync(RfcommDeviceService.GetDeviceSelector(RfcommServiceId.SerialPort));
            //var retList = new List<DeviceInformation>();
            listDevices.Clear();
            listNames.Clear();
            foreach (var serviceInfo in serviceInfoCollection)
            {
                listNames.Add(serviceInfo.Name);
                listDevices.Add(serviceInfo);
            }
            //return retList;
        }

        public List<DeviceInformation> listDevices = new List<DeviceInformation>();
        public List<string> listNames = new List<string>();

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
                        dataWriter = new DataWriter(socket.OutputStream);
                        dataReadWorker.RunWorkerAsync();
                    }
                    else
                    {
                        MessageBox.Show("Connection failed");
                    }
                    //await socket.ConnectAsync(serviceInfo., rfcommService.ConnectionServiceName, SocketProtectionLevel.BluetoothEncryptionAllowNullAuthentication);
                    //dataReader = new DataReader(socket.InputStream);
                    //dataReadWorker.RunWorkerAsync();
                    //dataWriter = new DataWriter(socket.OutputStream);
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
                    if (dataReader != null)
                    {
                        await dataReader.LoadAsync(sizeof(float) * 2);
                        if (MessageReceived != null) {
                            Deployment.Current.Dispatcher.BeginInvoke(() => { MessageReceived((float)dataReader.ReadSingle(), (float)dataReader.ReadSingle()); });
                        }
                        //ms.WriteByte(dataReader.ReadByte());

                        //if (ms.Length==sizeof(float) *2) {
                        //    using (BinaryReader objRead = new BinaryReader(ms))
                        //    {
                        //    }
                        //    ms.Close();
                        //    ms = new MemoryStream();
                        //}
                    }
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
