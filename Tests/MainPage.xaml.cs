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

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ChangeUserAvatar();
        }
    }
}