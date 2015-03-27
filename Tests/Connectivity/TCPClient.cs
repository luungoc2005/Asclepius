using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace Asclepius.Connectivity
{
    class TCPClient
    {
        #region "Common variables"

        int intPort = 8019;
        string strHostName = "";
        StreamSocket _tcpClient;

        DataWriter dataWriter;
        DataReader dataReader;

        Task tskMessages;
        CancellationTokenSource objCancelSource;
        CancellationToken objCancelToken;
        #endregion
        
        #region "Class properties"

        public int Port
        {
            get
            {
                return intPort;
            }
            set
            {
                if (value > 0) intPort = value;
            }
        }

        public string hostName
        {
            get
            {
                return strHostName;
            }
            set
            {
                strHostName = value;
            }
        }

        #endregion

        #region "Public methods"

        public bool IsConnected() { return (_tcpClient == null); }

        public async void Connect(string strHost, int intPort)
        {
            try
            {
                strHostName = strHost;
                _tcpClient = new StreamSocket();
                await _tcpClient.ConnectAsync(new HostName(strHost), intPort.ToString());

                dataWriter = new DataWriter(_tcpClient.OutputStream);
                dataReader = new DataReader(_tcpClient.InputStream);

                objCancelSource = new CancellationTokenSource();
                objCancelToken = objCancelSource.Token;
                tskMessages = Task.Factory.StartNew(() => ReceiveThread(), objCancelSource.Token);

                //if (Connected != null) Connected(_tcpClient);
            }
            catch (Exception ex)
            {
                if (SocketError.GetStatus(ex.HResult) == SocketErrorStatus.Unknown)
                {
                    throw;
                }

                if (_tcpClient != null) _tcpClient.Dispose();
            }
        }

        #endregion

        public void Close()
        {
            try
            {
                if (IsConnected())
                {
                    objCancelSource.Cancel();
                    dataWriter.Dispose();
                    dataReader.Dispose();

                    //Cancel timeout
                    tskMessages.Wait(TimeSpan.FromMilliseconds(500));

                    _tcpClient.Dispose();
                    _tcpClient = null;
                }
            }
            catch
            {
            }
        }

        public async void SendBytes(byte[] buffer)
        {
            if ((_tcpClient != null) && (dataWriter != null))
            {
                dataWriter.WriteBytes(buffer);
                try
                {
                    await dataWriter.StoreAsync();
                }
                catch
                {
                    throw;
                }
            }
        }

        #region "Private methods"

        private async void ReceiveThread()
        {
            if (_tcpClient != null)
            {
                while (true)
                {
                    Thread.Sleep(10);

                    objCancelToken.ThrowIfCancellationRequested();

                    try
                    {
                        //MSG type
                        if (await dataReader.LoadAsync(sizeof(byte)) != sizeof(byte))
                        {
                            //Disconnected
                        }

                        byte intMsgType = dataReader.ReadByte();

                        //int intLength = MsgCommon.dictMessages[intMsgType];

                        byte[] bData = new byte[Math.Max(intLength - 1, 0)];

                        if (intLength > 0)
                        {
                            await dataReader.LoadAsync((uint)intLength);

                            dataReader.ReadBytes(bData);
                        }

                        //OnMessageReceived.Invoke(intMsgType, bData);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }

        #endregion
        
    }
}
