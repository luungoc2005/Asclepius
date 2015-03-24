using Lumia.Sense;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asclepius.Helpers
{
    class StepCounterHelper
    {
        private static volatile StepCounterHelper _singletonInstance;
        private static Object _syncRoot = new Object();
        private StepCounter _stepCounter;

        public bool isStarted { get; private set; }

        private StepCounterHelper()
        {

        }

        public async void StartCounter()
        {
            isStarted = await CallSensorcoreApiAsync(async () =>
            {
                if (_stepCounter == null)
                {
                    _stepCounter = await StepCounter.GetDefaultAsync();
                }
                else
                {
                    await _stepCounter.ActivateAsync();
                }
            });
        }

        public async void StopCounter()
        {
            if (isStarted && _stepCounter != null)
            {
                await CallSensorcoreApiAsync(async () =>
                    await _stepCounter.DeactivateAsync());
                isStarted = false;
            }
        }

        private async Task<bool> CallSensorcoreApiAsync(Func<Task> action)
        {
            Exception failure = null;
            try
            {
                await action();
            }
            catch (Exception e)
            {
                failure = e;
            }

            if (failure != null)
            {
                switch (SenseHelper.GetSenseError(failure.HResult))
                {
                    case SenseError.LocationDisabled:
                        NotificationsHelper.Instance.pushNotification(1);
                        return false;

                    case SenseError.SenseDisabled:
                        NotificationsHelper.Instance.pushNotification(2);
                        return false;

                    case SenseError.SensorNotAvailable:
                        NotificationsHelper.Instance.pushNotification(3);
                        return false;

                    default:
                        return false;
                }
            }

            return true;
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
    }
}
