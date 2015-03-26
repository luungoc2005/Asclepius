using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Asclepius.User;

namespace Asclepius.Models
{
    class NewAccountPageModel : INotifyPropertyChanged 
    {
        public AppUser user = new AppUser();
        Snapshot firstSnap = new Snapshot();

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public int BirthDay
        {
            get { return user.Birthdate.Day; }
            set
            {
                user.Birthdate = new DateTime(BirthYear, BirthMonth, value);
            }
        }

        public int BirthMonth
        {
            get { return user.Birthdate.Month; }
            set
            {
                user.Birthdate = new DateTime(BirthYear, value, BirthDay);
            }
        }

        public int BirthYear
        {
            get { return user.Birthdate.Year; }
            set
            {
                user.Birthdate = new DateTime(value, BirthMonth, BirthDay);
            }
        }

        public DateTime Birthdate
        {
            get
            {
                return user.Birthdate;
            }
            set
            {
                user.Birthdate = value;
                OnPropertyChanged("Birthdate");
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

        public double Weight
        {
            get
            {
                return firstSnap.Weight;
            }
            set
            {
                firstSnap.Weight = value;
                OnPropertyChanged("Weight");
            }
        }

        public double Height
        {
            get
            {
                return firstSnap.Height;
            }
            set
            {
                firstSnap.Height = value;
                OnPropertyChanged("Height");
            }
        }


        private string pass1;
        private string pass2;

        public string FirstPassword 
        {
            get
            {
                return pass1;
            }
            set
            {
                pass1 = value;
                OnPropertyChanged("FirstPassword");
                OnPropertyChanged("IsPasswordCorrect");
            }
        }

        public string ConfirmPassword
        {
            get
            {
                return pass2;
            }
            set
            {
                pass2 = value;
                OnPropertyChanged("ConfirmPassword");
                OnPropertyChanged("IsPasswordCorrect");
            }
        }

        public bool IsPasswordCorrect
        {
            get
            {
                return (pass1 == pass2);
            }
        }

        public void CreateAccount()
        {
            if (FirstPassword != "" && FirstPassword == ConfirmPassword)
            {
                user.Snapshots.Insert(0, firstSnap);
                user.setPassword(FirstPassword);
                AccountsManager.Instance.AcceptUser(user);
                AccountsManager.Instance.SaveUser();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
