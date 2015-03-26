using Lumia.Sense;
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
        private StepCounter _stepCounter;
        private DispatcherTimer _pollTimer;

        public bool isStarted { get; private set; }

        public bool isWalking { get; set; }

        private uint lastStepCount;
        private uint currentStepCount;

        StepCounterReading startingReading;

        public Models.MainPageModel mainModel;

        private StepCounterHelper()
        {

        }

        public async Task UpdateCounterAsync()
        {
            StepCounterReading current = null;
            bool res = await CallSensorcoreApiAsync(async () => { current = await _stepCounter.GetCurrentReadingAsync(); });
            StepCounterReading beginOfDay = await FirstReadingForTodayAsync();

            if (current != null && beginOfDay != null && res)
            {
                currentStepCount = current.WalkingStepCount + current.RunningStepCount;
                if (lastStepCount >= currentStepCount)
                {
                    isWalking = false;
                }
                else
                {
                    isWalking = true;
                }

                lastStepCount = currentStepCount;

                if (mainModel != null)
                {
                    mainModel.TotalSteps = currentStepCount;
                    mainModel.RunningSteps = current.RunningStepCount;
                    mainModel.Distance = Math.Round(currentStepCount * AccountsManager.Instance.CurrentUser.StrideLength / 1000, 1);
                }
            }            
        }

        private async Task UpdateUserRecordAsync()
        {
            if (AccountsManager.Instance.CurrentUser == null) return;

            Record record = AccountsManager.Instance.CurrentUser.Records[0];
            DateTime now = DateTime.Now;
            if (record == null || record.EndDate.CompareTo(now) < 0)
            {
                Record newRecord = new Record();
                newRecord.StartDate = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);
                newRecord.EndDate = now;
                newRecord.Stride = AccountsManager.Instance.CurrentUser.StrideLength;

                AccountsManager.Instance.CurrentUser.Records.Insert(0, newRecord);

                //AccountsManager.Instance.CurrentUser.SortLists();

                record = newRecord;
            }
            else
            {
                record.EndDate = now;
            }

            var results = await _stepCounter.GetStepCountHistoryAsync(new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0),
                TimeSpan.FromHours(1));

            if (record != null && results != null)
            {
                record.stepReading = results[results.Count - 1];
            }
        }

        private async Task<StepCounterReading> FirstReadingForTodayAsync()
        {
            var results = await _stepCounter.GetStepCountHistoryAsync(DateTime.Now.Date, TimeSpan.FromDays(1));

            if (results != null)
                return results[0];
            else
                return null;
        }

        private async Task<StepCounterReading> FirstReadingForHourAsync(DateTime time)
        {
            var results = await _stepCounter.GetStepCountHistoryAsync(new DateTime(time.Year,time.Month,time.Day,time.Hour,0,0), 
                TimeSpan.FromHours(1));

            if (results != null)
                return results[0];
            else
                return null;
        }

        public async Task StartCounterAsync()
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

            if (isStarted) startingReading = await _stepCounter.GetCurrentReadingAsync();

            _pollTimer = new DispatcherTimer();
            _pollTimer.Interval = TimeSpan.FromSeconds(5);
            _pollTimer.Tick += PollTimerTick;
            _pollTimer.Start();
        }

        private async void PollTimerTick(object sender, EventArgs e)
        {
            //if (mainModel != null) mainModel.TotalSteps += 1000; //comment this line
            if (isStarted)
            {
                await UpdateCounterAsync();
                await UpdateUserRecordAsync();
            }
        }

        public async Task StopCounterAsync()
        {
            if (_pollTimer != null)
            {
                _pollTimer.Stop();
                _pollTimer = null;
            }

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
