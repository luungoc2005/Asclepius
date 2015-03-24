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

        ViewModels.LoginPageViewModel viewModel = new ViewModels.LoginPageViewModel();

        // Constructor
        public LoginPage()
        {
            InitializeComponent();
            this.DataContext=viewModel;
            //objManager = new Connectivity.ConnectionManager();
            //objManager.Initialize();
            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string parameter = string.Empty;
            if (NavigationContext.QueryString.TryGetValue("file", out parameter))
            {
                viewModel = new ViewModels.LoginPageViewModel(parameter);
                this.DataContext = viewModel;
            }

            base.OnNavigatedTo(e);
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/NewAccountPage.xaml", UriKind.Relative));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.LoginButtonClick()) this.NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void HyperlinkButton_Click_1(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/AccountsSwitcher.xaml", UriKind.Relative));
        }

        //private async void Button_Click_1(object sender, RoutedEventArgs e)
        //{
        //    //serviceInfoList = await objManager.EnumerateDevices();
        //    //devicesList.ItemsSource = serviceInfoList;
        //    //devicesList.DisplayMemberPath = "Name";
        //}

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    //if (serviceInfoList != null && devicesList.SelectedIndex >= 0)
        //    //{
        //    //    objManager.Connect(serviceInfoList[devicesList.SelectedIndex]);
        //    //    objManager.MessageReceived += objManager_MessageReceived;
        //    //}
        //}

        //void objManager_MessageReceived(int ID, byte[] data)
        //{
        //    //switch (ID)
        //    //{
        //    //    case (int)'B':
        //    //        double newPoint = System.BitConverter.ToDouble(data, 0);
        //    //        graph1.AddPoint(newPoint);
        //    //        break;

        //    //    default:
        //    //        break;
        //    //}

        //}

        //private void Button_Click_2(object sender, RoutedEventArgs e)
        //{
        //    var objRand = new Random();
        //    graph1.AddPoint((double)objRand.Next(0,300));
        //}

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}