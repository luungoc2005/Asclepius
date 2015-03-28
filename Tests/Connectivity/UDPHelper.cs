using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Asclepius.User;

namespace Asclepius.Connectivity
{
    class UDPHelper
    {
        private static volatile UDPHelper _singletonInstance;
        private static Object _syncRoot = new Object();
        private Dictionary<byte[], AppUserEx> dictClients = new Dictionary<byte[], AppUserEx>();

        UDPClient _client;
        UDPClientFinder _finder;

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
            _client.Start();
        }

        private void _client_OnDataReceived(byte[] dest, byte msgType, byte[] data)
        {
            AppUserEx destUser = FindClient(dest);
            if (destUser != null)
            {
                switch (msgType)
                {
                    case 0:
                        destUser.Username = Encoding.Unicode.GetString(data, 0, data.Length);
                        break;
                    case 1:
                        destUser.UserAvatarSerialized = data;
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
                }
                else _finder.StopFinder();
            }
        }

        void _finder_OnClientFound(byte[] clientIP)
        {
            dictClients.Add(clientIP, new AppUserEx());
            SendUserData(clientIP);
        }

        private AppUserEx FindClient(byte[] clientIP)
        {
            if (dictClients.ContainsKey(clientIP)) return dictClients[clientIP];
            else return null;
        }
    }
}
