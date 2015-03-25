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
    public partial class NewAccountPage : PhoneApplicationPage
    {
        Models.NewAccountPageModel Model = new Models.NewAccountPageModel();

        public NewAccountPage()
        {
            InitializeComponent();
            this.DataContext = Model;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Model.CreateAccount();
            Asclepius.Helpers.AppSettings.DefaultUserfile = Model.user.FileName;

            this.NavigationService.Navigate(new Uri("/LoginPage.xaml", UriKind.Relative));
        }
    }
}