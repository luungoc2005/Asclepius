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

namespace Asclepius
{
    public partial class MainPage : PhoneApplicationPage
    {
        Models.MainPageModel Model = new Models.MainPageModel();

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
            graphSteps.DrawGraph();
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
            Model.Temperature = Convert.ToDouble(num1);
            Model.HeartRate = Convert.ToDouble(num2);
            Model.BytesReceived += sizeof(float) * 2;
        }

        //List<byte> _received = new List<byte>();
        //private void _bluetooth_MessageReceived(byte data)
        //{
        //    if (_received.Count == 0)
        //    {
        //        _received.Add(data);
        //    }
        //    else
        //    {
        //        if (_received.Count > 1 && (data == (byte)'T' || data == (byte)'B'))
        //        {
        //            string strNum = Encoding.UTF8.GetString(_received.Skip(1).ToArray(), 0, _received.Count - 1);
        //            if (_received[0] == (byte)'T')
        //            {
        //                Model.Temperature = Convert.ToDouble(strNum);
        //            }
        //            else
        //            {
        //                Model.HeartRate = Convert.ToInt32(strNum);
        //            }
        //        }
        //        else
        //        {
        //            _received.Add(data);
        //        }
        //    }
        //    Model.BytesReceived = _received.Count;
        //}

    }
}