using System;
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

namespace Asclepius.ViewModels
{
    class MainPageViewModel : INotifyPropertyChanged 
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

        public MainPageViewModel()
        {
            if (AppSettings.DefaultUserfile != "")
            {
                user = manager.CurrentUser;
            }
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
            set
            {
                user.Weight = value;
                OnPropertyChanged("Weight");
            }
        }

        public double Height
        {
            get
            {
                return user.Height;
            }
            set
            {
                user.Height = value;
                OnPropertyChanged("Height");
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

        public BitmapImage UserAvatar
        {
            get
            {
                return user.UserAvatar;
            }
        }
    }
}
