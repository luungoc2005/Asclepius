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

        public byte[] Password { get; set; }


        public void setPassword(string pass)
        {
            Password = hashPassword(pass);
        }

        public bool comparePassword(string pass)
        {
            if (Password == null) return false;

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
            Birthdate = DateTime.Now;
            Username = "New User";
        }

        public void SortLists()
        {
            Records.Sort(delegate(Record x, Record y) { return x.CompareTo(y); });
        }

        #region "Basic info"

        public enum Gender : byte { Other, Male, Female };
        public Gender UserGender { get; set; }

        public DateTime Birthdate { get; set; }

        public List<Record> Records = new List<Record>();
        public List<Snapshot> Snapshots = new List<Snapshot>();

        public double Weight //user weight in kg
        {
            get
            {
                if (Snapshots.Count == 0)
                {
                    return 0;
                }
                else
                {
                    return Snapshots[0].Weight;
                }
            }
        }

        public double Height //user height in centimeters
        {
            get
            {
                if (Snapshots.Count == 0)
                {
                    return 0;
                }
                else
                {
                    return Snapshots[0].Height;
                }
            }
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

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public double Stride { get; set; }

        [XmlIgnore]
        public double StrideLength
        {
            get
            {
                if (Stride == 0)
                {
                    return Stride;
                }
                else
                {
                    switch (UserGender)
                    {
                        case Gender.Other:
                            return Height * 0.415;
                        case Gender.Male:
                            return Height * 0.415;
                        case Gender.Female:
                            return Height * 0.413;
                        default:
                            return Height * 0.413;
                    }
                }
            }
            set
            {
                Stride = value;
            }
        }

        #endregion

        #region "Social"

        public List<string> Friends;
        public string Status { get; set; }

        #endregion

    }
}
