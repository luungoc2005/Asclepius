using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Asclepius.Models;

namespace Asclepius
{
    public partial class AccountsSwitcher : PhoneApplicationPage
    {
        AccountsSwitcherModel Model = new AccountsSwitcherModel();

        public AccountsSwitcher()
        {
            InitializeComponent();
            this.DataContext = Model;
        }


        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/NewAccountPage.xaml", UriKind.Relative));
        }

        private void ListBox_DoubleTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (Model.isUpdating) return;
            if (Model.SelectedAccount > -1)
            {
                try
                {
                    Helpers.AppSettings.DefaultUserfile = Model.ListAccounts[Model.SelectedAccount].FileName;
                    this.NavigationService.Navigate(new Uri("/LoginPage.xaml?file=" + Model.ListAccounts[Model.SelectedAccount].FileName, UriKind.Relative));
                }
                catch { }
            }
        }
    }
}