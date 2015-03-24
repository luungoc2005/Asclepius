using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Asclepius.User;
using System.ComponentModel;

namespace Asclepius.ViewModels
{
    public class AccountsSwitcherViewModel : INotifyPropertyChanged
    {
        List<AppUser> listAccounts=new List<AppUser>();
        
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public List<AppUser> ListAccounts
        {
            get
            {
                return listAccounts;
            }
        }

        [System.ComponentModel.DefaultValue(-1)]
        public int SelectedAccount { get; set; }

        public AccountsSwitcherViewModel()
        {
            listAccounts.Clear();
            foreach (string tmp in AccountsManager.Instance.listFiles())
            {
                listAccounts.Add(AccountsManager.Instance.LoadUser(tmp));
            }
            OnPropertyChanged("ListAccounts");
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
