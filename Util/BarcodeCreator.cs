using BarcodeLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShopUrban.Util
{
    class BarcodeCreator
    {
        public const int WIDTH = 246;//290;
        public const int HEIGHT = 50;//70;//120;

        public static string barcodeFolder = KStrings.BASE_FOLDER + @"barcodes\";

        private string getBarcodeFolder()
        {
            if (!Directory.Exists(barcodeFolder))
            {
                Directory.CreateDirectory(barcodeFolder);
            }
            return barcodeFolder;
        }

        public string createBarcode(string NumericString, int imageWidth = WIDTH, int imageHeight = HEIGHT)
        {
            try
            {
                string filename = getBarcodeFolder() + NumericString + ".png";

                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }

                // Create an instance of the API
                Barcode barcodLib = new Barcode();

                Color foreColor = Color.Black; // Color to print barcode
                Color backColor = Color.Transparent; // background color

                // Generate the barcode with your settings
                Image barcodeImage = barcodLib.Encode(
                    TYPE.CODE128, NumericString, foreColor, backColor, imageWidth, imageHeight);
                //Image barcodeImage = barcodLib.Encode(TYPE.UPCA, //NumString must be 12 in number
                //    "038000356216", foreColor, backColor, imageWidth, imageHeight);

                if (barcodeImage == null) return null;

                barcodeImage.Save(filename, ImageFormat.Png);

                return filename;
            }
            catch (Exception e)
            {
                MessageBox.Show("Error: "+e.Message);
                return null;
            }
        }

    }
}
