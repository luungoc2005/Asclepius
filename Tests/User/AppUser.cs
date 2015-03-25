using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace Asclepius.User
{
    public class AppUser
    {
        [XmlIgnore]
        public BitmapImage UserAvatar
        {
            get
            {
                if (UserAvatarSerialized != null)
                {
                    using (MemoryStream ms = new MemoryStream(UserAvatarSerialized))
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.SetSource(ms);
                        return bitmap;
                    }
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value != null) 
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        WriteableBitmap wbi = new WriteableBitmap(value);
                        wbi.SaveJpeg(ms, Math.Min(128,wbi.PixelWidth), Math.Min(128,wbi.PixelHeight), 0, 85); //cut to 128x128
                        UserAvatarSerialized = ms.ToArray();
                    }
                };
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [XmlElement("UserAvatar")]
        public byte[] UserAvatarSerialized { get; set; }

        public enum Gender : byte { Other, Male, Female };

        public string Username { get; set; }

        private string fileName = "";
        public string FileName
        {
            get
            {
                if (fileName == "")
                {
                    //var random = new Random();
                    //byte[] temp = new byte[16];
                    //random.NextBytes(temp);

                    //fileName = AsciiToString(temp);
                    fileName = System.IO.Path.GetRandomFileName();
                }
                return fileName;
            }
            set
            {
                this.fileName = value;
            }
        }

        public static string AsciiToString(byte[] bytes)
        {
            return string.Concat(bytes.Select(b => b <= 0x7f ? (char)b : '?'));
        } 

        public Gender UserGender { get; set; }

        public DateTime Birthdate { get; set; }

        public List<Record> Records;
        public List<Snapshot> Snapshots;
        public List<Record> UserGoals;
        public List<Record> AutoGoals;

        public double Weight { get; set; } //user weight in kg
        public double Height { get; set; } //user height in meters

        public byte[] Password { get; set; }


        public void setPassword(string pass)
        {
            Password = hashPassword(pass);
        }

        public bool comparePassword(string pass)
        {
            byte[] hash = hashPassword(pass);
            for (int i = 0; i < Math.Min(hash.Length,Password.Length); i++)
            {
                if (hash[i] != Password[i]) return false;
            }
            return true;
        }

        private byte[] hashPassword(string pass)
        {
            var hash = new SHA256Managed();
            byte[] password = Encoding.Unicode.GetBytes(pass);
            return hash.ComputeHash(password);
        }

        public AppUser()
        {
            Records = new List<Record>();
            UserGoals=new List<Record>();
            AutoGoals = new List<Record>();
            Birthdate = DateTime.Now;
            Username = "New User";
        }

        public void SortLists()
        {
            Records.Sort(delegate(Record x, Record y) { return x.CompareTo(y); });
            UserGoals.Sort(delegate(Record x, Record y) { return x.CompareTo(y); });
            AutoGoals.Sort(delegate(Record x, Record y) { return x.CompareTo(y); });
        }

        public int Age
        {
            get
            {
                DateTime now = DateTime.Now;
                int age = now.Year - Birthdate.Year;
                if (now.Month < Birthdate.Month || (now.Month == Birthdate.Month && now.Day < Birthdate.Day)) age--;
                return age;
            }
        }

        //Social
        
        public List<string> Friends;
        public string Status { get; set; }
    }
}
