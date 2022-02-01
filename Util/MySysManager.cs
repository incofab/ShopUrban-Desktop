using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace ShopUrban.Util
{
    public class MySysManager
    {
        const int PRODUCT_KEY_SIZE = 12;

        private string productKey;

        private static MySysManager mySysManager;

        public static MySysManager getInstance()
        {
            if (mySysManager == null)
            {
                mySysManager = new MySysManager();
            }
            return mySysManager;
        }

        public string getProductKey()
        {
            if (productKey != null) return productKey;

            string processor = this.processor();
            string harddisk = this.harddisk();
            string motherBoard = this.motherBoard();

            string uniqueId = $"{processor}-1ad-{harddisk}-2bd-{motherBoard}";

            uniqueId = Helpers.Reverse(uniqueId);

            uniqueId = Helpers.md5(uniqueId);

            uniqueId = Helpers.Reverse(uniqueId);

            uniqueId = uniqueId.Substring(0, PRODUCT_KEY_SIZE);

            productKey = this.replaceLetters(uniqueId).ToUpper();

            return productKey;
        }

        private string processor()
        {
            //ManagementObjectCollection mbsList = null;
            //ManagementObjectSearcher mbs = new ManagementObjectSearcher("Select * From Win32_processor");
            //mbsList = mbs.Get();
            //string id = "";
            //foreach (ManagementObject mo in mbsList)
            //{
            //    id = mo["ProcessorID"].ToString();
            //}
            return "";// id;
        }

        private string harddisk()
        {
            //ManagementObject dsk = new ManagementObject(@"win32_logicaldisk.deviceid=""c:""");
            //dsk.Get();
            //string id = dsk["VolumeSerialNumber"].ToString();
            return "";// id;
        }

        private string motherBoard()
        {
            //ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");

            //ManagementObjectCollection moc = mos.Get();

            //string serial = "MOTHER_BOARD";

            //foreach (ManagementObject mo in moc)
            //{
            //    serial = (string)mo["SerialNumber"];
            //}

            return "";// serial;
        }

        private String replaceLetters(String str)
        {
            if (str == null) return null;
            str = str.Replace("0", "z");
            str = str.Replace("1", "k");
            str = str.Replace("5", "x");
            return str;
        }

    }
}
