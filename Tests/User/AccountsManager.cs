using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Asclepius.User
{
    class AccountsManager
    {
        IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();

        private static volatile AccountsManager _singletonInstance;
        private static Object _syncRoot = new Object();

        [XmlIgnoreAttribute]
        public static AccountsManager Instance
        {
            get
            {
                if (_singletonInstance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_singletonInstance == null)
                        {
                            _singletonInstance = new AccountsManager();
                        }
                    }
                }
                return _singletonInstance;
            }
        }

        [XmlIgnoreAttribute]
        public AppUser CurrentUser { get; private set; }

        public bool isLoggedIn() { return (CurrentUser != null); }

        public AppUser CreateUser() { return new AppUser(); }

        public void DeleteUser() {
            if (isLoggedIn())
            {

            }
        }

        public AppUser LoadUser(string name) 
        {
            try
            {
                using (IsolatedStorageFileStream fStream = GetUserFile(name))
                {
                    var xmlSer = new XmlSerializer(typeof(AppUser));
                    AppUser ret = (AppUser)xmlSer.Deserialize(fStream);
                    fStream.Close();
                    return (ret==null?new AppUser():ret);
                }
            }
            catch
            {
                return new AppUser();
            }
        }

        public void AcceptUser(AppUser user)
        {
            CurrentUser = user;
        }

        public void AddUser(AppUser user)
        {

        }

        public void SaveUser() { 
            if (isLoggedIn())
            {
                using (IsolatedStorageFileStream fStream = GetUserFile(CurrentUser.FileName + ".xml"))
                {
                    var xmlSer = new XmlSerializer(typeof(AppUser));
                    xmlSer.Serialize(fStream, (AppUser)CurrentUser);
                    fStream.Close();
                }
            }
        }

        public IsolatedStorageFileStream GetUserFile(string fileName)
        {
            if (!isoStore.DirectoryExists("localusers"))
            {
                isoStore.CreateDirectory("localusers");
            }
            return isoStore.OpenFile("localusers/" + fileName, FileMode.OpenOrCreate);
        }

        public string[] listFiles()
        {
            return isoStore.GetFileNames("localusers/*.xml");
        }
    }
}
