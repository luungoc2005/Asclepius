using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Text;
using System.Windows.Threading;
using Asclepius.Connectivity;

namespace Asclepius
{
    public partial class MainPage : PhoneApplicationPage
    {
        Models.MainPageModel Model = new Models.MainPageModel();
        DispatcherTimer _updateTimer;
        UDPHelper _udpHelper;

        public MainPage()
        {
            InitializeComponent();
            this.DataContext = Model;

            //draw steps graph

            for (int i = 0; i < 24; i++)
            {
                graphSteps.xAxisPoints.Add(i);
            }
            graphSteps.yAxisPoints.Add(10000);
            graphSteps.yAxisPoints.Add(5000);
            graphSteps.xAxisMaxCount = 24;

            graphSteps.DrawAxis();

            graphHeart.yAxisPoints.Add(120);
            graphHeart.yAxisPoints.Add(80);
            graphHeart.xAxisMaxCount = 30;

            graphHeart.DrawAxis();

            _updateTimer = new DispatcherTimer();
            _updateTimer.Interval = TimeSpan.FromSeconds(5);
            _updateTimer.Tick += _updateTimer_Tick;

            _updateTimer_Tick(this, new EventArgs());

            _udpHelper = UDPHelper.Instance;
            _udpHelper.OnClientUpdate += _udpHelper_OnClientUpdate;
        }

        void _udpHelper_OnClientUpdate(object sender, EventArgs e)
        {
            listFriends.ItemsSource = _udpHelper.ClientList;
        }

        void _updateTimer_Tick(object sender, EventArgs e)
        {
            if (User.AccountsManager.Instance.CurrentUser != null)
            {
                graphSteps.DataSource = User.AccountsManager.Instance.CurrentUser.GetAccumulatedDailyRecord(DateTime.Now - TimeSpan.FromDays(Model.SelectedDay));
                graphSteps.DrawGraph();
                graphSteps.DrawAxis();
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            _updateTimer.Stop();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _updateTimer.Start();
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

        //Bluetooth device
        Connectivity.BluetoothConnection _bluetooth = new Connectivity.BluetoothConnection();

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            Model.ChangeUserAvatar();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await _bluetooth.EnumerateDevices();
            lbDevices.ItemsSource = _bluetooth.listDevices;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (lbDevices.SelectedIndex >= 0)
            {
                MessageBox.Show("Connecting to " + _bluetooth.listDevices[lbDevices.SelectedIndex].Name);
                _bluetooth.Connect(_bluetooth.listDevices[lbDevices.SelectedIndex]);
                _bluetooth.MessageReceived += _bluetooth_MessageReceived;
                //Model.BytesReceived += 1;
            }
        }

        private void _bluetooth_MessageReceived(float num1, float num2)
        {
            Model.Temperature = Math.Round(Convert.ToDouble(num1),2);
            Model.HeartRate = Math.Round(Convert.ToDouble(num2));
            Model.BytesReceived += sizeof(float) * 2;

            updateHeartRate(num2);
        }

        List<double> heartRateHistory = new List<double>();
        private void updateHeartRate(float rate)
        {
            if (heartRateHistory != null)
            {
                if (heartRateHistory.Count >= 30) heartRateHistory.RemoveAt(0);
                heartRateHistory.Add(Convert.ToDouble(rate));
                graphHeart.DataSource = heartRateHistory;
                graphHeart.DrawAxis();
                graphHeart.DrawGraph();
            }
        }

        private void ToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            _udpHelper.IsDiscoveryEnabled = true;
        }

        private void ToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            _udpHelper.IsDiscoveryEnabled = false;
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplicationBar.IsVisible = ((((Pivot)sender).SelectedIndex) == 1);
        }

        private void ApplicationBarIconButton_Click_Back(object sender, EventArgs e)
        {
            Model.SelectedDay += 1;
            _updateTimer_Tick(this, new EventArgs());
            UpdateAppBar();
        }

        private void ApplicationBarIconButton_Click_Next(object sender, EventArgs e)
        {
            Model.SelectedDay -= 1;
            _updateTimer_Tick(this, new EventArgs());
            UpdateAppBar();
        }

        private void UpdateAppBar()
        {
            ApplicationBarIconButton btn = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
            btn.IsEnabled = (Model.SelectedDay != 0);
        }

        private async void HyperlinkButton_Click_1(object sender, RoutedEventArgs e)
        {
            await _bluetooth.EnumerateDevices();
            foreach (Windows.Devices.Enumeration.DeviceInformation info in _bluetooth.listDevices)
            {
                if (info.Name == "Meleco")
                {
                    _bluetooth.Connect(info);
                    _bluetooth.MessageReceived += _bluetooth_MessageReceived;
                    connectBtn.IsEnabled = false;
                }
            }

            if (lbDevices.SelectedIndex >= 0)
            {
                MessageBox.Show("Connecting to " + _bluetooth.listDevices[lbDevices.SelectedIndex].Name);
                _bluetooth.Connect(_bluetooth.listDevices[lbDevices.SelectedIndex]);
                _bluetooth.MessageReceived += _bluetooth_MessageReceived;
                //Model.BytesReceived += 1;
            }
        }
    }
}