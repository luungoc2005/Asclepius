using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asclepius.Helpers
{
    class NotificationsHelper
    {
        private static volatile NotificationsHelper _singletonInstance;
        private static Object _syncRoot = new Object();
        private Dictionary<int,NotifcationItem> itemsDict=new Dictionary<int,NotifcationItem>();
        private List<int> itemsList = new List<int>();
        private List<int> disabledItems = new List<int>();

        private NotificationsHelper()
        {
            itemsDict.Add(1,new NotifcationItem("Location disabled", "Location is disabled on this device. Please enable it from Settings",
                delegate () {
                    //await SenseHelper.LaunchLocationSettingsAsync();
                    itemsList.Remove(1);
                }));

            itemsDict.Add(2,new NotifcationItem("Motion data disabled", "Motion data is disabled on this device. Please enable it from Settings",
                delegate() {
                    //await SenseHelper.LaunchSenseSettingsAsync();
                    itemsList.Remove(2);
                }));

            itemsDict.Add(3,new NotifcationItem("Sensor not found", "The required sensor is not available on this device.",
                delegate() {}, false));

            itemsDict.Add(4, new NotifcationItem("Take snapshot", "You are recommended to take a snapshot regularly",
                delegate() { App.RootFrame.Navigate(new Uri("/SnapshotPage.xaml", UriKind.Relative)); }, false));
            
        }

        public void pushNotification(int ID)
        {
            if (itemsDict.ContainsKey(ID) && !disabledItems.Contains(ID) && !itemsList.Contains(ID)) itemsList.Add(ID);
        }

        public void disableItem(int ID)
        {
            if (!disabledItems.Contains(ID)) disabledItems.Add(ID);
            if (itemsList.Contains(ID)) itemsList.Add(ID);
        }

        public static NotificationsHelper Instance
        {
            get
            {
                if (_singletonInstance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_singletonInstance == null)
                        {
                            _singletonInstance = new NotificationsHelper();
                        }
                    }
                }
                return _singletonInstance;
            }
        }

        public delegate void actionHandler();

        public class NotifcationItem
        {
            string title;
            string userText;
            bool isResolvable;
            actionHandler onAction;
            public NotifcationItem (string title, string userText, actionHandler onAction, bool isResolvable=true) {
                this.title = title;
                this.userText = userText;
                this.onAction=onAction;
                this.isResolvable=isResolvable;
            }
        }
    }
}
