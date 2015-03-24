using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asclepius.Helpers
{
    class AppSettings
    {
        private static void SaveSetting(string name, object setting)
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            if (!settings.Contains(name.ToLower()))
            {
                settings.Add(name.ToLower(), setting);
            }
            else
            {
                settings[name.ToLower()] = setting;
            }
            settings.Save();
        }

        private static object ReadSetting(string name, object def)
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains(name.ToLower()))
            {
                return settings[name.ToLower()];
            }
            else
            {
                return def;
            }
        }

        public static string DefaultUserfile
        {
            get
            {
                return (string)ReadSetting("defaultname", "");
            }
            set
            {
                SaveSetting("defaultname", value);
            }
        }

        public static string DefaultPassword
        {
            get
            {
                return (string)ReadSetting("defaultpass", "");
            }
            set
            {
                SaveSetting("defaultpass", value);
            }
        }
    }
}
