using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Asclepius.User;
using System.ComponentModel;

namespace Asclepius.Models
{
    public class AccountsSwitcherModel : INotifyPropertyChanged
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

        public AccountsSwitcherModel()
        {
            isUpdating = true;
            listAccounts.Clear();
            foreach (string tmp in AccountsManager.Instance.listFiles())
            {
                listAccounts.Add(AccountsManager.Instance.LoadUser(tmp));
            }
            OnPropertyChanged("ListAccounts");

            if (listAccounts.Count > 0)
            {
                for (int i = 0; i <= listAccounts.Count - 1; i++)
                {
                    if (listAccounts[i].FileName == Helpers.AppSettings.DefaultUserfile)
                    {
                        SelectedAccount = i;
                    }
                }
            }
            isUpdating = false;
        }
        
        public bool isUpdating = false;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
