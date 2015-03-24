using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Asclepius.Helpers;
using Asclepius.User;

namespace Asclepius.ViewModels
{
    class LoginPageViewModel : INotifyPropertyChanged 
    {
        AccountsManager manager = AccountsManager.Instance;
        AppUser user;

        public LoginPageViewModel()
        {
            if (AppSettings.DefaultUserfile != "")
            {
                user = manager.LoadUser(AppSettings.DefaultUserfile + ".xml");
            }
            else { user = new AppUser(); }
        }

        public LoginPageViewModel(string filename)
        {
            if (filename != "")
            {
                user = manager.LoadUser(filename + ".xml");
            }
            else { user = new AppUser(); }
        }

        public BitmapImage UserAvatar
        {
            get
            {
                return user.UserAvatar;
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
            } 
        }

        public bool IsUserAvailable
        {
            get
            {
                return (AppSettings.DefaultUserfile != null);
            }
        }

        public string Password { get; set; }

        public bool IsPasswordInvalid { get; set; }

        public bool LoginButtonClick()
        {
            if (Username == "") return false;
            if (user.comparePassword(Password)) {
                //Login
                manager.AcceptUser(user);
                return true;
            }
            else
            {
                IsPasswordInvalid = true;
                return false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
