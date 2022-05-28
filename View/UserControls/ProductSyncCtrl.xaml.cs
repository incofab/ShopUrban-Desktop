using Dapper;
using Newtonsoft.Json;
using ShopUrban.Model;
using ShopUrban.Services;
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
    public partial class ProductSyncCtrl : UserControl
    {
        private MainWindow mainWindow;
        //private ProductImagesDownloader ProductImagesDownloader;

        public ProductSyncCtrl(MainWindow mainWindow)
        {
            InitializeComponent();

            // This UserControl is only activated if products haven't been downloaded
            // and/or Images have not been synced

            handleViews();
            this.mainWindow = mainWindow;

            //ProductImagesDownloader = new ProductImagesDownloader(downloadImages_progress, downloadImages_completed);
        }

        private void handleViews()
        {
            var productLastSync = Setting.getValue(Setting.KEY_PRODUCT_LAST_SYNC);
            var imagesSynced = Setting.getValue(Setting.KEY_IMAGES_SYNCED);

            tbSyncText.Text = "Records Empty, Synchronize Now";

            if (productLastSync != null)
            {
                tbSyncText.Text = "Last Sync: " + productLastSync;
            }
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            List<Order> unUploadedOrders = Order.getUnUploadedOrders();

            if(unUploadedOrders.Count > 0)
            {
                Toast.showError("Recent orders have to be uplaoaded first before syncing the products");
                return;
            }

            syncProducts();
        }

        private void syncProducts()
        {
            setIsProductSyncing(true);

            var obj = new SynchronizeShopProduct(
                (List<ShopProduct> shopProducts) => {

                    Toast.showInformation(shopProducts == null ? "Nothing" : shopProducts.Count + " shop products downloaded");

                    setIsProductSyncing(false);
                    setIsImageSyncing(true);

                    handleViews();
                },
                downloadImages_progress, 
                () => {

                    Setting.createOrUpdate(new Setting{key = Setting.KEY_IMAGES_SYNCED, value = "true"});

                    setIsImageSyncing(false);
                }
            );

            obj.syncProductsAndImages();
        }

        private void downloadImages_progress(object sender, ProgressChangedEventArgs e)
        {
            var len = ((List<ProductUnit>)e.UserState).Count;

            imageProgress.Value = e.ProgressPercentage/len * 100;

            tbProgress.Text = $"{e.ProgressPercentage}/{len}";
        }

        private bool isSyncing;
        private void setIsProductSyncing(bool isLoading) 
        {
            isSyncing = isLoading;
            btnSync.Content = isLoading ? "Loading..." : "Synchronize";
        }
        private void setIsImageSyncing(bool isLoading) 
        {
            isSyncing = isLoading;
            //btnSyncImages.Content = isLoading ? "Downloading..." : "Download Images";
            btnSync.Content = isLoading ? "Downloading Images..." : "Synchronize";
        }

        private void btnSyncImages_Click(object sender, RoutedEventArgs e)
        {
            //if (isSyncing) return;
            //downloadImages();
        }

        private void btnSkipImageSync_Click(object sender, RoutedEventArgs e)
        {
            //MyEventBus.post(new EventMessage(EventMessage.EVENT_PRODUCT_SYNC_COMPLETED, null));
            //downloadImages_completed(null, null);
        }

        /*
        private async void syncProductsDeprecated()
        {
            var lastSync = Setting.getValue(Setting.KEY_PRODUCT_LAST_SYNC_SERVER);

            List<Product> products = await ProductSyncHelper.syncProducts(lastSync);

            Toast.showInformation((string)(products == null ? "Nothing" : products.Count + " products downloaded"));

            setIsProductSyncing(false);

            handleViews();

            // After syncing products, Start syncing images

            downloadImages();
        }

        private void downloadImages()
        {
            List<ProductUnit> productUnits = ProductUnit.all(
                " Where photo IS NOT NULL AND local_photo IS NULL ");

            if (productUnits == null) return;

            setIsImageSyncing(true);

            //worker.RunWorkerAsync(productUnits);
            ProductImagesDownloader.beginDownloadProcess(productUnits);
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
                    MyEventBus.post(new EventMessage(EventMessage.EVENT_PRODUCT_SYNC_COMPLETED, null));

                }, null);
            });
        }
        */

    }
}
