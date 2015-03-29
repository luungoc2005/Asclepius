using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Asclepius.User;
using System.ComponentModel;

namespace Asclepius.Connectivity
{
    public class UDPHelper : INotifyPropertyChanged
    {
        private static volatile UDPHelper _singletonInstance;
        private static Object _syncRoot = new Object();
        private Dictionary<byte[], AppUser> dictClients = new Dictionary<byte[], AppUser>();

        UDPClient _client;
        UDPClientFinder _finder;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public List<AppUser> GetClientList()
        {
            List<AppUser> retVal = new List<AppUser>();
            foreach (AppUser user in dictClients.Values)
            {
                retVal.Add(user);
            }
            return retVal;
        }

        public static UDPHelper Instance
        {
            get
            {
                if (_singletonInstance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_singletonInstance == null)
                        {
                            _singletonInstance = new UDPHelper();
                        }
                    }
                }
                return _singletonInstance;
            }
        }
        
        public UDPHelper()
        {
            _client = new UDPClient();
            _finder = new UDPClientFinder();
            _finder.OnClientFound += _finder_OnClientFound;
            _client.OnDataReceived += _client_OnDataReceived;
        }

        private void _client_OnDataReceived(byte[] dest, byte msgType, byte[] data)
        {
            AppUser destUser = FindClient(dest);
            if (destUser != null)
            {
                switch (msgType)
                {
                    case 0:
                        destUser.Username = Encoding.Unicode.GetString(data, 0, data.Length);
                        break;
                    case 1:
                        destUser.UserAvatarSerialized = data;
                        SendUserData(dest);
                        break;
                    default:
                        break;
                }
            }
        }

        private async void SendUserData(byte[] dest)
        {
            AppUser user = User.AccountsManager.Instance.CurrentUser;
            if (user != null)
            {
                await _client.SendMessage(0, Encoding.Unicode.GetBytes(user.Username), dest);
                await _client.SendMessage(1, user.UserAvatarSerialized, dest);
            }
        }

        private bool _discovery = false;
        public bool IsDiscoveryEnabled
        {
            get 
            {
                return _discovery;
            }
            set 
            {
                _discovery = value;
                if (_discovery) 
                {
                    _finder.StartFinder();
                    _finder.BroadcastIP();
                    _client.Start();
                }
                else
                {
                    _finder.StopFinder();
                    _client.Stop();
                }
            }
        }

        void _finder_OnClientFound(byte[] clientIP)
        {
            dictClients.Add(clientIP, new AppUser());
            SendUserData(clientIP);
        }

        private AppUser FindClient(byte[] clientIP)
        {
            if (dictClients.ContainsKey(clientIP)) return dictClients[clientIP];
            else return null;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler OnClientUpdate;
    }
}
