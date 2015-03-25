using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asclepius.Connectivity
{
    class ClientsManager
    {
        private static volatile ClientsManager _singletonInstance;
        private static Object _syncRoot = new Object();

        public static ClientsManager Instance
        {
            get
            {
                if (_singletonInstance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_singletonInstance == null)
                        {
                            _singletonInstance = new ClientsManager();
                        }
                    }
                }
                return _singletonInstance;
            }
        }

        public ClientsManager()
        {

        }
        

    }
}
