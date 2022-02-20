using Newtonsoft.Json;
using ShopUrban.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShopUrban.Util.Network
{
    class ProductImagesDownloader
    {
        public static string ImagesFolder = KStrings.BASE_FOLDER + "products\\images\\";

        private BackgroundWorker worker = new BackgroundWorker()
        {
            WorkerReportsProgress = true,
            WorkerSupportsCancellation = true
        };

        public ProductImagesDownloader(
            ProgressChangedEventHandler downloadImages_progress = null,
            RunWorkerCompletedEventHandler downloadImages_completed = null
        ){
            if (!Directory.Exists(ImagesFolder))
            {
                Directory.CreateDirectory(ImagesFolder);
            }

            worker.DoWork += downloadImages_worker;

            if(downloadImages_progress != null) worker.ProgressChanged += downloadImages_progress;
            if(downloadImages_completed != null) worker.RunWorkerCompleted += downloadImages_completed;
        }

        public void beginDownloadProcess(List<ProductUnit> productUnits)
        {
            worker.RunWorkerAsync(productUnits);
        }

        private void downloadImages_worker(object sender, DoWorkEventArgs e)
        {
            List<ProductUnit> productUnits = (List<ProductUnit>)e.Argument;

            downloadImages(productUnits);
        }

        private void downloadImages(List<ProductUnit> productUnits)
        {
            var len = productUnits.Count;

            using (WebClient client = new WebClient())
            {
                var i = 0;
                foreach (var productUnit in productUnits)
                {
                    i++;

                    if (string.IsNullOrEmpty(productUnit.photo))
                    {
                        worker.ReportProgress(i, productUnits);
                        continue;
                    }

                    Uri uri = new Uri(productUnit.photo);
                    string filename = System.IO.Path.GetFileName(uri.LocalPath);

                    string filepath = ImagesFolder + filename;

                    if (File.Exists(filepath))
                    {
                        Helpers.log($"Image ({filename}) File already exists");

                        if (!filepath.Equals(productUnit.local_photo))
                        {
                            productUnit.local_photo = filepath;
                            ProductUnit.update(productUnit);
                        }

                        worker.ReportProgress(i, productUnits);
                        continue;
                    }

                    //var filePath = productDownloader.downloadProductImage(productUnit.photo);
                    //var filePath = await productDownloader.downloadImage(productUnit.photo);
                    client.DownloadFile(uri, filepath);

                    productUnit.local_photo = filepath;
                    ProductUnit.update(productUnit);

                    worker.ReportProgress(i, productUnits);
                }
            }
        }

    }
}
