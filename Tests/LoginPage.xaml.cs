using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Asclepius.Resources;
using Windows.Devices.Enumeration;

namespace Asclepius
{
    public partial class LoginPage : PhoneApplicationPage
    {
        //Connectivity.ConnectionManager objManager;
        //List<DeviceInformation> serviceInfoList;

        Models.LoginPageModel Model = new Models.LoginPageModel();

        // Constructor
        public LoginPage()
        {
            InitializeComponent();
            this.DataContext=Model;

            this.Loaded += LoginPage_Loaded;

            //objManager = new Connectivity.ConnectionManager();
            //objManager.Initialize();
            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        void LoginPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (User.AccountsManager.Instance.listFiles().Length == 0)
            {
                this.NavigationService.Navigate(new Uri("/NewAccountPage.xaml", UriKind.Relative));
            }
            if (Helpers.AppSettings.DefaultPassword != "") Button_Click(this, new RoutedEventArgs());
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to quit?", "Confirmation", MessageBoxButton.OKCancel) == MessageBoxResult.OK) {
                Application.Current.Terminate();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string parameter = string.Empty;
            if (NavigationContext.QueryString.TryGetValue("file", out parameter))
            {
                Model = new Models.LoginPageModel(parameter);
                this.DataContext = Model;
            }

            base.OnNavigatedTo(e);
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/NewAccountPage.xaml", UriKind.Relative));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Model.LoginButtonClick()) this.NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void HyperlinkButton_Click_1(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/AccountsSwitcher.xaml", UriKind.Relative));
        }

    }
}