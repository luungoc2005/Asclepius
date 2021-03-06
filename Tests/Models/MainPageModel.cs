﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Asclepius.Helpers;
using Asclepius.User;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using System.IO;
using System.Windows.Media.Imaging;
using Windows.Devices.Enumeration;
using System.Globalization;

namespace Asclepius.Models
{
    class MainPageModel : INotifyPropertyChanged 
    {
        AccountsManager manager = AccountsManager.Instance;
        AppUser user;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public MainPageModel()
        {
            if (AppSettings.DefaultUserfile != "")
            {
                user = manager.CurrentUser;
            }
            StepCounterHelper.Instance.mainModel = this;
            Helpers.StepCounterHelper.Instance.InitializeNumbers();
        }

        public string Birthdate
        {
            get
            {
                return user.Birthdate.Date.ToShortDateString();
            }
        }

        public Asclepius.User.AppUser.Gender Gender
        {
            get
            {
                return user.UserGender;
            }
            set
            {
                user.UserGender = value;
                OnPropertyChanged("Gender");
            }
        }

        public string Username
        {
            get
            {
                return user.Username;
            }
            set
            {
                user.Username = value;
                OnPropertyChanged("Username");
            }
        }

        public string GenderText
        {
            get
            {
                switch (Gender)
                {
                    case AppUser.Gender.Male:
                        return "Male";
                    case AppUser.Gender.Female:
                        return "Female";
                    case AppUser.Gender.Other:
                        return "(Other)";
                    default:
                        return "(Other)";
                }
            }
        }

        public string Description
        {
            get
            {
                return String.Format("{0}, {1} {2} old", GenderText, user.Age, (user.Age >= 1 ? "years" : "year"));
            }
        }

        public double Weight
        {
            get
            {
                return user.Weight;
            }
        }

        public double Height
        {
            get
            {
                return user.Height;
            }
        }

        public double BMI
        {
            get
            {
                return user.BMI;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void ChangeUserAvatar()
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            Microsoft.Phone.Shell.PhoneApplicationService.Current.ContractActivated += Application_ContractActivated;
            openPicker.PickSingleFileAndContinue();
        }

        private async void Application_ContractActivated(object sender, IActivatedEventArgs e)
        {
            var filePickerContinuationArgs = e as FileOpenPickerContinuationEventArgs;

            if (filePickerContinuationArgs != null)
            {
                // Handle file here                
                FileOpenPickerContinuationEventArgs args=filePickerContinuationArgs;
                if (args.Files[0] != null)
                {
                    using (FileRandomAccessStream stream = (FileRandomAccessStream)await args.Files[0].OpenAsync(FileAccessMode.Read))
                    {
                        BitmapImage bi = new BitmapImage();
                        bi.SetSource(stream.AsStreamForRead());
                        user.UserAvatar = bi;
                        OnPropertyChanged("UserAvatar");
                        manager.SaveUser();
                    }
                }
            }
            Microsoft.Phone.Shell.PhoneApplicationService.Current.ContractActivated -= Application_ContractActivated;
        }

        //social

        public BitmapImage UserAvatar
        {
            get
            {
                return user.UserAvatar;
            }
        }

        public string UserStatus
        {
            get
            {
                return user.Status;
            }
            set
            {
                user.Status = value;
                OnPropertyChanged("UserStatus");
            }
        }

        bool _isWalking;
        uint _totalSteps;
        uint _runningSteps;

        public bool IsWalking
        {
            get
            {
                return _isWalking;
            }
            set
            {
                _isWalking = value;
                OnPropertyChanged("IsWalking");
            }
        }

        public uint TotalSteps
        {
            get
            {
                return _day == 0 ? _totalSteps : (uint)selectedTotalSteps;
            }
            set
            {
                _totalSteps = value;
                OnPropertyChanged("TotalSteps");
                OnPropertyChanged("CaloriesBurned");
                OnPropertyChanged("Distance");
            }
        }

        public uint RunningSteps
        {
            get
            {
                return _day == 0 ? _runningSteps : (uint)selectedRunningSteps;
            }
            set
            {
                _runningSteps = value;
                OnPropertyChanged("RunningSteps");
                OnPropertyChanged("CaloriesBurned");
                OnPropertyChanged("Distance");
            }
        }

        public double Distance
        {
            get
            {
                return Math.Round((User.AccountsManager.Instance.CurrentUser.StrideLength / 10000) * (TotalSteps / 2), 2);
            }
        }

        public uint WalkTime { get; set; }
        public uint RunTime { get; set; }

        public double CaloriesBurned
        {
            get
            {
                double Time = ((double)(WalkTime + RunTime) / 3600);
                if (Time == 0) return 0;
                double KPH = Distance / Time;
                //CB = [0.0215 x KPH3 - 0.1765 x KPH2 + 0.8710 x KPH + 1.4577] x WKG x T
                return Math.Round((0.0251 * KPH * KPH * KPH - 0.2157 * KPH * KPH + 0.7888 * KPH + 1.2957) * Weight * Time);
            }
        }

        double _temperature;
        double _heartRate;

        public double Temperature
        {
            get
            {
                return _temperature;
            }
            set
            {
                _temperature = value;
                OnPropertyChanged("Temperature");
            }
        }

        public double HeartRate
        {
            get
            {
                return _heartRate;
            }
            set
            {
                _heartRate = value;
                OnPropertyChanged("HeartRate");
            }
        }

        int _bytes;
        public int BytesReceived
        {
            get
            {
                return _bytes;
            }
            set
            {
                _bytes = value;
                OnPropertyChanged("BytesReceived");
            }
        }

        int _day = 0;
        public int SelectedDay
        {
            get
            {
                return _day;
            }
            set
            {
                _day = value;
                //day changing
                if (_day != 0)
                {
                    List<Record> tempList = manager.CurrentUser.GetDailyRecord(DateTime.Now - TimeSpan.FromDays(_day));
                    selectedTotalSteps = tempList.Sum((Record r) => { return r.WalkingStepCount + r.RunningStepCount; });
                    selectedRunningSteps = tempList.Sum((Record r) => { return r.RunningStepCount; });
                }
                OnPropertyChanged(null);
            }
        }

        private int selectedTotalSteps;
        private int selectedRunningSteps;

        public string DayText
        {
            get
            {
                CultureInfo ci = new CultureInfo("en-GB");

                string format = "D";

                if (_day == 0)
                    return "Today";
                else if (_day == 1)
                    return "Yesterday";
                else
                {
                    DateTime time = DateTime.Now - TimeSpan.FromDays(_day);
                    return time.ToString(format, ci);
                }
            }
        }

        //void _bluetooth_MessageReceived(int ID, byte[] data)
        //{
        //    string data = Encoding.UTF8.GetString(data,0,data.Length);
        //}
    }
}
