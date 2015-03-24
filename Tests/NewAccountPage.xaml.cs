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
        ViewModels.NewAccountPageViewModel viewModel = new ViewModels.NewAccountPageViewModel();

        public NewAccountPage()
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            viewModel.CreateAccount();
            Asclepius.Helpers.AppSettings.DefaultUserfile = viewModel.user.FileName;

            this.NavigationService.Navigate(new Uri("/LoginPage.xaml", UriKind.Relative));
        }
    }
}