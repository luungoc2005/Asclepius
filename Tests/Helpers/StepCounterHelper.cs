using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Asclepius.User;
using System.Windows.Threading;

namespace Asclepius.Helpers
{
    class StepCounterHelper
    {
        private static volatile StepCounterHelper _singletonInstance;
        private static Object _syncRoot = new Object();
        private DispatcherTimer _pollTimer;

        public bool isStarted { get; private set; }

        public bool isWalking { get; set; }

        private uint lastStepCount;
        private uint currentStepCount;

        public Models.MainPageModel mainModel;

        private StepCounterHelper()
        {

        }

        public static StepCounterHelper Instance
        {
            get
            {
                if (_singletonInstance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_singletonInstance == null)
                        {
                            _singletonInstance = new StepCounterHelper();
                        }
                    }
                }
                return _singletonInstance;
            }
        }

        internal Task StartCounterAsync()
        {
            throw new NotImplementedException();
        }

        internal Task StopCounterAsync()
        {
            throw new NotImplementedException();
        }
    }
}
