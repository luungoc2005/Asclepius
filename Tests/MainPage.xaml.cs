using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Asclepius
{
    public partial class MainPage : PhoneApplicationPage
    {
        ViewModels.MainPageViewModel viewModel = new ViewModels.MainPageViewModel();

        public MainPage()
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);

            if (MessageBox.Show("Would you like to sign out of this account?", "Prompt", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                User.AccountsManager.Instance.SaveUser();
                Helpers.AppSettings.DefaultPassword = "";
                this.NavigationService.Navigate(new Uri("/LoginPage.xaml", UriKind.Relative));
            }
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ChangeUserAvatar();
        }
    }
}