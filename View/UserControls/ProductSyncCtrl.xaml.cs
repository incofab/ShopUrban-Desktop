using Dapper;
using Newtonsoft.Json;
using ShopUrban.Model;
using ShopUrban.Util;
using ShopUrban.Util.Network;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShopUrban.View.UserControls
{
    /// <summary>
    /// Interaction logic for ProductSyncCtrl.xaml
    /// </summary>
    public partial class ProductSyncCtrl : UserControl
    {
        //private string productLastSync;
        //private string productLastSyncServer;
        //private string imagesSynced;
        private MainWindow mainWindow;

        private BackgroundWorker worker = new BackgroundWorker()
        {
            WorkerReportsProgress = true,
            WorkerSupportsCancellation = true
        };

        public ProductSyncCtrl(MainWindow mainWindow)
        {
            InitializeComponent();

            // This UserControl is only activated if products haven't been downloaded
            // and/or Images have not been synced

            handleViews();
            this.mainWindow = mainWindow;

            worker.DoWork += downloadImages_worker;
            worker.RunWorkerCompleted += downloadImages_completed;
            worker.ProgressChanged += downloadImages_progress;
        }

        private void handleViews()
        {
            var productLastSync = Setting.getValue(Setting.KEY_PRODUCT_LAST_SYNC);
            var imagesSynced = Setting.getValue(Setting.KEY_IMAGES_SYNCED);

            tbSyncText.Text = "Records Empty, Synchronize Now";
            imagesLoadingBox.Visibility = Visibility.Collapsed;

            //If products are downloaded but images not synced yet.
            if (productLastSync != null)
            {
                tbSyncText.Text = "Last Sync: " + productLastSync;

                if (imagesSynced != "true")
                {
                    imagesLoadingBox.Visibility = Visibility.Visible;
                }
            }
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            setIsProductSyncing(true);
            
            syncProducts();
        }

        private async void syncProducts()
        {
            var productLastSyncServer = Setting.getValue(Setting.KEY_PRODUCT_LAST_SYNC_SERVER);

            ProductDownloader productDownloader = new ProductDownloader();
            List<Product> products = await productDownloader.loadProducts();

            setIsProductSyncing(false);

            if (products == null) return;

            int totalCount = 0;
            foreach (var product in products)
            {
                Product.createOrUpdate(product);
                ProductUnit.multiCreateOrUpdate(product.productUnits);
                
                if(product.shopProducts != null)
                {
                    var len = product.shopProducts.Count;
                    productLastSyncServer = product.shopProducts[len - 1].updated_at;

                    ShopProduct.multiCreateOrUpdate(product.shopProducts);

                    totalCount += len;
                }
            }

            var productLastSync = DateTime.Now.ToString(KStrings.TIME_FORMAT);
            Setting.createOrUpdate(new Setting
            {
                key = Setting.KEY_PRODUCT_LAST_SYNC,
                value = productLastSync
            });

            productLastSyncServer = DateTime.Now.ToString(KStrings.TIME_FORMAT);
            Setting.createOrUpdate(new Setting
            {
                key = Setting.KEY_PRODUCT_LAST_SYNC_SERVER,
                value = productLastSyncServer
            });

            handleViews();

            MessageBox.Show((string)(products == null ? "Nothing" : totalCount + " products downloaded"));
        }

        private void downloadImages()
        {
            List<ProductUnit> productUnits = ProductUnit.all(
                " Where photo IS NOT NULL AND local_photo IS NULL ");

            if (productUnits == null) return;

            setIsImageSyncing(true);

            worker.RunWorkerAsync(productUnits);

        }

        private void downloadImages_worker(object sender, DoWorkEventArgs e)
        {
            List<ProductUnit> productUnits = (List<ProductUnit>)e.Argument;

            startImagesDownload(productUnits);
        }

        private void startImagesDownload(List<ProductUnit> productUnits)
        {
            var len = productUnits.Count;
            ProductDownloader productDownloader = new ProductDownloader();

            using (WebClient client = new WebClient())
            {
                var i = 0;
                foreach (var productUnit in productUnits)
                {
                    i++;

                    Uri uri = new Uri(productUnit.photo);
                    string filename = System.IO.Path.GetFileName(uri.LocalPath);

                    string filepath = ProductDownloader.imagesFolder + filename;

                    if (File.Exists(filepath))
                    {
                        Helpers.log($"Image ({filename}) File already exists");
                        File.Delete(filepath);
                        filepath = ProductDownloader.imagesFolder + $"{new Random().Next(100, 999)}-{filename}";
                    }

                    client.DownloadFile(uri, filepath);

                    //var filePath = productDownloader.downloadProductImage(productUnit.photo);
                    //var filePath = await productDownloader.downloadImage(productUnit.photo);

                    if (filepath == null)
                    {
                        Helpers.log($"{productUnit.name} has no image");
                        worker.ReportProgress(i, productUnits);
                        continue;
                    }

                    productUnit.local_photo = filepath;
                    ProductUnit.update(productUnit);

                    worker.ReportProgress(i, productUnits);
                }
            }
        }

        private void downloadImages_progress(object sender, ProgressChangedEventArgs e)
        {
            var len = ((List<ProductUnit>)e.UserState).Count;

            imageProgress.Value = e.ProgressPercentage/len * 100;

            tbProgress.Text = $"{e.ProgressPercentage}/{len}";
        }
        private void downloadImages_completed(object sender, RunWorkerCompletedEventArgs e)
        {
            Setting.createOrUpdate(new Setting
            {
                key = Setting.KEY_IMAGES_SYNCED,
                value = "true"
            });

            setIsImageSyncing(false);

            TimerHelper.SetTimeout(1000, () =>
            {
                Application.Current.Dispatcher.Invoke((Action)delegate ()
                {
                    //mainWindow.handleDisplay();
                    MyEventBus.post(new EventMessage(EventMessage.EVENT_PRODUCT_SYNC_COMPLETED, null));

                }, null);
            });
        }

        private bool isSyncing;

        public MainWindow MainWindow { get; }

        private void setIsProductSyncing(bool isLoading) 
        {
            isSyncing = isLoading;
            btnSync.Content = isLoading ? "Loading..." : "Synchronize";
        }
        private void setIsImageSyncing(bool isLoading) 
        {
            isSyncing = isLoading;
            btnSyncImages.Content = isLoading ? "Downloading..." : "Download Images";
        }

        private void btnSyncImages_Click(object sender, RoutedEventArgs e)
        {
            if (isSyncing) return;

            downloadImages();
        }

        private void btnSkipImageSync_Click(object sender, RoutedEventArgs e)
        {
            //MyEventBus.post(new EventMessage(EventMessage.EVENT_PRODUCT_SYNC_COMPLETED, null));
            downloadImages_completed(null, null);
        }
    }
}
