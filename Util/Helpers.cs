using ShopUrban.Model;
using ShopUrban.View.Receipt;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows;

namespace ShopUrban.Util
{
    public static class Helpers
    {

        public static void ShuffleList<T>(this IList<T> list)
        {
            Random random = new Random();
            int n = list.Count;

            for (int i = list.Count - 1; i > 1; i--)
            {
                int rnd = random.Next(i + 1);

                T value = list[rnd];
                list[rnd] = list[i];
                list[i] = value;
            }
        }
        public static string Reverse(string s)
        {
            if (s == null) return null;
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public static async Task DownloadFileTaskAsync(this HttpClient client, Uri uri, string FileName)
        {
            using (var s = await client.GetStreamAsync(uri))
            {
                using (var fs = new FileStream(FileName, FileMode.CreateNew))
                {
                    await s.CopyToAsync(fs);
                }
            }
        }

        public static string md5(string str)
        {
            byte[] asciiBytes = ASCIIEncoding.ASCII.GetBytes(str);
            byte[] hashedBytes = MD5CryptoServiceProvider.Create().ComputeHash(asciiBytes);
            string hashedString = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();

            return hashedString;
        }

        public static string addSpaces(string str, int interval)
        {
            if (str == null) return str;

            str = str.Replace(" ", "");
            int len = str.Length;
            int remaining = len;

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < len; i += interval)
            {
                if (remaining <= interval)
                {
                    sb.Append(str.Substring(i, remaining));
                }
                else
                {
                    sb.Append(str.Substring(i, interval));
                    sb.Append(" ");
                }

                remaining = (remaining > interval) ? (remaining - interval) : remaining;
            }

            return sb.ToString();
        }

        public static bool isInternetAvailable()
        {
            try
            {
                System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();

                System.Net.NetworkInformation.PingReply result = ping.Send("www.google.com");

                if (result.Status == System.Net.NetworkInformation.IPStatus.Success)
                    return true;
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static string getFilenameFromPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return path;
            int lastIndex = path.LastIndexOf("?");
            
            if(lastIndex != -1){
                path = HttpUtility.HtmlDecode(path);

                Uri uri = new Uri("http://fakeurl.com" + path.Substring(lastIndex));
                string filename = HttpUtility.ParseQueryString(uri.Query).Get("filename");
                
                if(filename != null) return getFilenameFromPath(filename);

                path = path.Substring(0, lastIndex);
            }

            return System.IO.Path.GetFileName(path);
        }
        public static string stripPtags(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;

            str = str.Replace("<p>", "<div>").Replace("</p>", "</div>");

            return str;
        }
        public static string wrapInDivTag(string str)
        {
            var style = "<style> p{ margin-top: 3px; margin-bottom: 3px } </style>";

            return "<div>" +
                style + "<div>" + str + "</div>" 
                + "</div>";
        }
        public static bool IsWindowOpen<T>(string name = "") where T : Window
        {
            return string.IsNullOrEmpty(name)
               ? Application.Current.Windows.OfType<T>().Any()
               : Application.Current.Windows.OfType<T>()
                .Any(w => w.Name.Equals(name));
        }

        public static string naira(double amount)
        {
            return KStrings.NAIRA_SIGN+numberFormat(amount);
        }
        public static string numberFormat(double number)
        {
            return string.Format("{0:#,##0.##}", number);
        }

        public static string filterNumber(string pin)
        {
            if (string.IsNullOrEmpty(pin)) return pin;

            return pin.Replace(" ", "").Replace("-", "").Trim();
        }
        
        public static string getLoginToken()
        {
            var staff = getLoggedInStaff();

            return staff == null ? null : staff.token;
        }

        public static Staff getLoggedInStaff()
        {
            if (MainWindow.staffDetail != null) return MainWindow.staffDetail;

            var id = Setting.getValue(Setting.KEY_LOGGED_IN_STAFF_ID);

            int idInt;
            int.TryParse(id, out idInt);

            return Staff.findById(idInt);
        }

        public static void log(string message)
        {
            if (!KStrings.DEV) return;

            Trace.WriteLine(message);
        }
        public static void playSound(string path)
        {
            SoundPlayer player = new SoundPlayer(path);
            player.Load();
            player.Play();
        }
        public static void playErrorSound()
        {
            playSound(Environment.CurrentDirectory + @"/buzz.wav");
        }
        public static bool IsPhoneNumberValid(string number)
        {
            //return Regex.Match(number, @"^(\+[0-9]{9})$").Success;
            return Regex.Match(number, @"^[0][0-9]{10}$").Success;
        }
        public static string getVersionNo()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();

            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

            return fileVersionInfo.FileVersion;
        }

        public static void openUrl(string url)
        {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }

        public static void showReceipt(Order order)
        {
            var layout = Setting.getValue(Setting.KEY_RECEIPT_LAYOUT);

            if (Setting.RECEIPT_LAYOUT_58MM.Equals(layout))
            {
                new Thermal58mm(order).ShowDialog();
            }
            else if (Setting.RECEIPT_LAYOUT_88MM.Equals(layout))
            {
                new Thermal88mm(order).ShowDialog();
            }
            else if (Setting.RECEIPT_LAYOUT_A4.Equals(layout))
            {
                new A4Receipt(order).ShowDialog();
            }
            else
            {
                new Thermal88mm(order).ShowDialog();
            }
        }
        

    }
}
