using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Asclepius.ViewModels;

namespace Asclepius
{
    public partial class AccountsSwitcher : PhoneApplicationPage
    {
        AccountsSwitcherViewModel viewModel = new AccountsSwitcherViewModel();

        public AccountsSwitcher()
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }


        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/NewAccountPage.xaml", UriKind.Relative));
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (viewModel.isUpdating) return;
            if (viewModel.SelectedAccount > -1)
            {
                try
                {
                    Helpers.AppSettings.DefaultUserfile = viewModel.ListAccounts[viewModel.SelectedAccount].FileName;
                    this.NavigationService.Navigate(new Uri("/LoginPage.xaml?file=" + viewModel.ListAccounts[viewModel.SelectedAccount].FileName, UriKind.Relative));
                }
                catch { }
            }
        }
    }
}