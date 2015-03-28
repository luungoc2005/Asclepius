using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Asclepius.User;
using System.Windows.Threading;
using Microsoft.Phone.Applications.Common;
using System.Windows;

namespace Asclepius.Helpers
{
    class StepCounterHelper
    {
        private static volatile StepCounterHelper _singletonInstance;
        private static Object _syncRoot = new Object();

        public Models.MainPageModel mainModel;

        private AccelerometerHelper _accelerometer = AccelerometerHelper.Instance;

        uint _totalSteps = 0; uint _runningSteps = 0;

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

        public void StartCounter()
        {
            _accelerometer.Active = true;
            _accelerometer.ReadingChanged += _accelerometer_ReadingChanged;
            _pollTimer = new DispatcherTimer();
            _pollTimer.Interval = TimeSpan.FromSeconds(1);
            _pollTimer.Tick += _pollTimer_Tick;
            _pollTimer.Start();
        }

        public void InitializeNumbers()
        {
            AppUser user = User.AccountsManager.Instance.CurrentUser;
            if (user == null) return;

            DateTime _day = DateTime.Now;
            DateTime _start = new DateTime(_day.Year, _day.Month, _day.Day, 0, 0, 0);

            Record firstRecord = user.GetHourlyRecord(_start, true);

            WalkTime = 0; RunTime = 0;

            if (firstRecord != null)
            {
                WalkTime = firstRecord.WalkTime;
                RunTime = firstRecord.RunTime;
                _totalSteps = (uint)firstRecord.WalkingStepCount + (uint)firstRecord.RunningStepCount;
                _runningSteps = (uint)firstRecord.RunningStepCount;
            }

            for (int i = 1; i < 24; i++)
            {
                _start = _start.AddHours(1);
                Record current = user.GetHourlyRecord(_start, true);

                if (current != null)
                {
                    WalkTime += current.WalkTime;
                    RunTime += current.RunTime;
                    _totalSteps += (uint)current.WalkingStepCount + (uint)current.RunningStepCount;
                    _runningSteps += (uint)current.RunningStepCount;
                }
            }

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                if (mainModel != null)
                {
                    mainModel.TotalSteps = _totalSteps;
                    mainModel.RunningSteps = _runningSteps;
                    mainModel.WalkTime = WalkTime;
                    mainModel.RunTime = RunTime;
                }
            });
        }

        Simple3DVector _last;
        int _samplesCount = 0;
        double[] _dsamples = new double[10];
        int _skip = 0;
        bool _isSkipping = false;
        double _lastAverage = 0;

        void _accelerometer_ReadingChanged(object sender, AccelerometerHelperReadingEventArgs e)
        {
            if (_last == null)
            {
                _last = e.RawAcceleration;
            }
            else
            {
                Simple3DVector _current = e.RawAcceleration;

                double _cosa = (_current.X * _last.X + _current.Y * _last.Y + _current.Z * _last.Z) /
                (Math.Sqrt(_current.X * _current.X + _current.Y * _current.Y + _current.Z * _current.Z) *
                Math.Sqrt(_last.X * _last.X + _last.Y * _last.Y + _last.Z * _last.Z));

                if (_samplesCount < 10)
                {
                    _dsamples[_samplesCount] += _cosa;
                    _samplesCount += 1;
                }
                else
                {
                    Array.Copy(_dsamples, 1, _dsamples, 0, _dsamples.Length - 1);
                    _dsamples[9] = _cosa;

                    if (_skip >= 10)
                    {
                        _skip = 0;
                        _isSkipping = false;
                    }
                    else
                    {
                        if (_isSkipping) _skip += 1;
                    }

                    if (!_isSkipping)
                    {
                        //double _weightedAverage = (double)Enumerable.Range(0, 9).Select(x => (x + 2) * _dsamples[x]).Sum() / 55;
                        ////don't know why +2 works but as long as it does...or not

                        //non-LINQ alternative
                        double _weightedAverage = 0;
                        for (int i = 0; i < 10; i++)
                        {
                            _weightedAverage += (i + 1) * _dsamples[i];
                        }
                        _weightedAverage = _weightedAverage / 55;
                        
                        if (_weightedAverage > _lastAverage && _weightedAverage < 0.95) //hardcoded threshold
                        {
                            OnStepDetected();
                            _isSkipping = true;
                            _skip = 0;
                        }
                        _lastAverage = _weightedAverage;
                    }
                }

                _last = _current;
            }
            //throw new NotImplementedException();
        }

        public uint WalkTime { get; set; }
        public uint RunTime { get; set; }
        public bool IsRunning { get; set; }
        public bool IsWalking { get; set; }

        private int _stepCount = 0;
        private int _lastCount = 0;
        private int _changeCount = 0;
        private int[] _changeRecord = new int[5];

        private void OnStepDetected()
        {
            _stepCount += 1;
            _totalSteps += 1;
            if (IsRunning) _runningSteps += 1;

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                if (mainModel != null) {                    
                    mainModel.TotalSteps = _totalSteps;
                    mainModel.RunningSteps = _runningSteps;
                    mainModel.WalkTime = WalkTime;
                    mainModel.RunTime = RunTime;
                }
            });

            //update record
            var _rec = GetCurrentRecord();
            if (_rec !=null)
            {
                if (!IsRunning) _rec.WalkingStepCount += 1;
                else _rec.RunningStepCount += 1;
            }
        }

        private Record GetCurrentRecord()
        {
            AppUser _user = AccountsManager.Instance.CurrentUser;
            if (_user != null)
            {
                Record _rec = _user.GetHourlyRecord(DateTime.Now);
                return _rec;
            }
            return null;
        }

        DispatcherTimer _pollTimer;

        private void _pollTimer_Tick(object sender, EventArgs e)
        {
            _changeRecord[_changeCount] = _stepCount - _lastCount;

            if (_changeCount >= 4)
            {
                double _average = Enumerable.Range(0, 4).Select(x => (double)_changeRecord[x]).Sum() / 4;
                if (_average == 0)
                {
                    IsWalking = false;
                    IsRunning = false;
                }
                else if (_average <=3)
                {
                    IsWalking = true;
                    IsRunning = false;
                }
                else
                {
                    IsWalking = false;
                    IsRunning = true;
                }
            }
            _changeCount = (_changeCount >= 4 ? 0 : _changeCount + 1);

            if (IsWalking) { WalkTime += 1; }
            if (IsRunning) { RunTime += 1; }

            //update record
            var _rec = GetCurrentRecord();
            if (_rec != null)
            {
                if (!IsRunning) _rec.WalkTime += 1;
                else _rec.RunTime += 1;
            }

            _lastCount = _stepCount;
        }

        public void StopCounter()
        {
            _accelerometer.Active = false;
        }
    }
}
